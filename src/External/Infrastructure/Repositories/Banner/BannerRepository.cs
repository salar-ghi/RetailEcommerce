using Domain.Enums;

namespace Infrastructure.Repositories;

public class BannerRepository : Repository<Banner, int>, IBannerRepository
{
    public BannerRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Banner>> GetActiveBannersByPlacementAsync(string placementCode)
    {
        var now = DateTime.UtcNow;
        return await _context.Banners
            .Include(b => b.Placements)
            .Where(b => !b.IsDeleted && b.IsActive &&
                        b.Placements.Any(p => p.Code == placementCode) &&
                        (b.StartDate == null || b.StartDate <= now) &&
                        (b.EndDate == null || b.EndDate >= now))
            .OrderByDescending(b => b.Priority)
            .ThenByDescending(b => b.CreatedTime)
            .ToListAsync();
    }

    public async Task<IEnumerable<Banner>> GetAllActiveAsync()
    {
        var now = DateTime.UtcNow;
        return await _context.Banners
            .Include(b => b.Placements)
            .Where(b => !b.IsDeleted && b.IsActive &&
                        (b.StartDate == null || b.StartDate <= now) &&
                        (b.EndDate == null || b.EndDate >= now))
            .OrderByDescending(b => b.Priority)
            .ToListAsync();
    }

    public async Task<Banner?> GetByIdWithPlacesAsync(int id)
    {
        return await _context.Banners
            .Include(b => b.Placements)
            .FirstOrDefaultAsync(b => b.Id == id && !b.IsDeleted);
    }

    public async Task<IEnumerable<Banner>> SearchAsync(string? name, BannerType? type)
    {
        var query = _context.Banners
            .Include(b => b.Placements)
            .Where(b => !b.IsDeleted);

        if (!string.IsNullOrEmpty(name))
            query = query.Where(b => b.Name.Contains(name));

        if (type.HasValue)
            query = query.Where(b => b.Type == type.Value);

        return await query.ToListAsync();
    }
}
