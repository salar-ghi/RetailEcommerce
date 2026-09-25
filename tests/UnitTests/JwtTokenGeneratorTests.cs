using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Application.Helper;
using Domain.Entities;
using Microsoft.Extensions.Options;
using Xunit;

namespace UnitTests;

public class JwtTokenGeneratorTests
{
    private readonly JwtTokenGenerator _jwtTokenGenerator;

    public JwtTokenGeneratorTests()
    {
        var jwtSettings = Options.Create(new JwtSettings
        {
            Key = "SuperSecretKeyForTestingJwtTokenGenerator123!",
            Issuer = "TestIssuer",
            Audience = "TestAudience",
            ExpirationMinutes = 60
        });

        _jwtTokenGenerator = new JwtTokenGenerator(jwtSettings);
    }

    [Fact]
    public void GenerateJwtToken_SingleRole_IncludesRoleClaim()
    {
        // Arrange
        var user = new User
        {
            Id = "user-123",
            PhoneNumber = "09123456789",
            UserRoles = new List<UserRole>
            {
                new UserRole
                {
                    Role = new Role { Name = "Manager" }
                }
            }
        };

        // Act
        var tokenString = _jwtTokenGenerator.GenerateJwtToken(user);

        // Assert
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(tokenString);

        var roleClaims = jwtToken.Claims.Where(c => c.Type == ClaimTypes.Role || c.Type == "role").Select(c => c.Value).ToList();
        Assert.Contains("Manager", roleClaims);
    }

    [Fact]
    public void GenerateJwtToken_MultipleRoles_IncludesAllRoleClaims()
    {
        // Arrange
        var user = new User
        {
            Id = "user-456",
            PhoneNumber = "09987654321",
            UserRoles = new List<UserRole>
            {
                new UserRole
                {
                    Role = new Role { Name = "Manager " } // Trimming test
                },
                new UserRole
                {
                    Role = new Role { Name = "Developer" }
                }
            }
        };

        // Act
        var tokenString = _jwtTokenGenerator.GenerateJwtToken(user);

        // Assert
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(tokenString);

        var roleClaims = jwtToken.Claims.Where(c => c.Type == ClaimTypes.Role || c.Type == "role").Select(c => c.Value).ToList();
        Assert.Equal(2, roleClaims.Count);
        Assert.Contains("Manager", roleClaims);
        Assert.Contains("Developer", roleClaims);
    }

    [Theory]
    [InlineData("Admin,Manager,Analytics,Developer", "Manager", true)]
    [InlineData("Admin,Manager,Analytics,Developer", "Analytics", true)]
    [InlineData("Admin,Manager,Accountant,Developer", "Manager", true)]
    [InlineData("Admin,Manager,Marketing,Developer", "Manager", true)]
    [InlineData("Admin, Manager, Analytics, Developer", "Manager", false)] // Leading space causes failure in standard matching
    public void RoleMatching_ValidatesExpectedResult(string rolesAttributeString, string userRole, bool expectedMatch)
    {
        // ASP.NET Core splits AuthorizeAttribute.Roles by comma
        var allowedRoles = rolesAttributeString.Split(',');

        var userHasPermission = allowedRoles.Any(role => role.Equals(userRole));

        Assert.Equal(expectedMatch, userHasPermission);
    }
}
