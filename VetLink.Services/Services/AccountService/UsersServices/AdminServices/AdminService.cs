using AutoMapper;
using Microsoft.AspNetCore.Identity;
using VetLink.Data.Entities;
using VetLink.Data.Enums;
using VetLink.Repository.Interfaces;
using VetLink.Repository.Specifications.Paginated;
using VetLink.Repository.Specifications.SellerSpecifications;
using VetLink.Repository.Specifications.UserSpecifications;
using VetLink.Services.Helper;
using VetLink.Services.Services.AccountService.AuditLogService;
using VetLink.Services.Services.AccountService.Dtos;
using VetLink.Services.Services.AccountService.Dtos.AdminDtos;
using VetLink.Services.Services.AccountService.Dtos.AuthDtos;
using VetLink.Services.Services.AccountService.Dtos.BuyerDtos;
using VetLink.Services.Services.AccountService.Dtos.SellerDtos;
using VetLink.Services.Services.AccountService.MailService;
using VetLink.Services.Services.AccountService.MailService.Dtos;
using VetLink.Services.Services.AccountService.OTPService;
using VetLink.Services.Services.AccountService.TokenService;
using VetLink.Services.Services.AccountService.TokenService.Dtos;
using VetLink.Services.Services.AccountService.UsersServices.AdminService;
using VetLink.Services.Services.CachedService;
using VetLink.Services.Services.Email;
using VetLink.Services.Services.Notifications;

