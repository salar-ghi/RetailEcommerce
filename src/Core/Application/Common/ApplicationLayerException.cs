namespace Application.Common;

public class ApplicationLayerException : Exception
{
    public ApplicationLayerException(string message) : base(message)
    {
    }

    public ApplicationLayerException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
