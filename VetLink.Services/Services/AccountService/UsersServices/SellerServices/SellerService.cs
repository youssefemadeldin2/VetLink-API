using System.Net;
using System.Text.Json;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using VetLink.Data.Entities;
using VetLink.Data.Enums;
using VetLink.Repository.Interfaces;
using VetLink.Services.Helper;
using VetLink.Services.Services.AccountService.AuditLogService;
using VetLink.Services.Services.AccountService.Dtos.AuthDtos;
using VetLink.Services.Services.AccountService.Dtos.SellerDtos;
using VetLink.Services.Services.AccountService.MailService;
using VetLink.Services.Services.AccountService.MailService.Dtos;
using VetLink.Services.Services.AccountService.OTPService;
using VetLink.Services.Services.AccountService.TokenService;
using VetLink.Services.Services.AccountService.TokenService.Dtos;
using VetLink.Services.Services.CachedService;
using VetLink.Services.Services.Email;
using VetLink.Services.Services.Notifications;

namespace VetLink.Services.Services.AccountService.UsersServices.SellerServices
{
    public class SellerService : BaseAccountService, ISellerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public SellerService(
            IUnitOfWork unitOfWork,
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
            : base(mapper, signInManager, roleManager, userManager, otpService,
                  emailService, tokenService, cacheService, notificationService, auditLogService)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ServiceResult> RegisterAsync(RegisterAsSellerDto dto)
        {
            if (await _userManager.FindByEmailAsync(dto.Email) != null)
                return ServiceResult.Fail("User already exists", 409);

            var pendingKey = GetPendingKey(dto.Email);

            if (await _cacheService.GetCacheResponseAsync(pendingKey) != null)
                return ServiceResult.Fail("OTP already sent", 409);

            var pending = new PendingRegistration
            {
                Email = dto.Email,
                FullName = dto.FullName,
                PhoneNumber = dto.PhoneNumber,
                Password = dto.Password,
                Role = SellerRole,
                ExtraData = JsonSerializer.Serialize(dto),
                CreatedAt = DateTime.UtcNow
            };

            await _cacheService.SetCacheResponseWithNoSerializAsync(
                pendingKey,
                JsonSerializer.Serialize(pending, _jsonOptions),
                TimeSpan.FromHours(1)
            );

            var (otp, nonce) = await _otpService.GenerateRegistrationOtpAsync(dto.Email);

            var model = new OtpEmailModel
            {
                Email = dto.Email,
                OtpCode = otp,
                Nonce = nonce,
                Name = dto.FullName
            };

            var (subject, body) = EmailTemplates.GetOtpVerificationEmail(model);
            await _emailService.SendEmailAsync(dto.Email, subject, body);

            return ServiceResult.Ok("OTP sent successfully");
        }

        public async Task<ServiceResult> VerifyEmailOtpAsync(string email, string otp, string nonce)
        {
            var pendingKey = GetPendingKey(email);
            var cached = await _cacheService.GetCacheResponseAsync(pendingKey);

            if (string.IsNullOrEmpty(cached))
                return ServiceResult.Fail("OTP expired", 410);

            var pending = JsonSerializer.Deserialize<PendingRegistration>(
                cached,
                _jsonOptions);

            if (pending == null || string.IsNullOrWhiteSpace(pending.Email))
            {
                return ServiceResult.Fail(
                    "Invalid pending registration data (email missing)",
                    500
                );
            }

            var otpResult = await _otpService.ValidateRegistrationOtpAsync(email, otp, nonce);
            if (otpResult != OtpValidationResult.Success)
                return ServiceResult.Fail("Invalid OTP", 400);

            var user = new User
            {
                Email = pending.Email,
                UserName = pending.Email,
                FullName = pending.FullName,
                PhoneNumber = pending.PhoneNumber,
                Status = AccountStatus.pending_approval,
                EmailConfirmed = true,
                EmailVerifiedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            var createUser = await _userManager.CreateAsync(user, pending.Password);
            if (!createUser.Succeeded)
                return HandleErrors(createUser);

            await _userManager.AddToRoleAsync(user, SellerRole);

            var sellerDto = JsonSerializer.Deserialize<RegisterAsSellerDto>(pending.ExtraData, _jsonOptions);
            if (sellerDto == null)
                return ServiceResult.Fail("Invalid seller data", 500);

            var seller = CreateSeller(user.Id, sellerDto);

            await _unitOfWork.Repository<Seller, int>().AddAsync(seller);
            await _unitOfWork.SaveChangesAsync();

            await _cacheService.RemoveCacheAsync(pendingKey);

            var (subject, body) = EmailTemplates.GetOtpVerificationSuccessSellerEmail(
                new EmailModel { Email = email, Name = user.FullName }
            );
            await _emailService.SendEmailAsync(email, subject, body);

            FireAndForgetAudit(user.Email, AuditAction.EMAIL_VERIFIED, "Seller email verified");

            return ServiceResult.Ok(
                "Email verified successfully. Waiting for admin approval"
            );
        }

        public async Task<ServiceResult<AuthTokenResultDto>> SignInAsync(LoginDto dto)
        {
            return await ProcessLoginAsync(dto, SellerRole);
        }

        private static Seller CreateSeller(int userId, RegisterAsSellerDto dto)
        {
            return new Seller
            {
                UserId = userId,
                StoreName = dto.StoreName,
                StoreDescription = dto.StoreDescription,
                CommercialRegisterURL = dto.CommercialRegisterURL,
                PracticeLicenseURL = dto.PracticeLicenseURL,
                StoreLogoURL = dto.StoreLogoURL ?? string.Empty
            };
        }

        private static ServiceResult HandleErrors(IdentityResult result)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return ServiceResult.Fail(errors, (int)HttpStatusCode.BadRequest);
        }

        // Extended PendingRegistration class for Seller
        protected new class PendingRegistration : BaseAccountService.PendingRegistration
        {
            public string ExtraData { get; set; } = string.Empty;
        }
    }
}