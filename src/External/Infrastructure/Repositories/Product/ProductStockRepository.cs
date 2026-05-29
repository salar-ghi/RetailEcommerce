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
            .Include(ps => ps.Product)
            .Include(ps => ps.Space)
            .Include(ps => ps.Zone)
            .Include(ps => ps.Shelf)
            .Where(ps => ps.Quantity <= threshold || ps.Quantity - ps.ReservedQuantity <= ps.ReorderThreshold)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<ProductStock>> GetByProductLocationsAsync(long productId)
    {
        return await _context.ProductStocks
            .Include(ps => ps.Product)
            .Include(ps => ps.Space)
            .Include(ps => ps.Zone)
            .Include(ps => ps.Shelf)
            .Include(ps => ps.ProductInventoryBatch)
            .Include(ps => ps.ProductVariantOption)
            .Where(ps => ps.ProductId == productId)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<ProductStock>> GetByStorageScopeAsync(int? spaceId, int? zoneId, int? shelfId)
    {
        var query = _context.ProductStocks
            .Include(ps => ps.Product)
            .Include(ps => ps.Space)
            .Include(ps => ps.Zone)
            .Include(ps => ps.Shelf)
            .AsQueryable();

        if (spaceId.HasValue)
            query = query.Where(ps => ps.SpaceId == spaceId.Value);
        if (zoneId.HasValue)
            query = query.Where(ps => ps.ZoneId == zoneId.Value);
        if (shelfId.HasValue)
            query = query.Where(ps => ps.ShelfId == shelfId.Value);

        return await query.AsNoTracking().ToListAsync();
    }
}