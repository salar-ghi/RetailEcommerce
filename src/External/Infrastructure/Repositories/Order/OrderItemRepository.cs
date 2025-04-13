namespace Infrastructure.Repositories;

public class OrderItemRepository : Repository<OrderItem, int>, IOrderItemRepository
{
    public OrderItemRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<OrderItem>> GetByOrderIdAsync(string orderId)
    {
        return await _context.OrderItems
            .Where(oi => oi.OrderId == orderId)
            .Include(oi => oi.Product)
            .ToListAsync();
    }
}