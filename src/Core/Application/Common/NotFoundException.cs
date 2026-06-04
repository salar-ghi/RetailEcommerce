namespace Application.Common;

public sealed class NotFoundException : ApplicationLayerException
{
    public NotFoundException(string message) : base(message)
    {
    }
}
