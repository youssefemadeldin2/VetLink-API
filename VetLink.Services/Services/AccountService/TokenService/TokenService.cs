using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using VetLink.Data.Entities;
using VetLink.Services.Services.AccountService.TokenService.Dtos;
using VetLink.Services.Services.CachedService;

namespace VetLink.Services.Services.AccountService.TokenService
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;
        private readonly UserManager<User> _userManager;
        private readonly SymmetricSecurityKey _key;
        private readonly ICachedService _cache;

        private const int AccessTokenMinutes = 30;
        private const int RefreshTokenHours = 1;

        public TokenService(
            IConfiguration config,
            UserManager<User> userManager,
            ICachedService cache)
        {
            _config = config;
            _userManager = userManager;
            _cache = cache;
            _key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Token:Key"])
            );
        }

        public async Task<AuthTokenResultDto> GenerateTokens(User user)
        {
            var (accessToken, expiresAt, jti) = await GenerateAccessToken(user);
            var refreshToken = GenerateRefreshToken();

            await _cache.SetCacheResponseWithNoSerializAsync(
                $"ACCESS:{jti}",
                user.Id.ToString(),
                TimeSpan.FromMinutes(AccessTokenMinutes)
            );

            await AppendUserJti(user.Id, jti);

            await _cache.SetCacheResponseWithNoSerializAsync(
                $"REFRESH:{refreshToken}",
                user.Id.ToString(),
                TimeSpan.FromHours(RefreshTokenHours)
            );

            return new AuthTokenResultDto
            {
                AccessToken = accessToken,
                ExpiresAt = expiresAt,
                RefreshToken = refreshToken,
                RefreshTokenExpiresAt = DateTime.UtcNow.AddHours(RefreshTokenHours)
            };
        }

        public async Task<AuthTokenResultDto?> RefreshTokens(string refreshToken)
        {
            var userIdStr = await _cache.GetCacheResponseAsync($"REFRESH:{refreshToken}");
            if (string.IsNullOrEmpty(userIdStr))
                return null;

            await _cache.RemoveCacheAsync($"REFRESH:{refreshToken}");

            var user = await _userManager.FindByIdAsync(userIdStr);
            if (user == null)
                return null;

            return await GenerateTokens(user);
        }

        public async Task InvalidateTokens(User user)
        {
            var jtisCsv = await _cache.GetCacheResponseAsync($"USER_TOKENS:{user.Id}");
            if (!string.IsNullOrEmpty(jtisCsv))
            {
                var jtis = jtisCsv.Split(',', StringSplitOptions.RemoveEmptyEntries);
                foreach (var jti in jtis)
                    await _cache.RemoveCacheAsync($"ACCESS:{jti}");
            }

            await _cache.RemoveCacheAsync($"USER_TOKENS:{user.Id}");

            var refreshKeys = await _cache.GetKeysAsync("REFRESH:*");
            foreach (var key in refreshKeys)
            {
                var value = await _cache.GetCacheResponseAsync(key);
                if (value == user.Id.ToString())
                    await _cache.RemoveCacheAsync(key);
            }
        }

        public async Task<bool> IsAccessTokenValid(string jti)
        {
            var exists = await _cache.GetCacheResponseAsync($"ACCESS:{jti}");
            return !string.IsNullOrEmpty(exists);
        }

        private async Task<(string Token, DateTime Expires, string Jti)> GenerateAccessToken(User user)
        {
            var jti = Guid.NewGuid().ToString();

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, jti),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim("status", user.Status.ToString())
            };

            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
				claims.Add(new Claim(ClaimTypes.Role, role));


			var expires = DateTime.UtcNow.AddMinutes(AccessTokenMinutes);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Issuer = _config["Token:Issuer"],
                Audience = _config["Token:Audience"],
                Expires = expires,
                SigningCredentials = new SigningCredentials(
                    _key,
                    SecurityAlgorithms.HmacSha256
                )
            };

            var handler = new JwtSecurityTokenHandler();
            var token = handler.CreateToken(tokenDescriptor);

            return (handler.WriteToken(token), expires, jti);
        }

        private string GenerateRefreshToken()
        {
            var bytes = RandomNumberGenerator.GetBytes(64);
            return Convert.ToBase64String(bytes);
        }

        private async Task AppendUserJti(int userId, string jti)
        {
            var key = $"USER_TOKENS:{userId}";
            var existing = await _cache.GetCacheResponseAsync(key);

            var value = string.IsNullOrEmpty(existing)
                ? jti
                : $"{existing},{jti}";

            await _cache.SetCacheResponseWithNoSerializAsync(
                key,
                value,
                TimeSpan.FromDays(1)
            );
        }
    }
}
