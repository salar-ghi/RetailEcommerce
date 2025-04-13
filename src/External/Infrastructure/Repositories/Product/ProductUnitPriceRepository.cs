namespace Infrastructure.Repositories;

public class ProductUnitPriceRepository : Repository<ProductUnitPrice, long>, IProductUnitPriceRepository
{
    public ProductUnitPriceRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<ProductUnitPrice>> GetByProductIdAsync(int productId)
    {
        return await _context.ProductUnitPrices
            .Where(pup => pup.ProductId == productId)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<ProductUnitPrice>> SearchByPriceRangeAsync(decimal minPrice, decimal maxPrice)
    {
        return await _context.ProductUnitPrices
            .Where(pup => pup.Price >= minPrice && pup.Price <= maxPrice)
            .AsNoTracking()
            .ToListAsync();
    }
}