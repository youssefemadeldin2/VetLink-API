using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using VetLink.Repository.Interfaces;
using VetLink.Repository.Repositories;
using VetLink.Services.Services.AccountService.AuditLogService;
using VetLink.Services.Services.AccountService.MailService;
using VetLink.Services.Services.AccountService.OTPService;
using VetLink.Services.Services.AccountService.TokenService;
using VetLink.Services.Services.AccountService.UsersServices.AdminService;
using VetLink.Services.Services.AccountService.UsersServices.AdminServices;
using VetLink.Services.Services.AccountService.UsersServices.BuyerServices;
using VetLink.Services.Services.AccountService.UsersServices.SellerServices;
using VetLink.Services.Services.BrandService;
using VetLink.Services.Services.CachedService;
using VetLink.Services.Services.CategoryService;
using VetLink.Services.Services.ImageService;
using VetLink.Services.Services.Notifications;
using VetLink.Services.Services.OrderService;
using VetLink.Services.Services.ProductService;
using VetLink.Services.Services.ReviewService;
using VetLink.Services.Validators;

namespace VetLink.WebApi.Helpers
{
    public static class ServiceRegistration
    {
        public static void AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped(typeof(IGenericRepository<,>), typeof(GenericRepository<,>));
            services.AddScoped<ICachedService, CachedService>();
            //services.AddAutoMapper(typeof(CategoryProfile));
            services.AddScoped<ICategoryService, CategoryService>();
            //services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddTransient<IEmailService, EmailService>();
            services.AddScoped<IOtpService, OtpService>();
            services.AddScoped<IBuyerService, BuyerService>();
            services.AddScoped<ISellerService, SellerService>();
            services.AddScoped<IAdminService, AdminService>();
            services.AddScoped<IAuditLogService, AuditLogService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IBrandService, BrandService>();
            services.AddScoped<IReviewService, ReviewService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IImageStorageService, CloudinaryImageStorageService>();
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());// auto mapper track class ": Profile
			services.AddValidatorsFromAssemblyContaining<CreateProductDtoValidator>();
			services.AddFluentValidationAutoValidation();
			services.AddFluentValidationClientsideAdapters();

			services.AddSignalR();
            services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly", policy =>
                    policy.RequireClaim("UserRole", "Admin"));
            });
            services.AddAuthorization(options =>
            {
                options.AddPolicy("Admin-Seller", policy =>
                    policy.RequireClaim("UserRole", ["Admin", "Seller"]));
            });
            services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
                options.ApiVersionReader = new UrlSegmentApiVersionReader();
            });
            services.AddVersionedApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });
        }
    }
}
