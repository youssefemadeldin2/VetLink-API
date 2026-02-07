using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using VetLink.WebApi.Helpers;
using VetLink.Data.Enums;
using VetLink.Repository.Specifications.Paginated;
using VetLink.Services.Helper;
using VetLink.Services.Services.AccountService.Dtos;
using VetLink.Services.Services.AccountService.Dtos.AdminDtos;
using VetLink.Services.Services.AccountService.Dtos.AuthDtos;
using VetLink.Services.Services.AccountService.Dtos.BuyerDtos;
using VetLink.Services.Services.AccountService.Dtos.SellerDtos;
using VetLink.Services.Services.AccountService.TokenService.Dtos;
using VetLink.Services.Services.AccountService.UsersServices.AdminService;
using VetLink.Services.Services.AccountService.UsersServices.BuyerServices;

namespace VetLink.WebApi.Controllers.AccountControllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/admin")] 
    [Authorize(Roles = "Admin")]
    [EnableRateLimiting("auth")]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;
        private readonly IBuyerService _buyerService;

        public AdminController(IAdminService adminService, IBuyerService buyerService)
        {
            _adminService = adminService;
            _buyerService = buyerService;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _adminService.SignInAsync(dto);
            return new ResultActionResult<AuthTokenResultDto>(result);
        }

        [HttpPost("logout")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Logout()
        {
            var userId = GetUserId();
            var result = await _buyerService.LogoutAsync(userId);
            return new ResultActionResult(result);
        }

        [HttpGet("sellers/pending")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetPendingSellers([FromQuery] PaginationSpecification pagination)
        {
            if (pagination.PageIndex < 1 || pagination.PageSize < 1 || pagination.PageSize > 100)
                return BadRequest(new { message = "Invalid pagination parameters" });

            var result = await _adminService.GetPendingSellersAsync(pagination);
            return new ResultActionResult<PaginatedResultDto<PendingSellerDto>>(result);
        }

        [HttpGet("sellers")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAllSellers(
            [FromQuery] PaginationSpecification pagination,
            [FromQuery] AccountStatus? status = null,
            [FromQuery] string? search = null)
        {
            if (pagination.PageIndex < 1 || pagination.PageSize < 1 || pagination.PageSize > 100)
                return BadRequest(new { message = "Invalid pagination parameters" });

            var result = await _adminService.GetAllSellersAsync(status, search, pagination);
            return new ResultActionResult<PaginatedResultDto<SellerProfileDto>>(result);
        }

        [HttpPost("sellers/{sellerUserId}/approve")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ApproveSeller(int sellerUserId)
        {
            if (sellerUserId <= 0)
                return BadRequest(new { message = "Invalid seller user ID" });

            var approverId = GetUserId();
            var result = await _adminService.ApproveSellerAsync(sellerUserId, approverId);
            return new ResultActionResult(result);
        }

        [HttpPost("sellers/reject")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RejectSeller([FromBody] RejectSellerDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var approverId = GetUserId();
            var result = await _adminService.RejectSellerAsync(dto, approverId);
            return new ResultActionResult(result);
        }

        [HttpGet("buyers")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAllBuyers(
            [FromQuery] PaginationSpecification pagination,
            [FromQuery] string? search = null)
        {
            if (pagination.PageIndex < 1 || pagination.PageSize < 1 || pagination.PageSize > 100)
                return BadRequest(new { message = "Invalid pagination parameters" });

            var result = await _adminService.GetAllBuyersAsync(search, pagination);
            return new ResultActionResult<PaginatedResultDto<BuyerDto>>(result);
        }

        [HttpPost("users/{userId}/activate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ActivateUser(int userId)
        {
            if (userId <= 0)
                return BadRequest(new { message = "Invalid user ID" });

            var result = await _adminService.ActivateUserAsync(userId);
            return new ResultActionResult(result);
        }

        [HttpPost("users/{userId}/deactivate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeactivateUser(int userId)
        {
            if (userId <= 0)
                return BadRequest(new { message = "Invalid user ID" });

            var result = await _adminService.DeactivateUserAsync(userId);
            return new ResultActionResult(result);
        }

        [HttpPost("users/{userId}/reset-password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ResetUserPassword(int userId)
        {
            if (userId <= 0)
                return BadRequest(new { message = "Invalid user ID" });

            var result = await _adminService.ResetUserPasswordAsync(userId);
            return new ResultActionResult(result);
        }

        [HttpGet("profile")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetProfile()
        {
            var adminUserId = GetUserId();
            var result = await _adminService.GetProfileAsync(adminUserId);
            return new ResultActionResult<AdminProfileDto>(result);
        }

        [HttpPut("profile")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateAdminProfileDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var adminUserId = GetUserId();
            var result = await _adminService.UpdateProfileAsync(adminUserId, dto);
            return new ResultActionResult<AdminProfileDto>(result);
        }

        [HttpPost("admins")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateAdmin([FromBody] CreateAdminDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _adminService.CreateAdminAsync(dto);
            return new ResultActionResult<AdminProfileDto>(result);
        }

        [HttpGet("dashboard")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDashboard()
        {
            var result = await _adminService.GetDashboardAsync();
            return new ResultActionResult<AdminDashboardDto>(result);
        }

        [HttpGet("audit-logs")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAuditLogs(
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null,
            [FromQuery] string? action = null,
            [FromQuery] int? userId = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            if (page < 1 || pageSize < 1 || pageSize > 100)
                return BadRequest(new { message = "Invalid pagination parameters" });

            var result = await _adminService.GetAuditLogsAsync(fromDate, toDate, action, userId, page, pageSize);
            return new ResultActionResult<PaginatedResultDto<AuditLogDto>>(result);
        }

        [HttpGet("statistics")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetStatistics(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            if (startDate > endDate)
                return BadRequest(new { message = "Start date cannot be after end date" });

            var result = await _adminService.GetStatisticsAsync(startDate, endDate);
            return new ResultActionResult<StatisticsDto>(result);
        }

        [HttpPost("bulk-email")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SendBulkEmail([FromBody] BulkEmailDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _adminService.SendBulkEmailAsync(dto);
            return new ResultActionResult(result);
        }

        private int GetUserId()
        {
            var userIdClaim =
                User.FindFirst("uid")?.Value ??
                User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrWhiteSpace(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                throw new UnauthorizedAccessException("User ID not found in token");

            return userId;
        }
    }
}