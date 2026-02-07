using System.Net;
using System.Text.Json;
using VetLink.WebApi.Middleware;
using VetLink.Services;
using VetLink.Services.Helper;

namespace VetLink.WebApi.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger,
            IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred");
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            
            var response = exception switch
            {
                ValidationException validationEx => ServiceResult<string>.Fail(
                    "Validation failed",
                    (int)HttpStatusCode.UnprocessableEntity,
                    validationEx.Errors),
                NotFoundException notFoundEx => ServiceResult<string>.Fail(
                    notFoundEx.Message,
                    (int)HttpStatusCode.NotFound),
                _ => _env.IsDevelopment()
                    ? ServiceResult<string>.Fail(
                        $"An error occurred: {exception.Message}. Stack Trace: {exception.StackTrace}",
                        (int)HttpStatusCode.InternalServerError)
                    : ServiceResult<string>.Fail(
                        "An internal server error occurred. Please try again later.",
                        (int)HttpStatusCode.InternalServerError)
            };

            context.Response.StatusCode = response.StatusCode;
            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var json = JsonSerializer.Serialize(response, options);

            await context.Response.WriteAsync(json);
        }
    }
}
