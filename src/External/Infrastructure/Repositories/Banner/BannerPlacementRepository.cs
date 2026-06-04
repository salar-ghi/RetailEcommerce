using Domain.Enums;

namespace Infrastructure.Repositories;

public class BannerPlacementRepository : Repository<BannerPlacement, int>, IBannerPlacementRepository
{
    public BannerPlacementRepository(AppDbContext context) : base(context) { }

    public async Task<BannerPlacement?> GetByCodeAsync(string code)
    {
        if (!Enum.TryParse<BannerPageCode>(code, ignoreCase: true, out var parsedCode))
            return null;

        return await _context.Set<BannerPlacement>()
            .FirstOrDefaultAsync(p => !p.IsDeleted && p.Code == parsedCode);
    }


    public async Task<List<BannerPlacement>> GetAllByIdsAsync(List<int> placementIds)
    {
        var result = await _context.Set<BannerPlacement>()
            .Where(x => !x.IsDeleted && placementIds.Contains(x.Id))
            .ToListAsync();

        return result;
    }
}
