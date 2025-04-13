using Presentation.Models;

namespace Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{

    private readonly IUserService _userService;
    private readonly JwtHelper _jwtHelper;

    public AuthController(IUserService userService, IOptions<JwtSettings> jwtSettings)
    {
        _userService = userService;
        _jwtHelper = new JwtHelper(jwtSettings);
    }


    [HttpPost("signup")]
    public async Task<IActionResult> Signup([FromBody] SignupModel model)
    {
        try
        {
            var user = await _userService.RegisterAsync(model.Username, model.Password, model.Role);
            return Ok(new { Message = "User registered successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        var user = await _userService.AuthenticateAsync(model.Username, model.Password);
        if (user == null)
        {
            return Unauthorized(new { Message = "Invalid username or password" });
        }

        var token = _jwtHelper.GenerateToken(user);
        var refreshToken = await _userService.GenerateRefreshTokenAsync(user);
        return Ok(new { Token = token, RefreshToken = refreshToken });
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshModel model)
    {
        var user = await _userService.ValidateRefreshTokenAsync(model.RefreshToken);
        if (user == null)
        {
            return Unauthorized(new { Message = "Invalid refresh token" });
        }

        var newToken = _jwtHelper.GenerateToken(user);
        var newRefreshToken = await _userService.GenerateRefreshTokenAsync(user);
        return Ok(new { Token = newToken, RefreshToken = newRefreshToken });
    }


    //[HttpPost("login")]
    //public async Task<IActionResult> Login([FromBody] LoginRequest request)
    //{
    //    var token = await _authService.Authenticate(request.UsernameOrPhone, request.Password);
    //    if (string.IsNullOrEmpty(token)) return Unauthorized();

    //    return Ok(new { token });
    //}
}
