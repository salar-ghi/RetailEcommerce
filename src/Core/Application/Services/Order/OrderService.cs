namespace Application.Services;

public class OrderService : IOrderService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRedisCacheService _cacheService;
    private readonly IBasketService _basketService;
    private readonly IMapper _mapper;
    private readonly IFinanceService _financeService;

    public OrderService(IUnitOfWork unitOfWork,
        IBasketService basketService,
        IMapper mapper,
        IRedisCacheService cacheService,
        IFinanceService financeService)
    {
        _unitOfWork = unitOfWork;
        _basketService = basketService;
        _cacheService = cacheService;
        _mapper = mapper;
        _financeService = financeService;
    }

    public async Task<OrderDto> CreateOrderFromBasketAsync(string userId, ShippingAddressDto shippingAddress, string paymentMethod)
    {
        var basketDto = await _basketService.GetBasketAsync(userId);
        if (basketDto.Items.Count == 0) throw new InvalidOperationException("Cannot create an order from an empty basket.");

        var method = ParsePaymentMethod(paymentMethod);
        var total = basketDto.Items.Sum(item => item.Quantity * item.UnitPrice);
        var order = new Order
        {
            Id = Guid.NewGuid().ToString(), CustomerId = userId, CreatedTime = DateTime.UtcNow,
            Status = OrderStatus.Pending, Source = OrderSource.Storefront,
            ShippingAddress = _mapper.Map<ShippingAddress>(shippingAddress),
            Items = basketDto.Items.Select(item => new OrderItem { ProductId = item.ProductId, Quantity = item.Quantity, UnitPrice = item.UnitPrice }).ToList(),
            Payments = new List<Payment> { new() { Id = Guid.NewGuid().ToString(), Amount = total, Method = method, Status = PaymentStatus.Pending, PaymentDate = DateTime.UtcNow } }
        };

        await _unitOfWork.Orders.AddAsync(order);
        await _unitOfWork.SaveChangesAsync();
        await _cacheService.SetCachedDataAsync(order.Id, order, TimeSpan.FromHours(3));
        await _basketService.ClearBasketAsync(userId);
        return _mapper.Map<OrderDto>(order);
    }

    public async Task<OrderDto> CreateManualOrderAsync(CreateManualOrderRequest request)
    {
        if (request.Items.Count == 0) throw new InvalidOperationException("Cannot create an order without items.");
        var customerId = await ResolveCustomerAsync(request);
        var subtotal = request.Items.Sum(i => i.TotalPrice > 0 ? i.TotalPrice : i.Quantity * i.UnitPrice);
        var finalTotal = request.FinalTotal > 0 ? request.FinalTotal : subtotal - request.DiscountAmount;
        var paid = request.Payments.Where(p => !IsCredit(p.Method)).Sum(p => p.Amount);

        var order = new Order
        {
            Id = Guid.NewGuid().ToString(), 
            CustomerId = customerId, 
            CreatedTime = DateTime.UtcNow,
            Status = paid >= finalTotal ? OrderStatus.Processing : OrderStatus.Pending,
            Source = OrderSource.AdminManual, 
            DiscountAmount = request.DiscountAmount, 
            Notes = request.Notes,
            ShippingAddress = request.ShippingAddress is not null ? _mapper.Map<ShippingAddress>(request.ShippingAddress) : new ShippingAddress { AddressLine1 = request.CustomerAddress },
            Items = request.Items.Select(i => new OrderItem { ProductId = i.ProductId, Quantity = i.Quantity, UnitPrice = i.UnitPrice, DiscountedPrice = 0, SaleUnit = i.SaleUnit, WeightUnit = i.WeightUnit, SpaceId = i.SpaceId, SpaceName = i.SpaceName, ZoneId = i.ZoneId, ZoneName = i.ZoneName, ShelfId = i.ShelfId, ShelfCode = i.ShelfCode }).ToList(),
            Payments = request.Payments.Select(p => new Payment { Id = Guid.NewGuid().ToString(), Amount = p.Amount, Method = MapPaymentMethod(p.Method), Status = p.Status, TransactionId = p.GatewayTxnId, DueDate = p.DueDate, FinanceAccountId = p.FinanceAccountId, BranchId = p.BranchId, PaymentDate = DateTime.UtcNow }).ToList()
        };

        await _unitOfWork.Orders.AddAsync(order);
        await _unitOfWork.SaveChangesAsync();
        await SyncPaymentsToFinanceAsync(order, request.Payments);
        await _cacheService.SetCachedDataAsync(order.Id, order, TimeSpan.FromHours(3));
        return _mapper.Map<OrderDto>(order);
    }

    public async Task<OrderDto> GetOrderByIdAsync(string orderId) => _mapper.Map<OrderDto>(await LoadOrderAsync(orderId));

    public async Task<OrderDto> GetOrderAsync(string userId, string orderId)
    {
        var order = await _unitOfWork.Orders.GetByUserIdAndOrderIdAsync(userId, orderId) ?? throw new KeyNotFoundException($"Order with ID {orderId} for user {userId} not found.");
        return _mapper.Map<OrderDto>(order);
    }

    public async Task<IEnumerable<OrderDto>> GetUserOrdersAsync(string userId) => _mapper.Map<IEnumerable<OrderDto>>(await _unitOfWork.Orders.GetByUserIdAsync(userId));
    public async Task<IEnumerable<OrderDto>> GetOrdersByUserIdAsync(string userId) => await GetUserOrdersAsync(userId);
    public async Task<IEnumerable<OrderDto>> ListOrdersAsync() => _mapper.Map<IEnumerable<OrderDto>>(await _unitOfWork.Orders.GetAllAsync(q => q.Include(o => o.Customer).Include(o => o.Items).ThenInclude(i => i.Product).Include(o => o.Payments)));

    public async Task UpdateOrderStatusAsync(string orderId, OrderStatus status)
    {
        var order = await LoadOrderAsync(orderId); order.Status = status;
        if (status == OrderStatus.Shipped) order.ShippingDate = DateTime.UtcNow;
        if (status is OrderStatus.Delivered or OrderStatus.Completed) order.PaymentDate ??= DateTime.UtcNow;
        await _unitOfWork.Orders.UpdateAsync(order); await _unitOfWork.SaveChangesAsync(); await _cacheService.SetCachedDataAsync(order.Id, order, TimeSpan.FromHours(3));
    }

    public async Task CancelOrderAsync(string orderId)
    {
        var order = await LoadOrderAsync(orderId);
        if (order.Status is OrderStatus.Shipped or OrderStatus.Delivered) throw new InvalidOperationException("Cannot cancel a shipped or delivered order.");
        order.Status = OrderStatus.Cancelled; await _unitOfWork.Orders.UpdateAsync(order); await _unitOfWork.SaveChangesAsync(); await _cacheService.RemoveCachedDataAsync(order.Id);
    }

    public async Task CreateReturnAsync(CreateReturnRequest request)
    {
        var order = await LoadOrderAsync(request.OrderId);
        order.Status = request.TotalRefund >= order.TotalAmount ? OrderStatus.Returned : OrderStatus.PartiallyReturned;
        await _unitOfWork.Orders.UpdateAsync(order); await _unitOfWork.SaveChangesAsync();
        await SyncRefundsToFinanceAsync(order, request);
    }

    private async Task<Order> LoadOrderAsync(string orderId) => await _unitOfWork.Orders.GetByIdAsync(orderId, q => q.Include(o => o.Customer).Include(o => o.Items).ThenInclude(i => i.Product).Include(o => o.Payments)) ?? throw new KeyNotFoundException($"Order with ID {orderId} not found.");

    private async Task<string> ResolveCustomerAsync(CreateManualOrderRequest request)
    {
        if (!string.IsNullOrWhiteSpace(request.CustomerId)) return request.CustomerId;
        var existing = !string.IsNullOrWhiteSpace(request.CustomerPhone) ? await _unitOfWork.Users.GetSingleAsync(u => u.PhoneNumber == request.CustomerPhone) : null;
        if (existing is not null) return existing.Id;
        var user = new User { Id = Guid.NewGuid().ToString(), Username = request.CustomerPhone ?? request.CustomerEmail ?? Guid.NewGuid().ToString("N"), Email = request.CustomerEmail ?? string.Empty, PhoneNumber = request.CustomerPhone ?? string.Empty, FirstName = request.Customer, LastName = string.Empty, PasswordHash = string.Empty, IsActive = true, CreatedTime = DateTime.UtcNow };
        await _unitOfWork.Users.AddAsync(user);
        var role = await _unitOfWork.Roles.GetSingleAsync(r => r.Name == "Customer");
        if (role is not null) await _unitOfWork.UserRoles.AddAsync(new UserRole { UserId = user.Id, RoleId = role.Id });
        return user.Id;
    }

    private async Task SyncPaymentsToFinanceAsync(Order order, IEnumerable<OrderPaymentSplitDto> splits)
    {
        foreach (var split in splits.Where(s => !IsCredit(s.Method) && s.Status == PaymentStatus.Completed))
            await _financeService.RecordOrderPaymentAsync(new RecordOrderFinanceDto { OrderId = order.Id, FinanceAccountId = split.FinanceAccountId ?? "default-cash", BranchId = split.BranchId, PaymentMethod = MapFinanceMethod(split.Method), CounterpartyId = order.CustomerId, CounterpartyName = order.Customer?.FirstName });
    }

    private async Task SyncRefundsToFinanceAsync(Order order, CreateReturnRequest request)
    {
        foreach (var refund in request.Refunds.Where(s => !IsCredit(s.Method)))
            await _financeService.RecordOrderPaymentAsync(new RecordOrderFinanceDto { OrderId = order.Id, FinanceAccountId = refund.FinanceAccountId ?? "default-cash", BranchId = refund.BranchId, PaymentMethod = MapFinanceMethod(refund.Method), CounterpartyId = order.CustomerId, CounterpartyName = order.Customer?.FirstName });
    }

    private static PaymentMethod ParsePaymentMethod(string method) => Enum.TryParse<PaymentMethod>(method, true, out var parsed) ? parsed : PaymentMethod.OnlineGateway;
    private static bool IsCredit(string? method) => NormalizePaymentMethod(method) == "credit";

    private static PaymentMethod MapPaymentMethod(string? method) => NormalizePaymentMethod(method) switch
    {
        "card" => PaymentMethod.Card,
        "bank_transfer" => PaymentMethod.BankTransfer,
        "online_gateway" => PaymentMethod.OnlineGateway,
        "wallet" => PaymentMethod.Wallet,
        "cheque" => PaymentMethod.Cheque,
        "credit" => PaymentMethod.Credit,
        _ => PaymentMethod.Cash
    };

    private static FinancePaymentMethod MapFinanceMethod(string? method) => NormalizePaymentMethod(method) switch
    {
        "card" => FinancePaymentMethod.Card,
        "bank_transfer" => FinancePaymentMethod.BankTransfer,
        "online_gateway" => FinancePaymentMethod.OnlineGateway,
        "wallet" => FinancePaymentMethod.Wallet,
        _ => FinancePaymentMethod.Cash
    };

    private static string NormalizePaymentMethod(string? method) => (method ?? "cash").Trim().Replace("-", "_").ToLowerInvariant();
}
