using VetLink.Repository.Specifications.Paginated;
using VetLink.Services.Helper;
using VetLink.Services.Services.AccountService.Dtos.AuthDtos;
using VetLink.Services.Services.AccountService.Dtos.BuyerDtos;
using VetLink.Services.Services.AccountService.Dtos.SellerDtos;
using VetLink.Services.Services.AccountService.TokenService.Dtos;

namespace VetLink.Services.Services.AccountService.UsersServices.BuyerServices
{
    public interface IBuyerService
    {
        Task<ServiceResult> RegisterAsync(RegisterAsBuyerDto dto);
        Task<ServiceResult<AuthTokenResultDto>> VerifyEmailOtpAsync(string email, string otp, string nonce);
        Task<ServiceResult<AuthTokenResultDto>> SignInAsync(LoginDto dto);
        Task<ServiceResult> ReSendOTPAsync(ResendOtpDto dto);
        Task<ServiceResult> ForgotPasswordAsync(ForgotPasswordDto dto);
        Task<ServiceResult> ResetPasswordAsync(ResetPasswordDto dto);
        Task<ServiceResult<BuyerProfileDto>> GetProfileAsync(int userId, PaginationSpecification pagination);
        Task<ServiceResult> ChangePasswordAsync(int userId, ChangePasswordDto dto);
        Task<ServiceResult> LogoutAsync(int userId);
        Task<ServiceResult<AuthTokenResultDto>> RefreshTokenAsync(RefreshTokenDto dto);
        Task<ServiceResult<BuyerProfileDto>> UpdateProfileAsync(int userId, UpdateBuyerProfileDto dto);

    }
}
