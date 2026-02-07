using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.RegularExpressions;
using VetLink.Data.Entities;
using VetLink.Data.Enums;
using VetLink.Services.Helper;
using VetLink.Services.Services.AccountService.AuditLogService;
using VetLink.Services.Services.AccountService.Dtos.AuthDtos;
using VetLink.Services.Services.AccountService.MailService;
using VetLink.Services.Services.AccountService.MailService.Dtos;
using VetLink.Services.Services.AccountService.OTPService;
using VetLink.Services.Services.AccountService.TokenService;
using VetLink.Services.Services.AccountService.TokenService.Dtos;
using VetLink.Services.Services.CachedService;
using VetLink.Services.Services.Email;
using VetLink.Services.Services.Notifications;

namespace VetLink.Services.Services.AccountService
{
	public abstract class BaseAccountService
	{
		#region Constants
		protected const int MaxLoginAttempts = 3;
		protected const int LockoutMinutes = 60;
		protected const int OtpExpiryMinutes = 10;
		protected const string BuyerRole = "Buyer";
		protected const string SellerRole = "Seller";
		protected const string AdminRole = "Admin";
		#endregion

		#region Dependencies
		protected readonly IMapper _mapper;
		protected readonly SignInManager<User> _signInManager;
		protected readonly RoleManager<Role> _roleManager;
		protected readonly UserManager<User> _userManager;
		protected readonly IOtpService _otpService;
		protected readonly IEmailService _emailService;
		protected readonly ITokenService _tokenService;
		protected readonly ICachedService _cacheService;
		protected readonly INotificationService _notificationService;
		protected readonly IAuditLogService _auditLogService;
		#endregion

		#region Ctor
		protected BaseAccountService(
			IMapper mapper,
			SignInManager<User> signInManager,
			RoleManager<Role> roleManager,
			UserManager<User> userManager,
			IOtpService otpService,
			IEmailService emailService,
			ITokenService tokenService,
			ICachedService cacheService,
			INotificationService notificationService,
			IAuditLogService auditLogService)
		{
			_mapper = mapper;
			_signInManager = signInManager;
			_roleManager = roleManager;
			_userManager = userManager;
			_otpService = otpService;
			_emailService = emailService;
			_tokenService = tokenService;
			_cacheService = cacheService;
			_notificationService = notificationService;
			_auditLogService = auditLogService;
		}
		#endregion

		#region Helpers
		protected string GetPendingKey(string email)
			=> $"PENDING_USER:{email}";

		protected void FireAndForgetAudit(string userEmail, AuditAction action, string details)
		{
			Task.Run(() => _auditLogService.Log(userEmail, action, details));
		}

