namespace Application.Common;

public class AuthResult
{
    public bool IsSuccess { get; set; }
    public string JwtToken { get; set; }
    public string RefreshToken { get; set; }
    public string ErrorMessage { get; set; }
    public DTOs.UserDto User { get; set; }

    // Optional: Factory methods for clarity
    public static AuthResult Success(string jwtToken, string refreshToken, DTOs.UserDto user = null)
    {
        return new AuthResult { IsSuccess = true, JwtToken = jwtToken, RefreshToken = refreshToken, User = user };
    }

    public static AuthResult Failure(string errorMessage)
    {
        return new AuthResult { IsSuccess = false, ErrorMessage = errorMessage };
    }
}
