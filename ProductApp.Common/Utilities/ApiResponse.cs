namespace ProductApp.Common.Utilities
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public List<string>? Errors { get; set; }

        public static ApiResponse<T> Ok(T data, string message = "Success") =>
            new() { Success = true, Data = data, Message = message };

        public static ApiResponse<T> Created(T data, string message = "Created successfully") =>
            new() { Success = true, Data = data, Message = message };

        public static ApiResponse<T> NotFound(string message = "Not found") =>
            new() { Success = false, Message = message };

        public static ApiResponse<T> BadRequest(string message, List<string>? errors = null) =>
            new() { Success = false, Message = message, Errors = errors };

        public static ApiResponse<T> Unauthorized(string message = "Unauthorized") =>
            new() { Success = false, Message = message };
    }
}
