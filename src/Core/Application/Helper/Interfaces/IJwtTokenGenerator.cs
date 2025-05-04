namespace Application.Helper;

public interface IJwtTokenGenerator
{
    string GenerateJwtToken(User user);
}