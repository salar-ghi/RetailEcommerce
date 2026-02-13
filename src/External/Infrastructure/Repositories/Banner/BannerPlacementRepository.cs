namespace Infrastructure.Repositories;

public class BannerPlacementRepository : Repository<BannerPlacement, int>, IBannerPlacementRepository
{
    public BannerPlacementRepository(AppDbContext context) : base(context)
    {
    }

    public Task<BannerPlacement?> GetByCodeAsync(string code)
    {
        throw new NotImplementedException();
    }
}
