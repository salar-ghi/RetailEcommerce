namespace Presentation.Models;

public sealed class ApiResponse<T>
{
    public bool Success { get; init; }
    public string? Message { get; init; }
    public T? Data { get; init; }
    public string TraceId { get; init; } = string.Empty;

    public static ApiResponse<T> Ok(T? data, string? message, string traceId) => new()
    {
        Success = true,
        Message = message,
        Data = data,
        TraceId = traceId
    };
}
