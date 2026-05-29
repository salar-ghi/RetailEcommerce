namespace Infrastructure.Repositories;

public class ShelfRepository : Repository<Shelf, int>, IShelfRepository
{
    public ShelfRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Shelf>> GetBySpaceIdAsync(int spaceId)
    {
        return await _context.Set<Shelf>()
            .Include(s => s.Space)
            .Include(s => s.Zone)
            .Where(s => s.SpaceId == spaceId)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<Shelf>> GetByZoneIdAsync(int zoneId)
    {
        return await _context.Set<Shelf>()
            .Include(s => s.Space)
            .Include(s => s.Zone)
            .Where(s => s.ZoneId == zoneId)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Shelf> GetByCodeAsync(string code)
    {
        return await _context.Set<Shelf>()
            .Include(s => s.Space)
            .Include(s => s.Zone)
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Code == code);
    }
}
