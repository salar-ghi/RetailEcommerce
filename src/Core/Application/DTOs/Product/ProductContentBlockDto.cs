namespace Application.DTOs;

/// <summary>
/// API representation of one ordered block in a product introduction.
/// </summary>
public class ProductContentBlockDto
{
    public string? Id { get; set; }
    public string Type { get; set; } = null!;
    public string? Text { get; set; }
    public string? Image { get; set; }
    public string? Caption { get; set; }
    public int? SortOrder { get; set; }
}
