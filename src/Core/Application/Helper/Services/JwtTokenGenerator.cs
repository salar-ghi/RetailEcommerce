namespace Application.Helper;

public class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly JwtSettings _jwtSettings;
    public JwtTokenGenerator(IOptions<JwtSettings> settings)
    {
        _jwtSettings = settings.Value ?? throw new ArgumentNullException(nameof(settings));
    }
    //private readonly IConfiguration;

    public string GenerateJwtToken(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.PhoneNumber),
        };

        //foreach (var role in user.UserRoles)
        //{
        //    claims.Add(new Claim(ClaimTypes.Role, role.Role));
        //}

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(30),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
