using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using VetLink.WebApi.Helpers;
using VetLink.Services.Services.AccountService.Dtos.AuthDtos;
using VetLink.Services.Services.AccountService.Dtos.SellerDtos;
using VetLink.Services.Services.AccountService.TokenService.Dtos;
using VetLink.Services.Services.AccountService.UsersServices.BuyerServices;
using VetLink.Services.Services.AccountService.UsersServices.SellerServices;

namespace VetLink.WebApi.Controllers.AccountControllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/seller")]
    [Authorize(Roles = "Seller")]
    public class SellerController : ControllerBase
    {
        private readonly ISellerService _sellerService;
        private readonly IBuyerService _buyerService;

        public SellerController(ISellerService sellerService, IBuyerService buyerService)
        {
            _sellerService = sellerService;
            _buyerService = buyerService;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        [EnableRateLimiting("auth")]
        public async Task<IActionResult> Register([FromBody] RegisterAsSellerDto dto)
        {
            var result = await _sellerService.RegisterAsync(dto);
            return new ResultActionResult(result);
        }

        [HttpPost("verify-email")]
        [AllowAnonymous]
        [EnableRateLimiting("auth")]
        public async Task<IActionResult> VerifyEmail([FromBody] OtpVerificationDto dto)
        {
            var result = await _sellerService.VerifyEmailOtpAsync(dto.Email, dto.OTP, dto.Nonce);
            return new ResultActionResult(result);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        [EnableRateLimiting("auth")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var result = await _sellerService.SignInAsync(dto);
            return new ResultActionResult<AuthTokenResultDto>(result);
        }

        [HttpPost("resend-otp")]
        [EnableRateLimiting("auth")]
        [AllowAnonymous]
        public async Task<IActionResult> ResendOtp([FromBody] ResendOtpDto dto)
        {
            var result = await _buyerService.ReSendOTPAsync(dto);
            return new ResultActionResult(result);
        }

        [HttpPost("forgot-password")]
        [AllowAnonymous]
        [EnableRateLimiting("auth")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
        {
            var result = await _buyerService.ForgotPasswordAsync(dto);
            return new ResultActionResult(result);
        }

        [HttpPost("reset-password")]
        [AllowAnonymous]
        [EnableRateLimiting("auth")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            var result = await _buyerService.ResetPasswordAsync(dto);
            return new ResultActionResult(result);
        }

        [HttpPost("logout")]
        [EnableRateLimiting("auth")]
        public async Task<IActionResult> Logout()
        {
            var result = await _buyerService.LogoutAsync(GetUserId());
            return new ResultActionResult(result);
        }

        // [HttpGet("profile")]
        // [Authorize(Roles = "Seller")]
        // [ProducesResponseType(StatusCodes.Status200OK)]
        // public async Task<IActionResult> GetProfile()
        // {
        //     var result = await _sellerService.GetProfileAsync(GetUserId());
        //     return new ResultActionResult<SellerProfileDto>(result);
        // }

        // [HttpPut("profile")]
        // [Authorize(Roles = "Seller")]
        // [ProducesResponseType(StatusCodes.Status200OK)]
        // [ProducesResponseType(StatusCodes.Status400BadRequest)]
        // public async Task<IActionResult> UpdateProfile([FromBody] UpdateSellerProfileDto dto)
        // {
        //     var userId = GetUserId();
        //     var result = await _sellerService.UpdateProfileAsync(userId, dto);
        //     return new ResultActionResult<SellerProfileDto>(result);
        // }

        // [HttpPut("store")]
        // [Authorize(Roles = "Seller")]
        // [ProducesResponseType(StatusCodes.Status200OK)]
        // [ProducesResponseType(StatusCodes.Status400BadRequest)]
        // public async Task<IActionResult> UpdateStore([FromBody] UpdateStoreDto dto)
        // {
        //     var userId = GetUserId();
        //     var result = await _sellerService.UpdateStoreAsync(userId, dto);
        //     return new ResultActionResult(result);
        // }

        // [HttpPost("change-password")]
        // [Authorize(Roles = "Seller")]
        // [ProducesResponseType(StatusCodes.Status200OK)]
        // [ProducesResponseType(StatusCodes.Status400BadRequest)]
        // public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        // {
        //     var userId = GetUserId();
        //     var result = await _sellerService.ChangePasswordAsync(userId, dto);
        //     return new ResultActionResult(result);
        // }

        // [HttpGet("dashboard")]
        // [Authorize(Roles = "Seller")]
        // [ProducesResponseType(StatusCodes.Status200OK)]
        // public async Task<IActionResult> GetDashboard()
        // {
        //     var userId = GetUserId();
        //     var result = await _sellerService.GetDashboardAsync(userId);
        //     return new ResultActionResult<SellerDashboardDto>(result);
        // }

        // [HttpGet("status")]
        // [AllowAnonymous]
        // [ProducesResponseType(StatusCodes.Status200OK)]
        // public async Task<IActionResult> CheckStatus([FromQuery] string email)
        // {
        //     var result = await _sellerService.CheckStatusAsync(email);
        //     return new ResultActionResult<SellerStatusDto>(result);
        // }

        // [HttpPost("reapply")]
        // [AllowAnonymous]
        // [ProducesResponseType(StatusCodes.Status200OK)]
        // [ProducesResponseType(StatusCodes.Status400BadRequest)]
        // public async Task<IActionResult> ReapplyAfterRejection([FromBody] ReapplySellerDto dto)
        // {
        //     var result = await _sellerService.ReapplyAfterRejectionAsync(dto);
        //     return new ResultActionResult(result);
        // }

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