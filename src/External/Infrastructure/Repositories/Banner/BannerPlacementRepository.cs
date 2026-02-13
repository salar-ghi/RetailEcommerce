namespace Infrastructure.Repositories;

public class BannerPlacementRepository : Repository<BannerPlacement, int>, IBannerPlacementRepository
{
    public BannerPlacementRepository(AppDbContext context) : base(context) { }

    public async Task<BannerPlacement?> GetByCodeAsync(string code)
    {
        return await _context.Set<BannerPlacement>()
            .FirstOrDefaultAsync(p => p.Code == code && !p.IsDeleted);
    }
}
