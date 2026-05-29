namespace Infrastructure.Repositories;

public class StorageSpaceRepository : Repository<StorageSpace, int>, IStorageSpaceRepository
{
    public StorageSpaceRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<StorageSpace>> SearchAsync(string searchTerm)
    {
        searchTerm ??= string.Empty;
        return await _context.Set<StorageSpace>()
            .Include(s => s.Zones)
            .Include(s => s.Shelves)
            .Where(s => s.Name.Contains(searchTerm) || (s.Code ?? string.Empty).Contains(searchTerm) || (s.Address ?? string.Empty).Contains(searchTerm))
            .AsNoTracking()
            .ToListAsync();
    }
}
