using Presentation.Models;

namespace Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{

    private readonly IUserService _userService;

    public AuthController(IUserService userService)
    {
        _userService = userService;
    }


    [HttpPost("signup")]
    public async Task<IActionResult> Signup([FromBody] SignupDto model)
    {
        try
        {
            var user = await _userService.RegisterAsync(model);
            return Ok(new { Message = "User registered successfully", UserId = user.Id });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto model)
    {
        try
        {
            var (jwtToken, refreshToken) = await _userService.AuthenticateAsync(model);
            return Ok(new { JwtToken = jwtToken, RefreshToken = refreshToken });
        }
        catch (Exception ex)
        {
            return Unauthorized(ex.Message);
        }
    }

    //[HttpPost("refresh")]
    //public async Task<IActionResult> Refresh([FromBody] RefreshModel model)
    //{
    //    var user = await _userService.ValidateRefreshTokenAsync(model.RefreshToken);
    //    if (user == null)
    //    {
    //        return Unauthorized(new { Message = "Invalid refresh token" });
    //    }

    //    var newToken = _jwtHelper.GenerateToken(user);
    //    var newRefreshToken = await _userService.GenerateRefreshTokenAsync(user);
    //    return Ok(new { Token = newToken, RefreshToken = newRefreshToken });
    //}
}
