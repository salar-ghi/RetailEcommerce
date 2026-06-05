namespace Application.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private static readonly string[] UserIdClaimTypes =
    {
        ClaimTypes.NameIdentifier,
        JwtRegisteredClaimNames.Sub,
        "sub",
        "userId",
        "uid"
    };

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    }

    public string UserId => GetUserId() ?? string.Empty;

    public IEnumerable<Claim> Claims => _httpContextAccessor.HttpContext?.User?.Claims ?? Enumerable.Empty<Claim>();

    private string? GetUserId()
    {
        var user = _httpContextAccessor.HttpContext?.User;

        if (user is null)
        {
            return null;
        }

        foreach (var claimType in UserIdClaimTypes)
        {
            var value = user.FindFirst(claimType)?.Value;

            if (!string.IsNullOrWhiteSpace(value))
            {
                return value;
            }
        }

        return null;
    }

    public string GenerateRandomPassword()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, 8)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
}
