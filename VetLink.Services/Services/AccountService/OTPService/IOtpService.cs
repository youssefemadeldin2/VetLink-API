using VetLink.Data.Entities;
using VetLink.Data.Enums;
using VetLink.Services.Services.AccountService.TokenService.Dtos;

namespace VetLink.Services.Services.AccountService.OTPService
{
    public interface IOtpService
    {
		Task<(string Otp, string Nonce)> GenerateOtpAsync(string email, int userId);
		Task<(string Otp, string Nonce)> GenerateRegistrationOtpAsync(string email);
		Task<OtpValidationResult> ValidateRegistrationOtpAsync(string email,string otp,string nonce);
		Task<OtpValidationResult> ValidateOtpAsync(User user,string otp,string nonce);
		Task<bool> CanResendOtpAsync(string email,int userId);
		Task InvalidateOtpAsync(string email,int userId);
		Task ClearOtpAsync(string email);
	}

}
