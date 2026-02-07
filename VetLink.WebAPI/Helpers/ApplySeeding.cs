using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VetLink.Data.Contexts;
using VetLink.Data.Entities;
using VetLink.Repository;

namespace VetLink.WebApi.Helpers
{
    public class ApplySeeding
    {
        public static async Task ApplySeedingAsync(WebApplication app)
        {
            //app.MapHealthChecks("/health", new HealthCheckOptions
            //{
            //	ResponseWriter = async (context, report) =>
            //	{
            //		var result = JsonSerializer.Serialize(new
            //		{
            //			status = report.Status.ToString(),
            //			checks = report.Entries.Select(e => new
            //			{
            //				name = e.Key,
            //				status = e.Value.Status.ToString(),
            //				description = e.Value.Description
            //			})
            //		});
            //		context.Response.ContentType = MediaTypeNames.Application.Json;
            //		await context.Response.WriteAsync(result);
            //	}
            //});

            //app.MapHealthChecks("/health/ready", new HealthCheckOptions
            //{
            //	Predicate = check => check.Tags.Contains("ready")
            //});

            //app.MapHealthChecks("/health/live", new HealthCheckOptions
            //{
            //	Predicate = _ => false
            //});
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var loggerFactory = services.GetRequiredService<ILoggerFactory>();
                try
                {
                    var context = services.GetRequiredService<VetLinkDbContext>();

                    var userManager = services.GetRequiredService<UserManager<User>>();

                    var roleManager = services.GetRequiredService<RoleManager<Role>>();


                    await context.Database.MigrateAsync();

                    await VetLinkContextSeed.SeedAdmin(userManager, roleManager);
                    await VetLinkContextSeed.SeedAsync(context, loggerFactory);

                    await context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    var logger = loggerFactory.CreateLogger<ApplySeeding>();
                    logger.LogError(ex.Message);
                }
            }
        }
    }
}
