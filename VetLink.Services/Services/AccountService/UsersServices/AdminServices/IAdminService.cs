using VetLink.Data.Enums;
using VetLink.Repository.Specifications.Paginated;
using VetLink.Services.Helper;
using VetLink.Services.Services.AccountService.Dtos;
using VetLink.Services.Services.AccountService.Dtos.AdminDtos;
using VetLink.Services.Services.AccountService.Dtos.AuthDtos;
using VetLink.Services.Services.AccountService.Dtos.BuyerDtos;
using VetLink.Services.Services.AccountService.Dtos.SellerDtos;
using VetLink.Services.Services.AccountService.TokenService.Dtos;

namespace VetLink.Services.Services.AccountService.UsersServices.AdminService
{
	public interface IAdminService
	{
		Task<ServiceResult<AuthTokenResultDto>> SignInAsync(LoginDto dto);
		Task<ServiceResult> ApproveSellerAsync(int sellerUserId, int approverId);
		Task<ServiceResult> RejectSellerAsync(RejectSellerDto dto, int approverId);
		Task<ServiceResult<PaginatedResultDto<PendingSellerDto>>> GetPendingSellersAsync(
			PaginationSpecification pagination);
		Task<ServiceResult<PaginatedResultDto<SellerProfileDto>>> GetAllSellersAsync(
			AccountStatus? status, string? search, PaginationSpecification pagination);
		Task<ServiceResult<PaginatedResultDto<BuyerDto>>> GetAllBuyersAsync(
			string? search, PaginationSpecification pagination);
		Task<ServiceResult> ActivateUserAsync(int userId);
		Task<ServiceResult> DeactivateUserAsync(int userId);
		Task<ServiceResult> ResetUserPasswordAsync(int userId);
		Task<ServiceResult<AdminDashboardDto>> GetDashboardAsync();
		Task<ServiceResult<PaginatedResultDto<AuditLogDto>>> GetAuditLogsAsync(
			DateTime? fromDate, DateTime? toDate, string? action, int? userId, int page, int pageSize);
		Task<ServiceResult<StatisticsDto>> GetStatisticsAsync(DateTime? startDate, DateTime? endDate);
		Task<ServiceResult> SendBulkEmailAsync(BulkEmailDto dto);
		Task<ServiceResult<AdminProfileDto>> GetProfileAsync(int adminUserId);
		Task<ServiceResult<AdminProfileDto>> UpdateProfileAsync(int adminUserId, UpdateAdminProfileDto dto);
		Task<ServiceResult<AdminProfileDto>> CreateAdminAsync(CreateAdminDto dto);
	}
}