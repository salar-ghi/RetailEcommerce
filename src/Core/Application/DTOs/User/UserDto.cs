using System.Text.Json.Serialization;

namespace Application.DTOs;

public class UserDto
{
    public string Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }

    public List<string> Roles { get; set; } = new List<string>();
}

public class AddUserDto
{
    //public string Id { get; set; } = null!;

    [JsonPropertyName("firstName")]
    public string FirstName { get; set; } = null!;
    
    [JsonPropertyName("lastName")]
    public string LastName { get; set; } = null!;

    [JsonPropertyName("username")]
    public string UserName { get; set; } = null!;

    [JsonPropertyName("email")]
    public string Email { get; set; } = null!;
    
    [JsonPropertyName("phoneNumber")]
    public string PhoneNumber { get; set; } = null!;
    
    [JsonPropertyName("description")]
    public string Description { get; set; }

    //[JsonPropertyName("roles")]
    //public List<string> Roles { get; set; } = new();

    [JsonPropertyName("roleIds")]
    public List<int> RoleIds { get; set; } = new();

    [JsonPropertyName("isAdmin")]
    public bool IsAdmin { get; set; }
}