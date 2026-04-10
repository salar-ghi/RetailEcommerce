namespace Infrastructure.Repositories;

public class OrderRepository: Repository<Order, string>, IOrderRepository
{
    public OrderRepository(AppDbContext context) : base(context) { }

    public async Task<Order> GetByUserIdAndOrderIdAsync(string userId, string orderId)
    {
        var result = await _context.Orders
            .AsNoTracking()
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(o => o.CustomerId == userId && o.Id == orderId);
        return result;
    }

    public async Task<IEnumerable<Order>> GetByUserIdAsync(string userId)
    {
        return await _context.Orders
            .AsNoTracking()
            .Where(o => o.CustomerId == userId)
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .ToListAsync();
    }

    public async Task<Order> GetOrderWithItemsAsync(string orderId)
    {
        return await _context.Orders
            .Where(o => o.Id == orderId)
            .AsNoTracking()
            .Include(o => o.Items)
            .ThenInclude(oi => oi.Product)
            .FirstOrDefaultAsync();
    }

}