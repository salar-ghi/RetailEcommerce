namespace Infrastructure.Repositories;

public class ProductVariantRepository : Repository<ProductVariant, long>, IProductVariantRepository
{
    public ProductVariantRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<ProductVariant>> GetByProductIdAsync(int productId)
    {
        return await _context.ProductVariants
            .Where(pv => pv.ProductId == productId)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<ProductVariant>> SearchByVariantNameAsync(string variantName)
    {
        return await _context.ProductVariants
            .Where(pv => pv.VariantName.Contains(variantName))
            .AsNoTracking()
            .ToListAsync();
    }
}