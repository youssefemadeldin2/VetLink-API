using System.Net;
using System.Text.Json;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using VetLink.Data.Entities;
using VetLink.Data.Enums;
using VetLink.Repository.Interfaces;
using VetLink.Repository.Specifications.Paginated;
using VetLink.Services.Helper;
using VetLink.Services.Services.AccountService.AuditLogService;
using VetLink.Services.Services.AccountService.Dtos.AuthDtos;
using VetLink.Services.Services.AccountService.Dtos.BuyerDtos;
using VetLink.Services.Services.AccountService.Dtos.SellerDtos;
using VetLink.Services.Services.AccountService.MailService;
using VetLink.Services.Services.AccountService.MailService.Dtos;
using VetLink.Services.Services.AccountService.OTPService;
using VetLink.Services.Services.AccountService.TokenService;
using VetLink.Services.Services.AccountService.TokenService.Dtos;
using VetLink.Services.Services.CachedService;
using VetLink.Services.Services.Email;
using VetLink.Services.Services.Notifications;
using VetLink.Services.Services.OrderService.Dtos;

namespace VetLink.Services.Services.AccountService.UsersServices.BuyerServices
{
    public class BuyerService : BaseAccountService, IBuyerService
    {
        private readonly IUnitOfWork _unitOfWork;

        public BuyerService(
            IMapper mapper,
            SignInManager<User> signInManager,
            RoleManager<Role> roleManager,
            UserManager<User> userManager,
            IOtpService otpService,
            IEmailService emailService,
            ITokenService tokenService,
            ICachedService cacheService,
            INotificationService notificationService,
            IUnitOfWork unitOfWork,
            IAuditLogService auditLogService)
            : base(mapper, signInManager, roleManager, userManager, otpService, emailService,
                  tokenService, cacheService, notificationService, auditLogService)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ServiceResult> RegisterAsync(RegisterAsBuyerDto dto)
        {
            var pending = new PendingRegistration
            {
                Email = dto.Email,
                FullName = dto.FullName,
                PhoneNumber = dto.PhoneNumber,
                Password = dto.Password,
                Role = "Buyer"
            };
            return await RegisterPendingAsync(pending);
        }

        public async Task<ServiceResult<AuthTokenResultDto>> VerifyEmailOtpAsync(string email, string otp, string nonce)
        {
            return await VerifyRegistrationOtpAsync(email, otp, nonce);
        }

        public async Task<ServiceResult<AuthTokenResultDto>> SignInAsync(LoginDto dto)
        {
            return await ProcessLoginAsync(dto, BuyerRole);
        }

        public async Task<ServiceResult> ReSendOTPAsync(ResendOtpDto dto)
        {
            var email = dto.Email.ToLower();

            var existingUser = await _userManager.FindByEmailAsync(dto.Email);
            if (existingUser != null && existingUser.EmailConfirmed)
                return ServiceResult.Fail("Email already verified", StatusCodes.Status409Conflict);

            var pendingKey = $"PENDING_USER:{email}";
            var cachedJson = await _cacheService.GetCacheResponseAsync(pendingKey);
            if (cachedJson == null)
                return ServiceResult.Fail("No pending registration found", StatusCodes.Status404NotFound);

            var pendingUser = JsonSerializer.Deserialize<PendingRegistration>(cachedJson);
            if (pendingUser == null)
                return ServiceResult.Fail("Invalid pending data", StatusCodes.Status500InternalServerError);

            var counterKey = $"OTP:REGISTER:COUNT:{email}";
            var count = await _cacheService.IncrementCounterAsync(counterKey, TimeSpan.FromHours(1));
            if (count > 2)
                return ServiceResult.Fail("Too many requests, try later", StatusCodes.Status429TooManyRequests);

            await _otpService.ClearOtpAsync(email);
            FireAndForgetAudit(email, AuditAction.OTP_SENT, $"OTP resent for {email}");

            var result = await ResendRegistrationOtpAsync(email);
            return ServiceResult.Ok("OTP resent successfully");
        }

        public async Task<ServiceResult> ForgotPasswordAsync(ForgotPasswordDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return ServiceResult.Fail("User not found", StatusCodes.Status404NotFound);

            await GenerateAndSendOtpAsync(user);
            return ServiceResult.Ok("OTP sent");
        }

