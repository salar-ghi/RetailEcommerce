namespace Domain.Entities;

public class BannerPlacement : BaseModel<int>
{
    public string Name { get; set; } = default!;
    public BannerPageCode Code { get; set; } = default!;
    public string? RecommendedSize { get; set; }

    public ICollection<BannerPlacementMap> BannerPlacementMaps { get; set; }
        = new List<BannerPlacementMap>();
}
