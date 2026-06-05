namespace Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{

    private readonly IUserService _userService;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public AuthController(IUserService userService, IJwtTokenGenerator jwtTokenGenerator)
    {
        _userService = userService;
        _jwtTokenGenerator = jwtTokenGenerator;
    }


    [HttpPost("signup")]
    public async Task<IActionResult> Signup(SignupDto model)
    {
        var result = await _userService.RegisterAsync(model);
        if (!result.IsSuccess)
        {
            return BadRequest(new ApiErrorResponse
            {
                StatusCode = StatusCodes.Status400BadRequest,
                ErrorCode = ErrorCodes.BadRequest,
                Message = result.Error,
                TraceId = HttpContext.TraceIdentifier,
                Path = HttpContext.Request.Path
            });
        }

        return Ok(ApiResponse<object>.Ok(new { result.Value }, "Signup completed successfully.", HttpContext.TraceIdentifier));
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto model)
    {
        var result = await _userService.AuthenticateAsync(model);
        if (!result.IsSuccess)
        {
            return Unauthorized(new ApiErrorResponse
            {
                StatusCode = StatusCodes.Status401Unauthorized,
                ErrorCode = ErrorCodes.Unauthorized,
                Message = result.ErrorMessage,
                TraceId = HttpContext.TraceIdentifier,
                Path = HttpContext.Request.Path
            });
        }

        return Ok(ApiResponse<object>.Ok(new { Token = result.JwtToken, RefreshToken = result.RefreshToken }, "Login completed successfully.", HttpContext.TraceIdentifier));
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshModel model)
    {
        if (model == null || string.IsNullOrWhiteSpace(model.RefreshToken))
        {
            return BadRequest(new ApiErrorResponse
            {
                StatusCode = StatusCodes.Status400BadRequest,
                ErrorCode = ErrorCodes.BadRequest,
                Message = "Refresh token is required.",
                TraceId = HttpContext.TraceIdentifier,
                Path = HttpContext.Request.Path
            });
        }

        var user = await _userService.ValidateRefreshTokenAsync(model.RefreshToken);
        if (user == null)
        {
            return Unauthorized(new ApiErrorResponse
            {
                StatusCode = StatusCodes.Status401Unauthorized,
                ErrorCode = ErrorCodes.Unauthorized,
                Message = "Invalid or expired refresh token.",
                TraceId = HttpContext.TraceIdentifier,
                Path = HttpContext.Request.Path
            });
        }

        var newToken = _jwtTokenGenerator.GenerateJwtToken(user);
        var newRefreshToken = await _userService.GenerateRefreshTokenAsync(user);
        return Ok(ApiResponse<object>.Ok(new { Token = newToken, RefreshToken = newRefreshToken }, "Session refreshed successfully.", HttpContext.TraceIdentifier));
    }
}
