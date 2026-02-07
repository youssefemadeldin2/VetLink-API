using System.Threading.RateLimiting;

namespace VetLink.WebApi.Extentions
{
    public static class RateLimetingExtention
    {
		public static IServiceCollection AddRateLimeting(this IServiceCollection services)
        {
			services.AddRateLimiter(options =>
			{
				options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

				options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
				{
					var userId = context.User?.FindFirst("uid")?.Value;

					var key = !string.IsNullOrEmpty(userId)
						? $"user:{userId}"
						: $"ip:{context.Connection.RemoteIpAddress}";

					return RateLimitPartition.GetFixedWindowLimiter(
						partitionKey: key,
						factory: _ => new FixedWindowRateLimiterOptions
						{
							PermitLimit = 100,
							Window = TimeSpan.FromMinutes(1),
							QueueLimit = 0
						});
				});

				options.OnRejected = async (context, token) =>
				{
					context.HttpContext.Response.ContentType = "application/json";

					await context.HttpContext.Response.WriteAsJsonAsync(new
					{
						success = false,
						message = "Too many requests. Please slow down.",
						statusCode = 429
					}, token);
				};
				options.AddPolicy("auth", context =>
				{
					var userId = context.User?.FindFirst("uid")?.Value;
					var key = userId ?? context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

					return RateLimitPartition.GetFixedWindowLimiter(
						key,
						_ => new FixedWindowRateLimiterOptions
						{
							PermitLimit = 3,
							Window = TimeSpan.FromMinutes(1)
						});
				});
				options.AddPolicy("Admin", context =>
				{
					var userId = context.User.FindFirst("uid")?.Value ?? "admin";
					return RateLimitPartition.GetFixedWindowLimiter(
						$"Admin:{userId}",
						_ => new FixedWindowRateLimiterOptions
						{
							PermitLimit = 200,
							Window = TimeSpan.FromMinutes(1)
						});
				});
			});


			return services;
        }
 	}
}
