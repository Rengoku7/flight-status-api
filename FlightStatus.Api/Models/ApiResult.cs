namespace FlightStatus.Api.Models;

public class ApiResult<T>
{
    public bool Success { get; set; }
    public string? Title { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }
    public List<string>? Errors { get; set; }

    public static ApiResult<T> SuccessResult(T data, string? title = null, string? message = null) => new()
    {
        Success = true,
        Title = title,
        Data = data,
        Message = message
    };

    public static ApiResult<T> FailureResult(string title, string? message = null, List<string>? errors = null) => new()
    {
        Success = false,
        Title = title,
        Message = message,
        Errors = errors
    };
}

public class ApiResult
{
    public bool Success { get; set; }
    public string? Title { get; set; }
    public string? Message { get; set; }
    public List<string>? Errors { get; set; }

    public static ApiResult SuccessResult(string? title = null, string? message = null) => new()
    {
        Success = true,
        Title = title,
        Message = message
    };

    public static ApiResult FailureResult(string title, string? message = null, List<string>? errors = null) => new()
    {
        Success = false,
        Title = title,
        Message = message,
        Errors = errors
    };
}
