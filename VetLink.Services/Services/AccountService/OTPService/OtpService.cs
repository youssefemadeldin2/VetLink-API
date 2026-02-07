using Microsoft.AspNetCore.Identity;
using VetLink.Data.Entities;
using VetLink.Data.Enums;
using VetLink.Services.Services.CachedService;

namespace VetLink.Services.Services.AccountService.OTPService
{
    public class OtpService : IOtpService
    {
        #region Constants
        private const string OtpKeyPrefix = "OTP";
        private const string AttemptsKeyPrefix = "OTP:ATTEMPTS";
        private const string NonceKeyPrefix = "OTP:NONCE";
        private const string CaptchaKeyPrefix = "OTP:CAPTCHA";
        private const string ResendKeyPrefix = "OTP:RESEND";

        private const int MaxOtpResendAttempts = 3;
        private const int MaxOtpValidationAttempts = 3;
        private const int OtpMinValue = 100000;
        private const int OtpMaxValue = 999999;
        private const int LockoutDurationMinutes = 60;
        private const int CaptchaThreshold = 2;

        private static readonly TimeSpan OtpExpiry = TimeSpan.FromMinutes(10);
        private static readonly TimeSpan ResendWindow = TimeSpan.FromMinutes(10);
        #endregion

        #region Dependencies
        private readonly ICachedService _cache;
        private readonly UserManager<User> _userManager;
        #endregion

        #region Constructor
        public OtpService(
            ICachedService cache,
            UserManager<User> userManager)
        {
            _cache = cache;
            _userManager = userManager;
        }
        #endregion

        #region Public API
        public async Task<(string Otp, string Nonce)> GenerateOtpAsync(string email, int userId)
        {
            var otp = GenerateOtp();
            var nonce = Guid.NewGuid().ToString("N");

            await CleanupOtpData(email, userId);
            await StoreOtpData(email, userId, otp, nonce);

            return (otp, nonce);
        }

        public async Task<bool> CanResendOtpAsync(string email, int userId)
        {
            var key = BuildKey(ResendKeyPrefix, email, userId);
            var count = Parse(await _cache.GetCacheResponseAsync(key));

            if (count >= MaxOtpResendAttempts)
                return false;

            await _cache.SetCacheResponseWithNoSerializAsync(
                key,
                (count + 1).ToString(),
                ResendWindow
            );

            return true;
        }

        public async Task<OtpValidationResult> ValidateOtpAsync(User user, string otp, string nonce)
        {
            if (await _userManager.IsLockedOutAsync(user))
                return OtpValidationResult.Locked;

            var email = user.Email!;
            var userId = user.Id;

            var storedHash = await _cache.GetCacheResponseAsync(
                BuildKey(OtpKeyPrefix, email, userId));

            if (storedHash is null)
                return OtpValidationResult.Expired;

            var nonceKey = BuildKey($"{NonceKeyPrefix}:{nonce}", email, userId);
            if (await _cache.GetCacheResponseAsync(nonceKey) is null)
                return OtpValidationResult.Invalid;

            var attempts = await GetAttempts(email, userId);

            if (!BCrypt.Net.BCrypt.Verify(otp, storedHash))
                return await HandleInvalidOtp(user, email, userId, attempts);

            await CleanupOtpData(email, userId);
            await _cache.RemoveCacheAsync(nonceKey);
            await _userManager.ResetAccessFailedCountAsync(user);

            return OtpValidationResult.Success;
        }

        public async Task InvalidateOtpAsync(string email, int userId)
        {
            await CleanupOtpData(email, userId);
        }
        #endregion

        #region Internal Logic
        private static string GenerateOtp()
            => Random.Shared.Next(OtpMinValue, OtpMaxValue + 1).ToString();

        private async Task StoreOtpData(string email, int userId, string otp, string nonce)
        {
            await _cache.SetCacheResponseWithNoSerializAsync(
                BuildKey(OtpKeyPrefix, email, userId),
                BCrypt.Net.BCrypt.HashPassword(otp),
                OtpExpiry
            );

            await _cache.SetCacheResponseWithNoSerializAsync(
                BuildKey(AttemptsKeyPrefix, email, userId),
                "0",
                OtpExpiry
            );

            await _cache.SetCacheResponseWithNoSerializAsync(
                BuildKey($"{NonceKeyPrefix}:{nonce}", email, userId),
                "1",
                OtpExpiry
            );

            await _cache.RemoveCacheAsync(
                BuildKey(CaptchaKeyPrefix, email, userId));
        }

