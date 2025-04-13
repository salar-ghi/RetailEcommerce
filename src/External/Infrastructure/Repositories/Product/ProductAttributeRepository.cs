namespace Infrastructure.Repositories;

public class ProductAttributeRepository : Repository<ProductAttribute, long>, IProductAttributeRepository
{
    public ProductAttributeRepository(AppDbContext context) : base(context) { }


    public async Task<IEnumerable<ProductAttribute>> GetByProductIdAsync(int productId)
    {
        return await _context.ProductAttributes
            .Where(pa => pa.ProductId == productId)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<ProductAttribute>> SearchByKeyAsync(string key)
    {
        return await _context.ProductAttributes
            .Where(pa => pa.Key.Contains(key))
            .AsNoTracking()
            .ToListAsync();
    }

}
