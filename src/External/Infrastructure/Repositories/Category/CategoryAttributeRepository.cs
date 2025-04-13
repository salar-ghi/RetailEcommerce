namespace Infrastructure.Repositories;

public class CategoryAttributeRepository : Repository<CategoryAttribute, int>, ICategoryAttributeRepository
{
    public CategoryAttributeRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<CategoryAttribute>> GetByCategoryIdAsync(int categoryId)
    {
        return await _context.CategoryAttributes
            .Where(ca => ca.CategoryId == categoryId)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<CategoryAttribute>> SearchByKeyAsync(string key)
    {
        return await _context.CategoryAttributes
            .Where(ca => ca.Key.Contains(key))
            .AsNoTracking()
            .ToListAsync();
    }
}