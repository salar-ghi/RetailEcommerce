namespace Infrastructure.Repositories;

public class StorageZoneRepository : Repository<StorageZone, int>, IStorageZoneRepository
{
    public StorageZoneRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<StorageZone>> GetBySpaceIdAsync(int spaceId)
    {
        return await _context.Set<StorageZone>()
            .Include(z => z.Space)
            .Include(z => z.Shelves.Where(s => !s.IsDeleted))
            .Where(z => z.SpaceId == spaceId && !z.IsDeleted && !z.Space.IsDeleted)
            .AsNoTracking()
            .ToListAsync();
    }
}
