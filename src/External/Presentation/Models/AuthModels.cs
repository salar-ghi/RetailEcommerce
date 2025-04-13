namespace Presentation.Models;

public class AuthModels
{
}


public class SignupModel
{
    public string Username { get; set; }
    public string Password { get; set; }
    public string Role { get; set; }
}

public class LoginModel
{
    public string Username { get; set; }
    public string Password { get; set; }
}

public class RefreshModel
{
    public string RefreshToken { get; set; }
}