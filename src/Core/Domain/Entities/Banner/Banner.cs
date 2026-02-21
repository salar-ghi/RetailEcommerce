namespace Domain.Entities;

public class Banner : BaseModel<int>
{
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public string ImageUrl { get; set; } = default!;
    public string? AltText { get; set; }
    public string? Link { get; set; }
    public string? CallToActionText { get; set; }
    public BannerType Type { get; set; } = BannerType.Advertisement;

    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    public BannerSize Size { get; set; }
    public int Priority { get; set; } = 0;
    public int ClickCount { get; set; } = 0; 
    public int ViewCount { get; set; } = 0;
    public bool IsActive { get; set; } = true;
    public ICollection<BannerPlacementMap> BannerPlacementMaps { get; set; }
        = new List<BannerPlacementMap>();
}
