using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Serilog;
using StackExchange.Redis;
using VetLink.Data.Contexts;
using VetLink.Services.Services.AccountService.MailService.Dtos;
using VetLink.Services.Services.ImageService.Dtos;
using VetLink.WebApi.Extentions;
using VetLink.WebApi.Helpers;
using VetLink.WebApi.Middleware;

namespace VetLink.WebApi
{
	public class Program
	{
		public static async Task Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Controllers
			builder.Services.AddControllers();

			// SQL Server
			builder.Services.AddDbContext<VetLinkDbContext>(options =>
			{
				options.UseSqlServer(
					builder.Configuration.GetConnectionString("VetLinkDB")
				);
			});

			// Redis
			builder.Services.AddSingleton<IConnectionMultiplexer>(_ =>
			{
				var config = ConfigurationOptions.Parse(
					builder.Configuration.GetConnectionString("redis")
				);
				return ConnectionMultiplexer.Connect(config);
			});

			// Email
			builder.Services.Configure<EmailSettings>(
				builder.Configuration.GetSection("EmailSettings")
			);

			// Cloudinary
			builder.Services.Configure<CloudinarySettings>(
				builder.Configuration.GetSection("Cloudinary")
			);

			// Application services
			builder.Services.AddApplicationServices();

			builder.Services.AddIdentityService(builder.Configuration);

			// Rate limiting
			builder.Services.AddRateLimeting();

			builder.Services.AddSwaggerDocumentation();

			var app = builder.Build();

			await ApplySeeding.ApplySeedingAsync(app);

			if (app.Environment.IsDevelopment())
			{
				app.UseOpenApi();       
				app.UseSwaggerUi();    
			}

			app.UseHsts();
			app.UseHttpsRedirection();

			app.UseExceptionHandler("/error");
			app.UseMiddleware<ExceptionMiddleware>();

			app.UseRateLimiter();

			app.UseAuthentication();
			app.UseAuthorization();

			app.MapControllers();

			app.Run();
		}
	}
}
