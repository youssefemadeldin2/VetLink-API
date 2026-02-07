using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using VetLink.WebApi.Helpers;
using VetLink.Repository.Specifications.Paginated;
using VetLink.Services.Services.AccountService.Dtos.AuthDtos;
using VetLink.Services.Services.AccountService.Dtos.BuyerDtos;
using VetLink.Services.Services.AccountService.Dtos.SellerDtos;
using VetLink.Services.Services.AccountService.TokenService.Dtos;
using VetLink.Services.Services.AccountService.UsersServices.BuyerServices;

namespace VetLink.WebApi.Controllers.AccountControllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/buyer")]
    public class BuyerController : ControllerBase
    {
        private readonly IBuyerService _buyerService;

        public BuyerController(IBuyerService buyerService)
        {
            _buyerService = buyerService;
        }

        // REGISTER 
        [HttpPost("register")]
        [AllowAnonymous]
        [EnableRateLimiting("auth")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterAsBuyerDto dto)
        {
            var result = await _buyerService.RegisterAsync(dto);
            return new ResultActionResult(result);
        }

        // VERIFY EMAIL 
        [HttpPost("verify-email")]
        [AllowAnonymous]
        [EnableRateLimiting("auth")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> VerifyEmail([FromBody] OtpVerificationDto dto)
        {
            var result = await _buyerService.VerifyEmailOtpAsync(dto.Email, dto.OTP, dto.Nonce);
            return new ResultActionResult<AuthTokenResultDto>(result);
        }

        // LOGIN 
        [HttpPost("login")]
        [AllowAnonymous]
        [EnableRateLimiting("auth")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status423Locked)]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var result = await _buyerService.SignInAsync(dto);
            return new ResultActionResult<AuthTokenResultDto>(result);
        }

        // RESEND OTP 
        [HttpPost("resend-otp")]
        [AllowAnonymous]
        [EnableRateLimiting("auth")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        public async Task<IActionResult> ResendOtp([FromBody] ResendOtpDto dto)
        {
            var result = await _buyerService.ReSendOTPAsync(dto);
            return new ResultActionResult(result);
        }

        // FORGOT PASSWORD 
        [HttpPost("forgot-password")]
        [AllowAnonymous]
        [EnableRateLimiting("auth")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
        {
            var result = await _buyerService.ForgotPasswordAsync(dto);
            return new ResultActionResult(result);
        }

        // RESET PASSWORD 
        [HttpPost("reset-password")]
        [AllowAnonymous]
        [EnableRateLimiting("auth")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            var result = await _buyerService.ResetPasswordAsync(dto);
            return new ResultActionResult(result);
        }

        // PROFILE 
        [HttpGet("profile")]
        [Authorize(Roles = "Buyer")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetProfile([FromQuery] PaginationSpecification pagination)
        {
            var result = await _buyerService.GetProfileAsync(GetUserId(), pagination);
            return new ResultActionResult<BuyerProfileDto>(result);
        }

        [HttpPut("profile")]
        [Authorize(Roles = "Buyer")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateBuyerProfileDto dto)
        {
            var result = await _buyerService.UpdateProfileAsync(GetUserId(), dto);
            return new ResultActionResult<BuyerProfileDto>(result);
        }

        // PASSWORD 
        [HttpPost("change-password")]
        [Authorize(Roles = "Buyer")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            var result = await _buyerService.ChangePasswordAsync(GetUserId(), dto);
            return new ResultActionResult(result);
        }

        // LOGOUT 
        [HttpPost("logout")]
        [Authorize(Roles = "Buyer")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Logout()
        {
            var result = await _buyerService.LogoutAsync(GetUserId());
            return new ResultActionResult(result);
        }

        // REFRESH TOKEN 
        [HttpPost("refresh-token")]
        [AllowAnonymous]
        [EnableRateLimiting("auth")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto dto)
        {
            var result = await _buyerService.RefreshTokenAsync(dto);
            return new ResultActionResult<AuthTokenResultDto>(result);
        }

        // HELPERS 
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