using System.Text.Json;

namespace VetLink.Services.Helper
{
    public class ServiceResult<T>
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }
        public int StatusCode { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public Dictionary<string, string[]>? Errors { get; set; }

        public static ServiceResult<T> Ok(T data, string message = "Success")
        {
            return new ServiceResult<T>
            {
                Success = true,
                Data = data,
                Message = message,
                StatusCode = 200
            };
        }

        public static ServiceResult<T> Ok(string message = "Success")
        {
            return new ServiceResult<T>
            {
                Success = true,
                Message = message,
                StatusCode = 200
            };
        }

        public static ServiceResult<T> Created(T data, string message = "Created successfully")
        {
            return new ServiceResult<T>
            {
                Success = true,
                Data = data,
                Message = message,
                StatusCode = 201
            };
        }

        public static ServiceResult<T> Fail(string message, int statusCode = 400, Dictionary<string, string[]>? errors = null)
        {
            return new ServiceResult<T>
            {
                Success = false,
                Message = message,
                StatusCode = statusCode,
                Errors = errors
            };
        }

        public static ServiceResult<T> NotFound(string message = "Resource not found")
        {
            return new ServiceResult<T>
            {
                Success = false,
                Message = message,
                StatusCode = 404
            };
        }

        public static ServiceResult<T> Unauthorized(string message = "Unauthorized access")
        {
            return new ServiceResult<T>
            {
                Success = false,
                Message = message,
                StatusCode = 401
            };
        }

        public static ServiceResult<T> Forbidden(string message = "Access forbidden")
        {
            return new ServiceResult<T>
            {
                Success = false,
                Message = message,
                StatusCode = 403
            };
        }

        public static ServiceResult<T> ValidationError(Dictionary<string, string[]> errors, string message = "Validation failed")
        {
            return new ServiceResult<T>
            {
                Success = false,
                Message = message,
                StatusCode = 422,
                Errors = errors
            };
        }

        public static ServiceResult<T> ServerError(string message = "Internal server error")
        {
            return new ServiceResult<T>
            {
                Success = false,
                Message = message,
                StatusCode = 500
            };
        }

        public static ServiceResult<T> Conflict(string message = "Conflict occurred")
        {
            return new ServiceResult<T>
            {
                Success = false,
                Message = message,
                StatusCode = 409
            };
        }

        // Helper methods
        public bool IsSuccess() => Success && StatusCode >= 200 && StatusCode < 300;
        public bool IsClientError() => !Success && StatusCode >= 400 && StatusCode < 500;
        public bool IsServerError() => !Success && StatusCode >= 500;

        public string ToJson(bool writeIndented = false)
        {
            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = writeIndented
            };

            return JsonSerializer.Serialize(this, jsonOptions);
        }
    }

    public class ServiceResult
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public int StatusCode { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public Dictionary<string, string[]>? Errors { get; set; }

        // Factory methods
        public static ServiceResult Ok(string message = "Success")
        {
            return new ServiceResult
            {
                Success = true,
                Message = message,
                StatusCode = 200
            };
        }

        public static ServiceResult Created(string message = "Created successfully")
        {
            return new ServiceResult
            {
                Success = true,
                Message = message,
                StatusCode = 201
            };
        }

        public static ServiceResult NoContent(string message = "No content")
        {
            return new ServiceResult
            {
                Success = true,
                Message = message,
                StatusCode = 204
            };
        }

        public static ServiceResult Fail(string message, int statusCode = 400, Dictionary<string, string[]>? errors = null)
        {
            return new ServiceResult
            {
                Success = false,
                Message = message,
                StatusCode = statusCode,
                Errors = errors
            };
        }

        public static ServiceResult NotFound(string message = "Resource not found")
            => Fail(message, 404);

        public static ServiceResult Unauthorized(string message = "Unauthorized access")
            => Fail(message, 401);

        public static ServiceResult Forbidden(string message = "Access forbidden")
            => Fail(message, 403);

        public static ServiceResult ServerError(string message = "Internal server error")
            => Fail(message, 500);

        public static ServiceResult Conflict(string message = "Conflict occurred")
            => Fail(message, 409);

        // Convert to generic result if needed
        public ServiceResult<T> ToGeneric<T>(T? data = default)
        {
            return new ServiceResult<T>
            {
                Success = Success,
                Message = Message,
                StatusCode = StatusCode,
                Data = data,
                Errors = Errors,
                Timestamp = Timestamp
            };
        }
    }
}