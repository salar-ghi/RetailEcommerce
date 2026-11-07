namespace Domain.Entities;

public class UserActivity : BaseModel<long>
{
    public string? UserId { get; set; }
    public string? SessionId { get; set; }
    public string Platform { get; set; } = "bazaar-azad";
    public string Action { get; set; } = string.Empty;
    public string? EntityType { get; set; }
    public string? EntityId { get; set; }
    public string? Path { get; set; }
    public string? Method { get; set; }
    public int? StatusCode { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public string? MetadataJson { get; set; }
}
