using Microsoft.AspNetCore.Mvc;
using VetLink.Services.Helper;

namespace VetLink.WebApi.Helpers
{
    public class ResultActionResult<T> : IActionResult
    {
        private readonly ServiceResult<T> _result;

        public ResultActionResult(ServiceResult<T> result)
        {
            _result = result;
        }

        public async Task ExecuteResultAsync(ActionContext context)
        {
            var objectResult = new ObjectResult(_result)
            {
                StatusCode = _result.StatusCode
            };

            await objectResult.ExecuteResultAsync(context);
        }
    }

    public class ResultActionResult : IActionResult
    {
        private readonly ServiceResult _result;

        public ResultActionResult(ServiceResult result)
        {
            _result = result;
        }

        public async Task ExecuteResultAsync(ActionContext context)
        {
            var objectResult = new ObjectResult(_result)
            {
                StatusCode = _result.StatusCode
            };

            await objectResult.ExecuteResultAsync(context);
        }
    }
}