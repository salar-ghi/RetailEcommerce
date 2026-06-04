namespace Presentation.Models;

public sealed class ApiErrorResponse
{
    public bool Success { get; init; } = false;
    public int StatusCode { get; init; }
    public string ErrorCode { get; init; } = "unexpected_error";
    public string Message { get; init; } = "An unexpected error occurred.";
    public string? Details { get; init; }
    public string TraceId { get; init; } = string.Empty;
    public string Path { get; init; } = string.Empty;
    public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.UtcNow;
    public IReadOnlyDictionary<string, string[]>? Errors { get; init; }
}
