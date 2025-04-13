namespace Infrastructure.Repositories;

public class ProductImageRepository : Repository<ProductImage, long>, IProductImageRepository
{
    public ProductImageRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<ProductImage>> GetByProductIdAsync(int productId)
    {
        return await _context.ProductImages
            .Where(pi => pi.ProductId == productId)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<ProductImage>> SearchByPrimaryAsync(bool isPrimary)
    {
        return await _context.ProductImages
            .Where(pi => pi.IsPrimary == isPrimary)
            .AsNoTracking()
            .ToListAsync();
    }

}