namespace VetLink.Services.Services.AccountService.UsersServices.AdminServices
{
    public class AdminService : BaseAccountService, IAdminService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AdminService(
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
            : base(mapper, signInManager, roleManager, userManager, otpService, emailService,
                  tokenService, cacheService, notificationService, auditLogService)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ServiceResult<AuthTokenResultDto>> SignInAsync(LoginDto dto)
        {
            return await ProcessLoginAsync(dto, AdminRole);
        }

        public async Task<ServiceResult> ApproveSellerAsync(int sellerUserId, int approverId)
        {
            var adminValidation = await ValidateAdminApprover(approverId);
            if (!adminValidation.IsValid)
                return ServiceResult.Fail(adminValidation.ErrorMessage);

            var sellerValidation = await ValidateSellerUser(sellerUserId);
            if (!sellerValidation.IsValid)
                return ServiceResult.Fail(sellerValidation.ErrorMessage);

            var user = sellerValidation.User!;

            if (!user.EmailConfirmed)
                return ServiceResult.Fail("Email not verified");

            if (user.Status == AccountStatus.active)
                return ServiceResult.Fail("Seller already approved");

            var seller = await _unitOfWork.Repository<Seller, int>().GetByIdAsync(user.Id);
            if (seller == null)
                return ServiceResult.Fail("Seller not found");

            using var transaction = await _unitOfWork.BeginTransactionAsync();

            user.Status = AccountStatus.active;
            user.UpdatedAt = DateTime.UtcNow;
            seller.ApprovedAt = DateTime.UtcNow;

            await _userManager.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();
            await transaction.CommitAsync();

            await SendSellerApprovalEmail(user.Email);
            FireAndForgetAudit(user.Email, AuditAction.SELLER_APPROVED,
                $"Seller {sellerUserId} approved by admin {approverId}");

            return ServiceResult.Ok("Seller approved successfully");
        }

        public async Task<ServiceResult> RejectSellerAsync(RejectSellerDto dto, int approverId)
        {
            var adminValidation = await ValidateAdminApprover(approverId);
            if (!adminValidation.IsValid)
                return ServiceResult.Fail(adminValidation.ErrorMessage);

            var user = await _userManager.FindByIdAsync(dto.SellerUserId.ToString());
            if (user == null)
                return ServiceResult.Fail("Seller user not found");

            if (!await _userManager.IsInRoleAsync(user, SellerRole))
                return ServiceResult.Fail("User is not a seller");

            if (!user.EmailConfirmed)
                return ServiceResult.Fail("Seller email not verified");

            if (user.Status != AccountStatus.pending_approval)
                return ServiceResult.Fail("Seller is not pending approval");

            user.Status = AccountStatus.rejected;
            user.RejectionReason = dto.Reason;
            user.UpdatedAt = DateTime.UtcNow;

            await _userManager.UpdateAsync(user);
            await SendSellerRejectionEmail(user.Email, dto.Reason);

            FireAndForgetAudit(user.Email, AuditAction.SELLER_REJECTED,
                $"Seller {dto.SellerUserId} rejected by admin {approverId}. Reason: {dto.Reason}");

            return ServiceResult.Ok("Seller rejected successfully");
        }

        public async Task<ServiceResult<PaginatedResultDto<PendingSellerDto>>> GetPendingSellersAsync(
            PaginationSpecification pagination)
        {
            var sellersRepo = _unitOfWork.Repository<Seller, int>();
            var specs = new SellerWithSpecification(AccountStatus.pending_approval, pagination, null);

            var sellers = await sellersRepo.ListAllWithSpecAsync(specs);
            var total = await sellersRepo.CountWithSpecAsync(specs);
            var totalPages = (int)Math.Ceiling(total / (double)pagination.PageSize);

            var result = sellers.Select(s => new PendingSellerDto
            {
                UserId = s.UserId,
                Email = s.User.Email,
                FullName = s.User.FullName,
                StoreName = s.StoreName,
                CreatedAt = s.User.CreatedAt
            }).ToList();

            var paging = new PaginatedResultDto<PendingSellerDto>(
                result, pagination.PageIndex, pagination.PageSize, total, totalPages);

            return ServiceResult<PaginatedResultDto<PendingSellerDto>>.Ok(
                paging, "Pending sellers retrieved successfully");
        }

        public async Task<ServiceResult<PaginatedResultDto<SellerProfileDto>>> GetAllSellersAsync(
            AccountStatus? status, string? search, PaginationSpecification pagination)
        {
            var specs = new SellerWithSpecification(status, pagination, search);
            var repo = _unitOfWork.Repository<Seller, int>();

            var sellers = await repo.ListAllWithSpecAsync(specs);
            var total = await repo.CountWithSpecAsync(specs);
            var totalPages = (int)Math.Ceiling(total / (double)pagination.PageSize);

            var mapped = _mapper.Map<IReadOnlyList<SellerProfileDto>>(sellers);
            var paging = new PaginatedResultDto<SellerProfileDto>(
                mapped, pagination.PageIndex, pagination.PageSize, total, totalPages);

            return ServiceResult<PaginatedResultDto<SellerProfileDto>>.Ok(
                paging, "Sellers fetched successfully");
        }

        public async Task<ServiceResult<PaginatedResultDto<BuyerDto>>> GetAllBuyersAsync(
            string? search, PaginationSpecification pagination)
        {
            var specs = new UserWithSpecification(pagination, search);
            var repo = _unitOfWork.Repository<User, int>();

            var users = await repo.ListAllWithSpecAsync(specs);
            var buyers = new List<User>();

            foreach (var user in users)
            {
                if (await _userManager.IsInRoleAsync(user, BuyerRole))
                {
                    buyers.Add(user);
                }
            }

            var countSpecs = new UserWithSpecification(pagination, search ?? string.Empty);
            var total = await repo.CountWithSpecAsync(countSpecs);
            var totalPages = (int)Math.Ceiling(total / (double)pagination.PageSize);

            var mapped = _mapper.Map<List<BuyerDto>>(buyers);
            var paging = new PaginatedResultDto<BuyerDto>(
                mapped, pagination.PageIndex, pagination.PageSize, total, totalPages);

            return ServiceResult<PaginatedResultDto<BuyerDto>>.Ok(
                paging, "Buyers fetched successfully");
        }

        #region Unimplemented Methods (Placeholders)
        public Task<ServiceResult> ActivateUserAsync(int userId)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResult> DeactivateUserAsync(int userId)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResult> ResetUserPasswordAsync(int userId)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResult<AdminDashboardDto>> GetDashboardAsync()
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResult<PaginatedResultDto<AuditLogDto>>> GetAuditLogsAsync(
            DateTime? fromDate, DateTime? toDate, string? action, int? userId, int page, int pageSize)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResult<StatisticsDto>> GetStatisticsAsync(DateTime? startDate, DateTime? endDate)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResult> SendBulkEmailAsync(BulkEmailDto dto)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResult<AdminProfileDto>> GetProfileAsync(int adminUserId)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResult<AdminProfileDto>> UpdateProfileAsync(int adminUserId, UpdateAdminProfileDto dto)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResult<AdminProfileDto>> CreateAdminAsync(CreateAdminDto dto)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Private Methods
        private async Task<(bool IsValid, string ErrorMessage, User? Admin)> ValidateAdminApprover(int approverId)
        {
            var admin = await _userManager.FindByIdAsync(approverId.ToString());

            if (admin == null || !await _userManager.IsInRoleAsync(admin, AdminRole))
            {
                return (false, "Only admins can approve sellers", null);
            }

            return (true, string.Empty, admin);
        }

        private async Task<(bool IsValid, string ErrorMessage, User? User)> ValidateSellerUser(int sellerUserId)
        {
            var user = await _userManager.FindByIdAsync(sellerUserId.ToString());

            if (user == null)
            {
                return (false, "Seller user not found", null);
            }

            if (!await _userManager.IsInRoleAsync(user, SellerRole))
            {
                return (false, "User is not a seller", null);
            }

            return (true, string.Empty, user);
        }

        private async Task SendSellerApprovalEmail(string email)
        {
            try
            {
                var model = new EmailModel
                {
                    Email = email,
                    Name = email.Split('@')[0]
                };

                var (subject, body) = EmailTemplates.GetSellerApprovalEmail(model);
                await _emailService.SendEmailAsync(email, subject, body);
            }
            catch (Exception ex)
            {
                // Log the error but don't throw - email failure shouldn't fail the whole operation
                FireAndForgetAudit(email, AuditAction.EMAIL_SEND_FAILED,
                    $"Failed to send approval email: {ex.Message}");
            }
        }

        private async Task SendSellerRejectionEmail(string email, string reason)
        {
            try
            {
                var model = new SellerRejectionEmailModel
                {
                    Email = email,
                    Name = email.Split('@')[0],
                    Reason = reason
                };

                var (subject, body) = EmailTemplates.GetSellerRejectionEmail(model);
                await _emailService.SendEmailAsync(email, subject, body);
            }
            catch (Exception ex)
            {
                // Log the error but don't throw
                FireAndForgetAudit(email, AuditAction.EMAIL_SEND_FAILED,
                    $"Failed to send rejection email: {ex.Message}");
            }
        }
        #endregion
    }
}