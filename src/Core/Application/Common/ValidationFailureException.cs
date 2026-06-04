namespace Application.Common;

public sealed class ValidationFailureException : ApplicationLayerException
{
    public ValidationFailureException(string message, IReadOnlyDictionary<string, string[]> errors) : base(message)
    {
        Errors = errors;
    }

    public IReadOnlyDictionary<string, string[]> Errors { get; }
}
