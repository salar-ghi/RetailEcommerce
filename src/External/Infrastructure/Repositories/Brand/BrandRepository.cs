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

    public async Task<IEnumerable<Brand>> GetAllWithCategoryAsync()
    {
        return await _context.Brands
            .Include(z => z.BrandCategories)
            .ThenInclude(x => x.Category)
            .Where(z => z.IsDeleted == false)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Brand?> GetByIdWithCategoryAsync(int id, bool asNoTracking = false)
    {
        IQueryable<Brand> query = _context.Brands
            .Include(brand => brand.BrandCategories)
            .ThenInclude(brandCategory => brandCategory.Category);

        if (asNoTracking)
        {
            query = query.AsNoTracking();
        }

        return await query.FirstOrDefaultAsync(brand => brand.Id == id && !brand.IsDeleted);
    }

    public async Task<bool> ExistsByNameAsync(string normalizedName, int? excludedId = null)
    {
        return await _context.Brands
            .AsNoTracking()
            .AnyAsync(brand =>
                !brand.IsDeleted &&
                (excludedId == null || brand.Id != excludedId.Value) &&
                brand.Name.Trim().ToLower() == normalizedName);
    }
}
