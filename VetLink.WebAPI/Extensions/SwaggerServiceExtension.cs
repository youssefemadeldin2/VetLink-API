//using NSwag;
//using NSwag.Generation.Processors.Security;

//namespace VetLink.WebApi.Extentions
//{
//    public static class SwaggerServiceExtension
//    {
//        public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
//        {


//			services.AddOpenApiDocument(options =>
//			{
//				options.Title = "VetLink API";

//				options.AddSecurity("Bearer", new OpenApiSecurityScheme
//				{
//					Type = OpenApiSecuritySchemeType.Http,
//					Scheme = "bearer",
//					BearerFormat = "JWT",
//					In = OpenApiSecurityApiKeyLocation.Header,
//					Description = "Enter: Bearer {your JWT token}"
//				});

//				options.OperationProcessors.Add(
//					new AspNetCoreOperationSecurityScopeProcessor("Bearer")
//				);
//			});

//			return services;

//            //services.AddSwaggerGen(c =>
//            //{
//            //    c.SwaggerDoc("v1", new OpenApiInfo
//            //    {
//            //        Title = "VetLink API",
//            //        Version = "v1",
//            //        Description = "VetLink API backend",
//            //        Contact = new OpenApiContact
//            //        {
//            //            Name = "Saeed Tai3",
//            //            Email = "saiedsaied372@gmail.com"
//            //        }
//            //    });

//            //    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
//            //    {
//            //        Name = "Authorization",
//            //        Type = SecuritySchemeType.ApiKey,
//            //        Scheme = "Bearer",
//            //        BearerFormat = "JWT",
//            //        In = ParameterLocation.Header,
//            //        Description = "Enter: Bearer {token}"
//            //    });

//            //    c.AddSecurityRequirement(document =>
//            //    {
//            //        var schemeRef = new OpenApiSecuritySchemeReference("Bearer");

//            //        var requirement = new OpenApiSecurityRequirement
//            //        {
//            //            [schemeRef] = new List<string>()
//            //        };

//            //        return requirement;
//            //    });
//            //});

//            //return services;

//        }
//    }
//}
using NSwag;
using NSwag.Generation.Processors.Security;

namespace VetLink.WebApi.Extentions
{
	public static class SwaggerServiceExtension
	{
		public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
		{
			services.AddOpenApiDocument(options =>
			{
				options.Title = "VetLink API";

				options.AddSecurity("Bearer", new OpenApiSecurityScheme
				{
					Type = OpenApiSecuritySchemeType.Http,
					Scheme = "bearer",
					BearerFormat = "JWT",
					Description = "Enter: Bearer {your JWT token}"
				});

				options.OperationProcessors.Add(
					new AspNetCoreOperationSecurityScopeProcessor("Bearer")
				);
			});

			return services;
		}
	}
}
