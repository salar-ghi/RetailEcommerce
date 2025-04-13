namespace Domain.IRepositories;

public interface IOrderRepository : IRepository<Order, string>
{
    Task<Order> GetByUserIdAndOrderIdAsync(string userId, string orderId);
    Task<IEnumerable<Order>> GetByUserIdAsync(string userId);
    Task<Order> GetOrderWithItemsAsync(string orderId);
    
    
    //Task AddPaymentAsync(Payment payment);
}
