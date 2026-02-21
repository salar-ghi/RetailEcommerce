namespace Infrastructure.Repositories;

public class BannerPlacementMapRepository : Repository<BannerPlacementMap, int>, IBannerPlacementMapRepository
{
    public BannerPlacementMapRepository(AppDbContext context) : base(context) { }
}
