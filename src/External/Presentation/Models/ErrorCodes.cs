namespace Presentation.Models;

public static class ErrorCodes
{
    public const string ValidationFailed = "validation_failed";
    public const string BadRequest = "bad_request";
    public const string Unauthorized = "unauthorized";
    public const string Forbidden = "forbidden";
    public const string NotFound = "not_found";
    public const string Conflict = "conflict";
    public const string DependencyFailure = "dependency_failure";
    public const string DatabaseFailure = "database_failure";
    public const string RequestCancelled = "request_cancelled";
    public const string UnexpectedError = "unexpected_error";
}
