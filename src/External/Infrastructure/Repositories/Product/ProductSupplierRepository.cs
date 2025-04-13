namespace Infrastructure.Repositories;

public class ProductSupplierRepository : Repository<ProductSupplier, int>, IProductSupplierRepository
{
    public ProductSupplierRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<ProductSupplier>> GetByProductIdAsync(int productId)
    {
        return await _context.ProductSuppliers
            .Where(ps => ps.ProductId == productId)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<ProductSupplier>> GetBySupplierIdAsync(int supplierId)
    {
        return await _context.ProductSuppliers
            .Where(ps => ps.SupplierId == supplierId)
            .AsNoTracking()
            .ToListAsync();
    }
    public async Task DeleteAsync(int productId, int supplierId)
    {
        var entity = await _context.ProductSuppliers
        .FirstOrDefaultAsync(ps => ps.ProductId == productId && ps.SupplierId == supplierId);
        if (entity != null)
        {
            _context.ProductSuppliers.Remove(entity);
        }
    }
}
