namespace Infrastructure.Repositories;

public class BannerPlacementRepository : Repository<BannerPlacement, int>, IBannerPlacementRepository
{
    public BannerPlacementRepository(AppDbContext context) : base(context) { }

    public async Task<BannerPlacement?> GetByCodeAsync(string code)
    {
        return await _context.Set<BannerPlacement>()
            .FirstOrDefaultAsync(p => !p.IsDeleted);
    }


    public async Task<List<BannerPlacement>> GetAllByIdsAsync(List<int> placementIds)
    {
        var result = await _context.Set<BannerPlacement>()
            .Where(x => placementIds.Contains(x.Id))
            .ToListAsync();

        return result;
    }
}
