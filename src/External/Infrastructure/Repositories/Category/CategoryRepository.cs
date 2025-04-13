namespace Infrastructure.Repositories;

public class CategoryRepository : Repository<Category, int>, ICategoryRepository
{
    public CategoryRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Category>> SearchByNameAsync(string name)
    {
        return await _context.Categories
            .Where(c => c.Name.Contains(name))
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<Category>> SearchByDescriptionAsync(string description)
    {
        return await _context.Categories
            .Where(c => c.Description.Contains(description))
            .AsNoTracking()
            .ToListAsync();
    }
}