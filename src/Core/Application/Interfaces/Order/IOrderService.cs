namespace Application.Interfaces;

public interface IOrderService
{
    Task<OrderDto> CreateOrderFromBasketAsync(string userId, ShippingAddressDto shippingAddress, string paymentMethod);
    Task<OrderDto> GetOrderAsync(string userId, string orderId);
    Task<IEnumerable<OrderDto>> GetUserOrdersAsync(string userId);
    Task UpdateOrderStatusAsync(string orderId, OrderStatus status);
    Task CancelOrderAsync(string orderId);
    Task<IEnumerable<OrderDto>> GetOrdersByUserIdAsync(string userId);

}