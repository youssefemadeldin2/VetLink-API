using VetLink.Data.Entities;
using VetLink.Services.Services.AccountService.TokenService.Dtos;

namespace VetLink.Services.Services.AccountService.TokenService
{
    public interface ITokenService
    {
        Task<AuthTokenResultDto> GenerateTokens(User user);

        Task<AuthTokenResultDto?> RefreshTokens(string refreshToken);

        Task InvalidateTokens(User user);

        Task<bool> IsAccessTokenValid(string jti);
    }
}
