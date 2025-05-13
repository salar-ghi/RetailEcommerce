namespace Infrastructure.Repositories;

public class CategoryRepository : Repository<Category, int>, ICategoryRepository
{
    public CategoryRepository(AppDbContext context) : base(context) { }


    public async Task GetCategoriesWithProductAndBrand()
    {
        var result = await _context.Categories
            .AsNoTracking()
            .Select(c => new 
            { 
                categoryId = c.Id,
                categoryName = c.Name,
                productCount = c.Products.Count(),
                Brands = c.Products.Select(p => new
                {
                    brandId = p.Brand.Id,
                    brandName = p.Brand.Name,
                }).Distinct().ToList()
            }).ToListAsync();
    }
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