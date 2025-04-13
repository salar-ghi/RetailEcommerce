namespace Application.Services;

public class OrderService : IOrderService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRedisCacheService _cacheService;
    private readonly IBasketService _basketService;
    private readonly IMapper _mapper;

    public OrderService(IUnitOfWork unitOfWork, 
        IBasketService basketService, 
        IMapper mapper,
        IRedisCacheService cacheService)
    {
        _unitOfWork = unitOfWork;
        _basketService = basketService;
        _cacheService = cacheService;
        _mapper = mapper;
    }

    public async Task<OrderDto> CreateOrderFromBasketAsync(string userId, ShippingAddressDto shippingAddress, string paymentMethod)
    {
        var basketDto = await _basketService.GetBasketAsync(userId);
        if (basketDto.Items.Count == 0)
        {
            throw new InvalidOperationException("Cannot create an order from an empty basket.");
        }

        var order = new Order
        {
            Id = Guid.NewGuid().ToString(),
            UserId = userId,
            CreatedTime = DateTime.UtcNow,
            Status = OrderStatus.Pending,
            ShippingAddress = _mapper.Map<ShippingAddress>(shippingAddress),
            //PaymentMethod = paymentMethod, ???
            Items = basketDto.Items.Select(item => new OrderItem
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice
            }).ToList()
        };

        await _unitOfWork.Orders.AddAsync(order);
        await _unitOfWork.SaveChangesAsync();

        // Clear the basket after order creation
        await _cacheService.SetCachedDataAsync<Order>(order.Id, order, TimeSpan.FromHours(3));
        await _basketService.ClearBasketAsync(userId);
        return _mapper.Map<OrderDto>(order);
    }

    public async Task<OrderDto> GetOrderAsync(string userId, string orderId)
    {
        var cachedOrder = await _cacheService.GetCachedDataAsync<Order>(orderId);
        if (cachedOrder != null && cachedOrder.UserId == userId)
        {
            return _mapper.Map<OrderDto>(cachedOrder);
        }

        var order = await _unitOfWork.Orders.GetByUserIdAndOrderIdAsync(userId, orderId);
        if (order == null)
        {
            throw new KeyNotFoundException($"Order with ID {orderId} for user {userId} not found.");
        }
        
        await _cacheService.SetCachedDataAsync<Order>(order.Id, order, TimeSpan.FromHours(3));
        return _mapper.Map<OrderDto>(order);
    }

    public async Task<IEnumerable<OrderDto>> GetUserOrdersAsync(string userId)
    {
        var orders = await _unitOfWork.Orders.GetByUserIdAsync(userId);
        return _mapper.Map<IEnumerable<OrderDto>>(orders);
    }

    public async Task UpdateOrderStatusAsync(string orderId, OrderStatus status)
    {
        var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
        if (order == null)
        {
            throw new KeyNotFoundException($"Order with ID {orderId} not found.");
        }

        order.Status = status;
        if (status == OrderStatus.Shipped) order.ShippingDate = DateTime.UtcNow;
        if (status == OrderStatus.Delivered || status == OrderStatus.Cancelled) order.PaymentDate ??= DateTime.UtcNow;

        await _unitOfWork.Orders.UpdateAsync(order);
        await _unitOfWork.SaveChangesAsync();
        await _cacheService.SetCachedDataAsync<Order>(order.Id, order, TimeSpan.FromHours(3));
    }

    public async Task CancelOrderAsync(string orderId)
    {
        var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
        if (order == null)
        {
            throw new KeyNotFoundException($"Order with ID {orderId} not found.");
        }

        if (order.Status == OrderStatus.Shipped || order.Status == OrderStatus.Delivered)
        {
            throw new InvalidOperationException("Cannot cancel a shipped or delivered order.");
        }

        order.Status = OrderStatus.Cancelled;
        await _unitOfWork.Orders.UpdateAsync(order);
        await _unitOfWork.SaveChangesAsync();
        await _cacheService.RemoveCachedDataAsync(order.Id);
    }

    public async Task<IEnumerable<OrderDto>> GetOrdersByUserIdAsync(string userId)
    {
        var orders = await _unitOfWork.Orders.GetByUserIdAsync(userId);
        return _mapper.Map<IEnumerable<OrderDto>>(orders);
    }

}