namespace Domain.IRepositories;

public interface IOrderItemRepository : IRepository<OrderItem, int>
{
    Task<IEnumerable<OrderItem>> GetByOrderIdAsync(string orderId);
}
