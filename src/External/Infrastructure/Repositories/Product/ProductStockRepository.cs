namespace Infrastructure.Repositories;

public class ProductStockRepository : Repository<ProductStock, long>, IProductStockRepository
{
    public ProductStockRepository(AppDbContext context) : base(context) { }

    public async Task<ProductStock> GetByProductIdAsync(int productId)
    {
        return await _context.ProductStocks
            .Where(ps => ps.ProductId == productId)
            .AsNoTracking()
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<ProductStock>> SearchByLowStockAsync(int threshold)
    {
        return await _context.ProductStocks
            .Where(ps => ps.Quantity <= threshold)
            .AsNoTracking()
            .ToListAsync();
    }
}