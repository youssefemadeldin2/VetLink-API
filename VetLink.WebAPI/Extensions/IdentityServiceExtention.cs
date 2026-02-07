using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using VetLink.Data.Contexts;
using VetLink.Data.Entities;
using VetLink.Services.Services.AccountService.TokenService;

namespace VetLink.WebApi.Extentions
{
    public static class IdentityServiceExtention
    {
        public static IServiceCollection AddIdentityService(
            this IServiceCollection services,
            IConfiguration config)
        {
            services.AddIdentity<User, Role>(options =>
            {
                options.User.RequireUniqueEmail = true;

                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;

                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;

                options.SignIn.RequireConfirmedEmail = false;
            })
            .AddEntityFrameworkStores<VetLinkDbContext>()
            .AddDefaultTokenProviders();
            services.ConfigureApplicationCookie(options =>
            {
                options.Events.OnRedirectToLogin = context =>
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return Task.CompletedTask;
                };

                options.Events.OnRedirectToAccessDenied = context =>
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    return Task.CompletedTask;
                };
            });

            services.AddAuthentication(
                options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,

                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(config["Token:Key"]!)
                        ),
                        ValidIssuer = config["Token:Issuer"],
                        ValidAudience = config["Token:Audience"],
                        ClockSkew = TimeSpan.Zero,
                        RoleClaimType = ClaimTypes.Role
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];
                            var path = context.HttpContext.Request.Path;

                            if (!string.IsNullOrEmpty(accessToken) &&
                                path.StartsWithSegments("/hubs/notifications"))
                            {
                                context.Token = accessToken;
                            }

                            return Task.CompletedTask;
                        },

                        OnAuthenticationFailed = context =>
                        {
                            if (context.Exception is SecurityTokenExpiredException)
                            {
                                context.Response.Headers.Add("Token-Expired", "true");
                            }
                            return Task.CompletedTask;
                        },

                        OnChallenge = context =>
                        {
                            context.HandleResponse();

                            context.Response.StatusCode = 401;
                            context.Response.ContentType = "application/json";

                            var response = new
                            {
                                StatusCode = 401,
                                Message = "Unauthorized: Invalid or missing token"
                            };

                            return context.Response.WriteAsync(
                                JsonSerializer.Serialize(response)
                            );
                        },

                        OnForbidden = context =>
                        {
                            context.Response.StatusCode = 403;
                            context.Response.ContentType = "application/json";

                            var response = new
                            {
                                StatusCode = 403,
                                Message = "Forbidden: Access denied"
                            };

                            return context.Response.WriteAsync(
                                JsonSerializer.Serialize(response)
                            );
                        },

                        OnTokenValidated = async context =>
                        {
                            var tokenService = context.HttpContext.RequestServices
                                .GetRequiredService<ITokenService>();

                            var jti = context.Principal?
                                .FindFirst(JwtRegisteredClaimNames.Jti)?.Value;

                            if (string.IsNullOrEmpty(jti) ||
                                !await tokenService.IsAccessTokenValid(jti))
                            {
                                context.Fail("Token revoked");
                            }
                        }
                    };
                });

            return services;
        }
    }
}
