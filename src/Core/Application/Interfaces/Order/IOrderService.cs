namespace Application.Interfaces;

public interface IOrderService
{
    Task<OrderDto> CreateOrderFromBasketAsync(string userId, ShippingAddressDto shippingAddress, string paymentMethod);
    Task<OrderDto> CreateManualOrderAsync(CreateManualOrderRequest request);
    Task<OrderDto> GetOrderAsync(string userId, string orderId);
    Task<OrderDto> GetOrderByIdAsync(string orderId);
    Task<IEnumerable<OrderDto>> GetUserOrdersAsync(string userId);
    Task<IEnumerable<OrderDto>> ListOrdersAsync();
    Task UpdateOrderStatusAsync(string orderId, OrderStatus status);
    Task CancelOrderAsync(string orderId);
    Task<IEnumerable<OrderDto>> GetOrdersByUserIdAsync(string userId);
    Task CreateReturnAsync(CreateReturnRequest request);
}
