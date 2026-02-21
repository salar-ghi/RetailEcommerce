namespace Domain.Entities;

public class BannerPlacementMap : BaseModel<int>
{
    public int BannerId { get; set; }
    public Banner Banner { get; set; }

    public int PlacementId { get; set; }
    public BannerPlacement Placement { get; set; }
}
