using VetLink.Services.Helper;
using VetLink.Services.Services.AccountService.Dtos.AuthDtos;
using VetLink.Services.Services.AccountService.Dtos.SellerDtos;
using VetLink.Services.Services.AccountService.TokenService.Dtos;

namespace VetLink.Services.Services.AccountService.UsersServices.SellerServices
{
    public interface ISellerService
    {
        Task<ServiceResult> RegisterAsync(RegisterAsSellerDto dto);
        Task<ServiceResult> VerifyEmailOtpAsync(string email, string otp, string nonce);
        Task<ServiceResult<AuthTokenResultDto>> SignInAsync(LoginDto dto);
    }
}