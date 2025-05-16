namespace Application.Interfaces;

public interface ICurrentUserService
{
    string UserId { get; }
    IEnumerable<Claim> Claims { get; }
}
