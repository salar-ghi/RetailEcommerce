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
    public async Task<IActionResult> Signup(SignupDto model)
    {
        var result = await _userService.RegisterAsync(model);
        if (!result.IsSuccess)
        {
            return BadRequest(result.Error);
        }
        return Ok(new { result.Value });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto model)
    {
        var result = await _userService.AuthenticateAsync(model);
        if (!result.IsSuccess)
        {
            return Unauthorized(result.ErrorMessage); // Returns 401 with message (omit message in prod for security if desired)
        }

        //return Ok(new { Token = result.JwtToken, RefreshToken = result.RefreshToken });
        return Ok(new { Token = result.JwtToken });
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
