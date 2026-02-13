namespace Application.DTOs;

public record BannerDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string? Link { get; set; }
    public string? CallToActionText { get; set; }
    public BannerType Type { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsActive { get; set; }
    public int DisplayOrder { get; set; }
    public List<string> PlacementCodes { get; set; } = new();
}

public class CreateBannerDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string? AltText { get; set; }
    public string? Link { get; set; }
    public string? CallToActionText { get; set; }
    public BannerType Type { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsActive { get; set; } = true;
    public int DisplayOrder { get; set; } = 0;
    public List<int> PlacementIds { get; set; } = new();   // required
}

public class UpdateBannerDto : CreateBannerDto { public int Id { get; set; } }