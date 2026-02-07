using Microsoft.AspNetCore.Mvc;
using VetLink.Services.Helper;

namespace VetLink.WebApi.Helpers
{
    public class ServiceResultActionResult<T> : IActionResult
    {
        private readonly ServiceResult<T> _serviceResult;

        public ServiceResultActionResult(ServiceResult<T> serviceResult)
        {
            _serviceResult = serviceResult;
        }

        public async Task ExecuteResultAsync(ActionContext context)
        {
            var objectResult = new ObjectResult(_serviceResult)
            {
                StatusCode = _serviceResult.StatusCode
            };

            await objectResult.ExecuteResultAsync(context);
        }
    }

    // Add non-generic version for consistency
    public class ServiceResultActionResult : IActionResult
    {
        private readonly ServiceResult _serviceResult;

        public ServiceResultActionResult(ServiceResult serviceResult)
        {
            _serviceResult = serviceResult;
        }

        public async Task ExecuteResultAsync(ActionContext context)
        {
            var objectResult = new ObjectResult(_serviceResult)
            {
                StatusCode = _serviceResult.StatusCode
            };

            await objectResult.ExecuteResultAsync(context);
        }
    }
}