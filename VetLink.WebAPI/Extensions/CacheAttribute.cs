using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using VetLink.Services.Helper;
using VetLink.Services.Services.CachedService;

namespace VetLink.WebApi.Helpers
{
    [AttributeUsage(AttributeTargets.Method)]
    public class CacheAttribute : Attribute, IAsyncActionFilter
    {
        private readonly int _timeToLiveSeconds;

        public CacheAttribute(int timeToLiveSeconds)
        {
            _timeToLiveSeconds = timeToLiveSeconds;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // Only cache GET requests
            if (!string.Equals(context.HttpContext.Request.Method, "GET", StringComparison.OrdinalIgnoreCase))
            {
                await next();
                return;
            }

            var cachedService = context.HttpContext.RequestServices.GetRequiredService<ICachedService>();
            var cacheKey = GenerateCacheKeyFromRequest(context.HttpContext.Request);
            var cachedResponse = await cachedService.GetCacheResponseAsync(cacheKey);

            if (!string.IsNullOrEmpty(cachedResponse))
            {
                var contentResult = new ContentResult
                {
                    Content = cachedResponse,
                    StatusCode = StatusCodes.Status200OK,
                    ContentType = "application/json"
                };

                // Add cache header
                context.HttpContext.Response.Headers["X-Cache"] = "HIT";
                context.Result = contentResult;
                return;
            }

            var executedContext = await next();

            // Add cache header for misses
            context.HttpContext.Response.Headers["X-Cache"] = "MISS";

            // Check if we should cache this response
            if (ShouldCacheResponse(executedContext))
            {
                try
                {
                    // Get the actual data to cache
                    var dataToCache = GetDataToCache(executedContext.Result);
                    if (dataToCache != null)
                    {
                        await cachedService.SetCacheResponseWithSerializAsync(
                            cacheKey,
                            dataToCache,
                            TimeSpan.FromSeconds(_timeToLiveSeconds));
                    }
                }
                catch (Exception ex)
                {
                    // Log cache error but don't break the request
                    var logger = context.HttpContext.RequestServices.GetService<ILogger<CacheAttribute>>();
                    logger?.LogError(ex, "Failed to cache response for key: {CacheKey}", cacheKey);
                }
            }
        }

        private bool ShouldCacheResponse(ActionExecutedContext context)
        {
            // Only cache successful GET responses
            if (context.Result is not ObjectResult objectResult)
                return false;

            var statusCode = objectResult.StatusCode ?? 200;
            return statusCode >= 200 && statusCode < 300;
        }

        private object? GetDataToCache(IActionResult result)
        {
            switch (result)
            {
                case ObjectResult objectResult:
                    // Handle ServiceResult<T>
                    if (objectResult.Value is ServiceResult serviceResult)
                    {
                        return serviceResult;
                    }
                    // Handle ServiceResult<T>
                    else if (objectResult.Value?.GetType().IsGenericType == true &&
                             objectResult.Value.GetType().GetGenericTypeDefinition() == typeof(ServiceResult<>))
                    {
                        return objectResult.Value;
                    }
                    // Handle any other successful result
                    else if ((objectResult.StatusCode ?? 200) >= 200 &&
                            (objectResult.StatusCode ?? 200) < 300)
                    {
                        return objectResult.Value;
                    }
                    break;

                case ContentResult contentResult:
                    if (contentResult.StatusCode >= 200 && contentResult.StatusCode < 300)
                    {
                        return contentResult.Content;
                    }
                    break;
            }

            return null;
        }

        private string GenerateCacheKeyFromRequest(HttpRequest request)
        {
            var cacheKey = new StringBuilder();

            // Include the request path
            cacheKey.Append(request.Path);

            // Include query parameters (sorted for consistency)
            if (request.Query.Any())
            {
                cacheKey.Append('?');
                var first = true;
                foreach (var (key, value) in request.Query.OrderBy(x => x.Key))
                {
                    if (!first) cacheKey.Append('&');
                    first = false;
                    cacheKey.Append(Uri.EscapeDataString(key));
                    cacheKey.Append('=');
                    cacheKey.Append(Uri.EscapeDataString(value.ToString()));
                }
            }

            var apiVersion = request.HttpContext.GetRequestedApiVersion()?.ToString();
            if (!string.IsNullOrEmpty(apiVersion))
            {
                cacheKey.Append($"|v:{apiVersion}");
            }

            return cacheKey.ToString();
        }
    }

    public interface IServiceResult
    {
        bool Success { get; }
        int StatusCode { get; }
        string? Message { get; }
    }

}