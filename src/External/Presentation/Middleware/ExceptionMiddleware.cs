namespace Presentation.Middleware;

public sealed class ExceptionMiddleware
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;
    private readonly IHostEnvironment _environment;

    public ExceptionMiddleware(
        RequestDelegate next,
        ILogger<ExceptionMiddleware> logger,
        IHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (OperationCanceledException exception) when (context.RequestAborted.IsCancellationRequested)
        {
            _logger.LogWarning(exception,
                "Request was cancelled by the client. TraceId: {TraceId}, Method: {Method}, Path: {Path}",
                context.TraceIdentifier,
                context.Request.Method,
                context.Request.Path);
        }
        catch (Exception exception)
        {
            if (context.Response.HasStarted)
            {
                _logger.LogError(exception,
                    "Unhandled exception occurred after the response started. TraceId: {TraceId}, Method: {Method}, Path: {Path}",
                    context.TraceIdentifier,
                    context.Request.Method,
                    context.Request.Path);
                throw;
            }

            await HandleExceptionAsync(context, exception);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var error = CreateErrorResponse(context, exception);
        LogException(context, exception, error);

        context.Response.Clear();
        context.Response.StatusCode = error.StatusCode;
        context.Response.ContentType = "application/json";

        await context.Response.WriteAsync(JsonSerializer.Serialize(error, JsonOptions));
    }

    private ApiErrorResponse CreateErrorResponse(HttpContext context, Exception exception)
    {
        var (statusCode, errorCode, safeMessage) = exception switch
        {
            ValidationFailureException => (
                StatusCodes.Status400BadRequest,
                ErrorCodes.ValidationFailed,
                exception.Message),
            FluentValidation.ValidationException => (
                StatusCodes.Status400BadRequest,
                ErrorCodes.ValidationFailed,
                "The request contains validation errors."),
            NotFoundException => (
                StatusCodes.Status404NotFound,
                ErrorCodes.NotFound,
                exception.Message),
            ArgumentNullException => (
                StatusCodes.Status400BadRequest,
                ErrorCodes.BadRequest,
                "A required value is missing."),
            ArgumentException => (
                StatusCodes.Status400BadRequest,
                ErrorCodes.BadRequest,
                exception.Message),
            KeyNotFoundException => (
                StatusCodes.Status404NotFound,
                ErrorCodes.NotFound,
                exception.Message),
            UnauthorizedAccessException => (
                StatusCodes.Status403Forbidden,
                ErrorCodes.Forbidden,
                "You do not have permission to perform this action."),
            InvalidOperationException => (
                StatusCodes.Status409Conflict,
                ErrorCodes.Conflict,
                exception.Message),
            DbUpdateConcurrencyException => (
                StatusCodes.Status409Conflict,
                ErrorCodes.Conflict,
                "The resource was modified by another request. Reload it and try again."),
            DbUpdateException => (
                StatusCodes.Status503ServiceUnavailable,
                ErrorCodes.DatabaseFailure,
                "A database error occurred while processing the request."),
            HttpRequestException => (
                StatusCodes.Status503ServiceUnavailable,
                ErrorCodes.DependencyFailure,
                "A required downstream service is currently unavailable."),
            TaskCanceledException => (
                StatusCodes.Status504GatewayTimeout,
                ErrorCodes.DependencyFailure,
                "The request timed out while waiting for a dependency."),
            _ => (
                StatusCodes.Status500InternalServerError,
                ErrorCodes.UnexpectedError,
                "An unexpected error occurred while processing the request.")
        };

        return new ApiErrorResponse
        {
            StatusCode = statusCode,
            ErrorCode = errorCode,
            Message = safeMessage,
            Details = ShouldExposeDetails(statusCode) ? exception.ToString() : null,
            TraceId = context.TraceIdentifier,
            Path = context.Request.Path,
            Errors = exception switch
            {
                ValidationFailureException validationFailureException => validationFailureException.Errors,
                FluentValidation.ValidationException validationException => validationException.Errors
                    .GroupBy(error => error.PropertyName)
                    .ToDictionary(
                        group => group.Key,
                        group => group.Select(error => error.ErrorMessage).ToArray()),
                _ => null
            }
        };
    }

    private void LogException(HttpContext context, Exception exception, ApiErrorResponse error)
    {
        var logLevel = error.StatusCode >= StatusCodes.Status500InternalServerError
            ? LogLevel.Error
            : LogLevel.Warning;

        _logger.Log(logLevel,
            exception,
            "Request failed with {StatusCode} ({ErrorCode}). TraceId: {TraceId}, Method: {Method}, Path: {Path}",
            error.StatusCode,
            error.ErrorCode,
            error.TraceId,
            context.Request.Method,
            context.Request.Path);
    }

    private bool ShouldExposeDetails(int statusCode) =>
        _environment.IsDevelopment() || statusCode < StatusCodes.Status500InternalServerError;
}

public static class GlobalExceptionMiddlewareExtensions
{
    public static IApplicationBuilder UseGlobalExceptionHandling(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ExceptionMiddleware>();
    }
}
