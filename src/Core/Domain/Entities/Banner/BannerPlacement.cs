namespace Domain.Entities;

public class BannerPlacement : BaseModel<int>
{
    public string Name { get; set; } = default!;
    public string Code { get; set; } = default!;
    public string? RecommendedSize { get; set; }

    public ICollection<Banner> Banners { get; set; } = new List<Banner>();
}