        public async Task<ServiceResult> ResetPasswordAsync(ResetPasswordDto dto)
        {
            var emailValidation = ValidateEmail(dto.Email);
            if (!emailValidation.IsValid)
                return ServiceResult.Fail(emailValidation.ErrorMessage, (int)HttpStatusCode.BadRequest);

            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return ServiceResult.Fail("User not found", StatusCodes.Status404NotFound);

            var otpResult = await _otpService.ValidateOtpAsync(user, dto.Otp, dto.Nonce);
            if (otpResult != OtpValidationResult.Success)
                return ServiceResult.Fail("Invalid OTP", StatusCodes.Status400BadRequest);

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            await _userManager.ResetPasswordAsync(user, token, dto.NewPassword);

            var model = new EmailModel
            {
                Email = user.Email!,
                Name = user.FullName
            };

            var (subject, body) = EmailTemplates.GetPasswordResetSuccessEmail(model);
            await _emailService.SendEmailAsync(user.Email!, subject, body);

            return ServiceResult.Ok("Password reset successful");
        }

        public async Task<ServiceResult<BuyerProfileDto>> UpdateProfileAsync(int userId, UpdateBuyerProfileDto dto)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return ServiceResult<BuyerProfileDto>.Fail("User not found", (int)HttpStatusCode.NotFound);

            _mapper.Map(dto, user);

            await _userManager.UpdateAsync(user);

            var profileDto = _mapper.Map<BuyerProfileDto>(user);

            return ServiceResult<BuyerProfileDto>.Ok(profileDto, "Profile updated");
        }

        public async Task<ServiceResult<BuyerProfileDto>> GetProfileAsync(int userId, PaginationSpecification pagination)
        {
            var identityUser = await _userManager.FindByIdAsync(userId.ToString());
            if (identityUser == null)
                return ServiceResult<BuyerProfileDto>.Fail("User not found", (int)HttpStatusCode.NotFound);

            var buyer = await _unitOfWork
                .Repository<User, int>()
                .GetByIdAsync(userId);

            if (buyer == null)
                return ServiceResult<BuyerProfileDto>.Fail("Buyer not found", (int)HttpStatusCode.NotFound);

            var ordersSpec = new BuyerOrdersSpecification(userId, pagination);
            var orders = await _unitOfWork.Repository<Order, int>().ListAllWithSpecAsync(ordersSpec);

            var totalItems = await _unitOfWork.Repository<Order, int>()
                .CountWithSpecAsync(ordersSpec);

            var orderDtos = _mapper.Map<List<BuyerOrderDto>>(orders);

            var paginatedOrders = new PaginatedResultDto<BuyerOrderDto>(
                orderDtos,
                pagination.PageIndex,
                pagination.PageSize,
                totalItems,
                (int)Math.Ceiling(totalItems / (double)pagination.PageSize)
            );

            var dto = _mapper.Map<BuyerProfileDto>(buyer);
            dto.Orders = paginatedOrders;

            return ServiceResult<BuyerProfileDto>.Ok(dto, "Profile loaded");
        }

        public async Task<ServiceResult> ChangePasswordAsync(int userId, ChangePasswordDto dto)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return ServiceResult.Fail("User not found", (int)HttpStatusCode.NotFound);

            var result = await _userManager.ChangePasswordAsync(
                user,
                dto.CurrentPassword,
                dto.NewPassword);

            if (!result.Succeeded)
                return ServiceResult.Fail("Invalid password", 400);

            return ServiceResult.Ok("Password changed");
        }

        public async Task<ServiceResult> LogoutAsync(int userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return ServiceResult.Fail("User not found", (int)HttpStatusCode.NotFound);

            await _tokenService.InvalidateTokens(user);

            return ServiceResult.Ok("Logged out successfully");
        }

        public async Task<ServiceResult<AuthTokenResultDto>> RefreshTokenAsync(RefreshTokenDto dto)
        {
            var tokens = await _tokenService.RefreshTokens(dto.RefreshToken);
            if (tokens == null)
                return ServiceResult<AuthTokenResultDto>.Fail("Invalid refresh token", (int)HttpStatusCode.Unauthorized);

            return ServiceResult<AuthTokenResultDto>.Ok(tokens, "Token refreshed");
        }
    }
}