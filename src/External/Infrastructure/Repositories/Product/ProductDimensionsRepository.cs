namespace Infrastructure.Repositories;

public class ProductDimensionsRepository : Repository<ProductDimensions, int>, IProductDimensionsRepository
{
    public ProductDimensionsRepository(AppDbContext context) : base(context) { }

    public async Task<ProductDimensions> GetByProductIdAsync(int productId)
    {
        return await _context.ProductDimensions
            .Where(pd => pd.ProductId == productId)
            .AsNoTracking()
            .FirstOrDefaultAsync();
    }
}
