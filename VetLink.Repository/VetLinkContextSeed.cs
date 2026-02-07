using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using VetLink.Data.Contexts;
using VetLink.Data.Entities;
using VetLink.Data.Enums;

namespace VetLink.Repository
{
    public class VetLinkContextSeed
    {
		public static async Task SeedAsync(VetLinkDbContext context,ILoggerFactory logger)
		{
			try
			{
				//if(context.Categories !=null && !context.Categories.Any())
				//{
				//	var CategoryData = File.ReadAllText("../VetLink.Repository/SeedData/Categories.json");
				//	var categories = JsonSerializer.Deserialize<List<Category>>(CategoryData);
				//	if (categories is not null)
				//		await context.Categories.AddRangeAsync(categories);
				//}
			}catch(Exception ex)
			{
				var loggerF = logger.CreateLogger<VetLinkContextSeed>();
				loggerF.LogError(ex.Message);
			}
		}

        public static async Task SeedAdmin(UserManager<User> userManager, RoleManager<Role> roleManager)
        {
           
                if (!userManager.Users.Any())
                {
                    var roles = new[] { "Admin", "Seller", "Buyer", "Veterinarian" };

					if(!roleManager.Roles.Any())
						foreach (var role in roles)
							if (!await roleManager.RoleExistsAsync(role))
								await roleManager.CreateAsync(new Role { Name =role});
							
						
                    var user = new User
                    {
						UserName = "admin@vetlink.com",
						NormalizedUserName = "ADMIN@VETLINK.COM",
						Email = "admin@vetlink.com",
						NormalizedEmail = "ADMIN@VETLINK.COM",
						EmailConfirmed = true,
						PhoneNumber = "+201550554041",
						PhoneNumberConfirmed = true,
						FullName = "System Administrator",
						Status = AccountStatus.active,
						EmailVerifiedAt = DateTime.UtcNow,
						CreatedAt = DateTime.UtcNow,
						UpdatedAt = DateTime.UtcNow,
						SecurityStamp = Guid.NewGuid().ToString(),
						ConcurrencyStamp = Guid.NewGuid().ToString(),
						LockoutEnabled = true,
						TwoFactorEnabled = false,
						AccessFailedCount = 0,
					};
					var result = await userManager.CreateAsync(user, "|VetLinkAdmin@VetLink.Com2026-");
					if (result.Succeeded)
					{
						// Assign admin role
						await userManager.AddToRoleAsync(user, "Admin");

						// Optionally assign additional roles
						//await userManager.AddToRoleAsync(user, "Seller");

						Console.WriteLine("Admin user created successfully!");
					}
					else
					{
						Console.WriteLine("Failed to create admin user:");
						foreach (var error in result.Errors)
						{
							Console.WriteLine($"- {error.Description}");
						}
					}
				}
        }
    }
}