		private static readonly Regex EmailRegex =
			new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);
		private static readonly Regex PhoneRegex =
			new(@"^01[0-2,5]{1}[0-9]{8}$", RegexOptions.Compiled);
		private static readonly Regex PasswordRegex =
			new(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&#])[A-Za-z\d@$!%*?&#]{8,}$", RegexOptions.Compiled);
		#endregion

		#region Validation Methods
		protected (bool IsValid, string ErrorMessage) ValidateEmail(string email)
		{
			if (string.IsNullOrWhiteSpace(email))
				return (false, "Email is required");

			if (!EmailRegex.IsMatch(email))
				return (false, "Invalid email format");

			return (true, string.Empty);
		}

		protected (bool IsValid, string ErrorMessage) ValidatePhone(string phone)
		{
			if (string.IsNullOrWhiteSpace(phone))
				return (false, "Phone number is required");

			if (!PhoneRegex.IsMatch(phone))
				return (false, "Invalid phone number format");

			return (true, string.Empty);
		}

		protected (bool IsValid, string ErrorMessage) ValidatePassword(string password)
		{
			if (string.IsNullOrWhiteSpace(password))
				return (false, "Password is required");

			if (!PasswordRegex.IsMatch(password))
				return (false, "Password must be at least 8 characters and contain uppercase, lowercase, number and special character");

			return (true, string.Empty);
		}
		#endregion

		#region Login
		protected async Task<ServiceResult<AuthTokenResultDto>> ProcessLoginAsync(LoginDto dto, string expectedRole)
		{
			var user = await _userManager.FindByEmailAsync(dto.Email);
			if (user == null)
				return ServiceResult<AuthTokenResultDto>.Fail("User not found");

			var roles = await _userManager.GetRolesAsync(user);
			if (!roles.Contains(expectedRole))
				return ServiceResult<AuthTokenResultDto>.Fail("Access denied");

			if (expectedRole == BuyerRole && !user.EmailConfirmed)
				return ServiceResult<AuthTokenResultDto>.Fail("Email not verified");

			var signInResult = await _signInManager.CheckPasswordSignInAsync(
				user,
				dto.Password,
				lockoutOnFailure: true
			);

			if (!signInResult.Succeeded)
				return await HandleFailedLogin(user, expectedRole == BuyerRole);

			if (user.Status != AccountStatus.active)
				return ServiceResult<AuthTokenResultDto>.Fail("Account not active");

			var tokens = await _tokenService.GenerateTokens(user);

			FireAndForgetAudit(user.Email, AuditAction.LOGIN_SUCCESS, "Login success");

			return ServiceResult<AuthTokenResultDto>.Ok(tokens, "Login successful");
		}

		private async Task<ServiceResult<AuthTokenResultDto>> HandleFailedLogin(User user, bool isBuyer)
		{
			FireAndForgetAudit(user.Email, AuditAction.LOGIN_FAILED, "Invalid password");

			if (isBuyer && await _userManager.GetAccessFailedCountAsync(user) >= MaxLoginAttempts)
			{
				await _userManager.SetLockoutEndDateAsync(
					user,
					DateTimeOffset.UtcNow.AddMinutes(LockoutMinutes)
				);

				await GenerateAndSendOtpAsync(user);

				return ServiceResult<AuthTokenResultDto>.Fail("Too many attempts, OTP sent");
			}

			return ServiceResult<AuthTokenResultDto>.Fail("Invalid credentials");
		}
		#endregion

		#region OTP Management
		protected async Task GenerateAndSendOtpAsync(User user)
		{
			if (!await _otpService.CanResendOtpAsync(user.Email, user.Id))
				return;

			var (otp, nonce) = await _otpService.GenerateOtpAsync(user.Email, user.Id);

			var model = new OtpEmailModel
			{
				Email = user.Email,
				Name = user.FullName,
				OtpCode = otp,
				Nonce = nonce,
				OtpExpiryMinutes = OtpExpiryMinutes
			};

			var (subject, body) = EmailTemplates.GetOtpVerificationEmail(model);
			await _emailService.SendEmailAsync(user.Email, subject, body);

			FireAndForgetAudit(user.Email, AuditAction.OTP_SENT, $"OTP sent nonce:{nonce}");
		}

		protected async Task<ServiceResult<AuthTokenResultDto>> ProcessEmailVerificationAsync(string email, string otp, string nonce)
		{
			var user = await _userManager.FindByEmailAsync(email);
			if (user == null)
				return ServiceResult<AuthTokenResultDto>.Fail("User not found");

			var result = await _otpService.ValidateOtpAsync(user, otp, nonce);

			return result switch
			{
				OtpValidationResult.Success => await CompleteVerificationAsync(user),
				OtpValidationResult.Expired => ServiceResult<AuthTokenResultDto>.Fail("OTP expired"),
				OtpValidationResult.Locked => ServiceResult<AuthTokenResultDto>.Fail("Account locked"),
				_ => ServiceResult<AuthTokenResultDto>.Fail("Invalid OTP")
			};
		}

		private async Task<ServiceResult<AuthTokenResultDto>> CompleteVerificationAsync(User user)
		{
			user.EmailConfirmed = true;
			user.EmailVerifiedAt = DateTime.UtcNow;

			var role = await _userManager.GetRolesAsync(user);
			if (role.FirstOrDefault() != SellerRole)
				user.Status = AccountStatus.active;

			await _userManager.UpdateAsync(user);

			var tokens = await _tokenService.GenerateTokens(user);

			return ServiceResult<AuthTokenResultDto>.Ok(tokens, "Email verified");
		}
		#endregion

		#region Registration
		protected async Task<ServiceResult> RegisterPendingAsync(PendingRegistration pending)
		{
			// Validation
			var emailValidation = ValidateEmail(pending.Email);
			if (!emailValidation.IsValid)
				return ServiceResult.Fail(emailValidation.ErrorMessage);

			var phoneValidation = ValidatePhone(pending.PhoneNumber);
			if (!phoneValidation.IsValid)
				return ServiceResult.Fail(phoneValidation.ErrorMessage);

			var passwordValidation = ValidatePassword(pending.Password);
			if (!passwordValidation.IsValid)
				return ServiceResult.Fail(passwordValidation.ErrorMessage);

			pending.Email = pending.Email.ToLower().Trim();

			// Existence check
			if (await _userManager.FindByEmailAsync(pending.Email) != null)
				return ServiceResult.Fail("User already exists");

			pending.PhoneNumber = pending.PhoneNumber.Trim();
			var phoneExists = await _userManager.Users
				.AnyAsync(u => u.PhoneNumber == pending.PhoneNumber);

			if (phoneExists)
				return ServiceResult.Fail("Phone number already exists");

			var key = GetPendingKey(pending.Email);
			if (await _cacheService.GetCacheResponseAsync(key) != null)
				return ServiceResult.Fail("Verification already sent");

			// Cache pending registration
			pending.CreatedAt = DateTime.UtcNow;
			var json = JsonSerializer.Serialize(
				pending,
				new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }
			);

			await _cacheService.SetCacheResponseWithNoSerializAsync(
				key,
				json,
				TimeSpan.FromHours(3)
			);

			// Send OTP
			await GenerateAndSendRegistrationOtpAsync(pending.Email);

			FireAndForgetAudit(
				pending.Email,
				AuditAction.REGISTRATION_STARTED,
				$"Pending registration for {pending.Email}"
			);

			return ServiceResult.Ok("Registration successful, verify your email");
		}

