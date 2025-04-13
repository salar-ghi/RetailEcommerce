namespace Infrastructure.Repositories;

public class BrandRepository : Repository<Brand, int>, IBrandRepository
{
    public BrandRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Brand>> SearchByNameAsync(string name)
    {
        return await _context.Brands
            .Where(b => b.Name.Contains(name))
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<Brand>> SearchByDescriptionAsync(string description)
    {
        return await _context.Brands
            .Where(b => b.Description.Contains(description))
            .AsNoTracking()
            .ToListAsync();
    }
}