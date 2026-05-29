using System.Text.Json.Serialization;

namespace Application.DTOs;

public record BrandDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Logo { get; set; }
    public DateTime CreatedTime { get; set; }
    public List<BrandCategories> Categories { get; set; } = [];

    [JsonPropertyName("categoryIds")]
    public List<int> CategoryIds { get; set; } = [];
}

public class BrandCategories
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public record BrandUpdateDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Logo { get; set; }
    public DateTime ModifiedBy { get; set; } = DateTime.Now;
}
