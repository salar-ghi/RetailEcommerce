namespace Infrastructure.Repositories;

public class SupplierRepository : Repository<Supplier, int>, ISupplierRepository
{
    public SupplierRepository(AppDbContext context) : base(context) { }
    public async Task<IEnumerable<Supplier>> SearchByNameAsync(string name)
    {
        return await _context.Suppliers
            .Where(s => s.Name.Contains(name))
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<Supplier>> SearchByContactInfoAsync(string contactInfo)
    {
        return await _context.Suppliers
            .Where(s => s.Info.Contains(contactInfo))
            .AsNoTracking()
            .ToListAsync();
    }
}
