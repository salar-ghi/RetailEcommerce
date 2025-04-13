namespace Infrastructure.Repositories;

public class ProductTagRepository : Repository<ProductTag, int>, IProductTagRepository
{
    public ProductTagRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<ProductTag>> GetByProductIdAsync(int productId)
    {
        return await _context.ProductTags
            .Where(pt => pt.ProductId == productId)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<ProductTag>> GetByTagIdAsync(int tagId)
    {
        return await _context.ProductTags
            .Where(pt => pt.TagId == tagId)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task DeleteAsync(int productId, int tagId)
    {
        var entity = await _context.ProductTags
        .FirstOrDefaultAsync(pt => pt.ProductId == productId && pt.TagId == tagId);
        if (entity != null)
        {
            _context.ProductTags.Remove(entity);
        }
    }

}