		protected async Task<ServiceResult> ResendRegistrationOtpAsync(string email)
		{
			var key = GetPendingKey(email);
			if (await _cacheService.GetCacheResponseAsync(key) == null)
				return ServiceResult.Fail("No pending registration found");

			await GenerateAndSendRegistrationOtpAsync(email);

			FireAndForgetAudit(email, AuditAction.OTP_RESENT, $"OTP resent {email}");

			return ServiceResult.Ok("OTP resent successfully");
		}

		protected async Task<ServiceResult<AuthTokenResultDto>> VerifyRegistrationOtpAsync(string email, string otp, string nonce)
		{
			var key = GetPendingKey(email);
			var cached = await _cacheService.GetCacheResponseAsync(key);

			if (string.IsNullOrWhiteSpace(cached))
				return ServiceResult<AuthTokenResultDto>.Fail("Verification expired");

			var pending = JsonSerializer.Deserialize<PendingRegistration>(cached,
				new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

			if (pending == null || string.IsNullOrWhiteSpace(pending.Email))
				return ServiceResult<AuthTokenResultDto>.Fail("Corrupted registration data");

			var otpResult = await _otpService.ValidateRegistrationOtpAsync(email, otp, nonce);
			if (otpResult != OtpValidationResult.Success)
				return ServiceResult<AuthTokenResultDto>.Fail("Invalid OTP");

			var user = new User
			{
				Email = pending.Email,
				UserName = pending.Email,
				FullName = pending.FullName,
				PhoneNumber = pending.PhoneNumber,
				EmailConfirmed = true,
				EmailVerifiedAt = DateTime.UtcNow,
				Status = AccountStatus.active,
				CreatedAt = DateTime.UtcNow
			};

			var result = await _userManager.CreateAsync(user, pending.Password);
			if (!result.Succeeded)
				return ServiceResult<AuthTokenResultDto>.Fail(
					string.Join(", ", result.Errors.Select(e => e.Description))
				);

			await _userManager.AddToRoleAsync(user, pending.Role);
			await _cacheService.RemoveCacheAsync(key);

			FireAndForgetAudit(user.Email, AuditAction.EMAIL_VERIFIED, "Verified & registered");

			// Send appropriate emails
			await SendRegistrationEmailsAsync(user, pending.Role);

			var tokens = await _tokenService.GenerateTokens(user);
			return ServiceResult<AuthTokenResultDto>.Ok(tokens, "Email verified & registration completed");
		}

		private async Task SendRegistrationEmailsAsync(User user, string role)
		{
			if (role == BuyerRole)
			{
				var successModel = new EmailModel
				{
					Email = user.Email,
					Name = user.FullName
				};
				var (subject1, body1) = EmailTemplates.GetOtpVerificationSuccessEmail(successModel);
				await _emailService.SendEmailAsync(user.Email, subject1, body1);

				var welcomeModel = new WelcomeEmailModel
				{
					Email = user.Email,
					Name = user.FullName
				};
				var (subject2, body2) = EmailTemplates.GetWelcomeEmail(welcomeModel);
				await _emailService.SendEmailAsync(user.Email, subject2, body2);
			}
			else
			{
				var successModel = new EmailModel
				{
					Email = user.Email,
					Name = user.FullName
				};
				var (subject, body) = EmailTemplates.GetOtpVerificationSuccessSellerEmail(successModel);
				await _emailService.SendEmailAsync(user.Email, subject, body);
			}
		}

		private async Task GenerateAndSendRegistrationOtpAsync(string email)
		{
			var (otp, nonce) = await _otpService.GenerateRegistrationOtpAsync(email);
			var model = new OtpEmailModel
			{
				Email = email,
				OtpCode = otp,
				Nonce = nonce,
				OtpExpiryMinutes = OtpExpiryMinutes
			};

			var (subject, body) = EmailTemplates.GetOtpVerificationEmail(model);
			await _emailService.SendEmailAsync(email, subject, body);
		}
		#endregion

		#region Models
		protected class PendingRegistration
		{
			public string Email { get; set; }
			public string FullName { get; set; }
			public string PhoneNumber { get; set; }
			public string Password { get; set; }
			public string Role { get; set; }
			public DateTime CreatedAt { get; set; }
		}
		#endregion
	}
}