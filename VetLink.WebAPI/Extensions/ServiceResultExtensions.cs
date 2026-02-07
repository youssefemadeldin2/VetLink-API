using Microsoft.AspNetCore.Mvc;
using VetLink.WebApi.Helpers;
using VetLink.Services.Helper;

namespace VetLink.WebApi.Extensions 
{
	public static class ServiceResultExtensions
	{
		// Convert ServiceResult<T> to IActionResult using ResultActionResult<T>
		public static IActionResult ToActionResult<T>(this ServiceResult<T> result)
		{
			return new ResultActionResult<T>(result);
		}

		// Convert non-generic ServiceResult to IActionResult
		public static IActionResult ToActionResult(this ServiceResult result)
		{
			return new ResultActionResult(result);
		}

		// Helper methods for common results
		public static IActionResult ToOkResult<T>(this T data, string message = "Success")
			=> ServiceResult<T>.Ok(data, message).ToActionResult();

		public static IActionResult ToCreatedResult<T>(this T data, string message = "Created successfully")
			=> ServiceResult<T>.Created(data, message).ToActionResult();

		public static IActionResult ToNotFoundResult(string message = "Resource not found")
			=> ServiceResult<object>.NotFound(message).ToActionResult();

		public static IActionResult ToValidationErrorResult(Dictionary<string, string[]> errors, string message = "Validation failed")
			=> ServiceResult<object>.ValidationError(errors, message).ToActionResult();

		public static IActionResult ToServerErrorResult(string message = "Internal server error")
			=> ServiceResult<object>.ServerError(message).ToActionResult();

		// ControllerBase extension methods for cleaner syntax
		public static IActionResult OkResult<T>(this ControllerBase controller, T data, string message = "Success")
		{
			return ServiceResult<T>.Ok(data, message).ToActionResult();
		}

		public static IActionResult OkResult(this ControllerBase controller, string message = "Success")
		{
			return ServiceResult.Ok(message).ToActionResult();
		}

		public static IActionResult NotFoundResult(this ControllerBase controller, string message = "Resource not found")
		{
			return ServiceResult.NotFound(message).ToActionResult();
		}

		public static IActionResult BadRequestResult(this ControllerBase controller, string message)
		{
			return ServiceResult.Fail(message, StatusCodes.Status400BadRequest).ToActionResult();
		}

		//public static IActionResult BadRequestResult(this ControllerBase controller, Dictionary<string, string[]> errors, string message = "Validation failed")
		//{
		//	return ServiceResult.ValidationError(errors, message).ToActionResult();
		//}

		public static IActionResult ServerErrorResult(this ControllerBase controller, string message = "Internal server error")
		{
			return ServiceResult.ServerError(message).ToActionResult();
		}

		public static IActionResult UnauthorizedResult(this ControllerBase controller, string message = "Unauthorized access")
		{
			return ServiceResult.Unauthorized(message).ToActionResult();
		}

		public static IActionResult ForbiddenResult(this ControllerBase controller, string message = "Access forbidden")
		{
			return ServiceResult.Forbidden(message).ToActionResult();
		}

		public static IActionResult ConflictResult(this ControllerBase controller, string message = "Conflict occurred")
		{
			return ServiceResult.Conflict(message).ToActionResult();
		}
	}
}