        private async Task<int> GetAttempts(string email, int userId)
            => Parse(await _cache.GetCacheResponseAsync(
                BuildKey(AttemptsKeyPrefix, email, userId)));

        private async Task<OtpValidationResult> HandleInvalidOtp(User user, string email, int userId, int attempts)
        {
            attempts++;

            await _cache.SetCacheResponseWithNoSerializAsync(
                BuildKey(AttemptsKeyPrefix, email, userId),
                attempts.ToString(),
                OtpExpiry
            );

            if (attempts == CaptchaThreshold)
            {
                await _cache.SetCacheResponseWithNoSerializAsync(
                    BuildKey(CaptchaKeyPrefix, email, userId),
                    "1",
                    OtpExpiry
                );

                return OtpValidationResult.CaptchaRequired;
            }

            if (attempts >= MaxOtpValidationAttempts)
            {
                await _userManager.SetLockoutEndDateAsync(
                    user,
                    DateTimeOffset.UtcNow.AddMinutes(LockoutDurationMinutes));

                await CleanupOtpData(email, userId);
                return OtpValidationResult.Locked;
            }

            return OtpValidationResult.Invalid;
        }
        public async Task<(string Otp, string Nonce)> GenerateRegistrationOtpAsync(string email)
        {
            var otp = GenerateOtp();
            var nonce = Guid.NewGuid().ToString("N");

            var otpKey = $"REG:OTP:{email}";
            var nonceKey = $"REG:NONCE:{email}";
            var attemptsKey = $"REG:ATTEMPTS:{email}";

            await _cache.SetCacheResponseWithNoSerializAsync(
                otpKey,
                BCrypt.Net.BCrypt.HashPassword(otp),
                OtpExpiry);

            await _cache.SetCacheResponseWithNoSerializAsync(
                nonceKey,   
                nonce,
                OtpExpiry);

            await _cache.SetCacheResponseWithNoSerializAsync(
                attemptsKey,
                "0",
                OtpExpiry);

            return (otp, nonce);
        }
		public async Task ClearOtpAsync(string email)
		{
			await _cache.RemoveCacheAsync($"REG:OTP:{email}");
			await _cache.RemoveCacheAsync($"REG:NONCE:{email}");
		}
		public async Task<OtpValidationResult> ValidateRegistrationOtpAsync(string email, string otp, string nonce)
        {
            var otpKey = $"REG:OTP:{email}";
            var nonceKey = $"REG:NONCE:{email}";
            var attemptsKey = $"REG:ATTEMPTS:{email}";

			var storedHash = await _cache.GetCacheResponseAsync(otpKey);
			if (storedHash is null)
				return OtpValidationResult.Expired;

			var storedNonce = await _cache.GetCacheResponseAsync(nonceKey);
			if (storedNonce != nonce)
				return OtpValidationResult.Invalid;

			var attempts = Parse(await _cache.GetCacheResponseAsync(attemptsKey));

			if (!BCrypt.Net.BCrypt.Verify(otp, storedHash))
			{
				attempts++;
				await _cache.SetCacheResponseWithNoSerializAsync(
					attemptsKey,
					attempts.ToString(),
					OtpExpiry);

				if (attempts >= MaxOtpValidationAttempts)
					return OtpValidationResult.Locked;

				return attempts == CaptchaThreshold
					? OtpValidationResult.CaptchaRequired
					: OtpValidationResult.Invalid;
			}

			// success
			await _cache.RemoveCacheAsync(otpKey);
            await _cache.RemoveCacheAsync(nonceKey);
            await _cache.RemoveCacheAsync(attemptsKey);

            return OtpValidationResult.Success;
        }

        private async Task CleanupOtpData(string email, int userId)
        {
            var keys = new[]
            {
                BuildKey(OtpKeyPrefix, email, userId),
                BuildKey(AttemptsKeyPrefix, email, userId),
                BuildKey(CaptchaKeyPrefix, email, userId)
            };

            foreach (var key in keys)
                await _cache.RemoveCacheAsync(key);
        }

        private static string BuildKey(string prefix, string email, int userId)
            => $"{prefix}:{email.ToLowerInvariant()}:{userId}";

        private static int Parse(string? value)
            => int.TryParse(value, out var v) ? v : 0;
        #endregion
    }
}
