namespace Application.DTOs;

public enum OrderSourceDto
{
    Storefront = 0,
    AdminManual = 1
}

public enum OrderPaymentMethodDto
{
    Cash = 0,
    Card = 1,
    BankTransfer = 2,
    OnlineGateway = 3,
    Wallet = 4,
    Cheque = 5,
    Credit = 6
}

public sealed class CreateManualOrderRequest
{
    public string? CustomerId { get; set; }
    public string Customer { get; set; } = string.Empty;
    public string? CustomerPhone { get; set; }
    public string? CustomerEmail { get; set; }
    public string? CustomerAddress { get; set; }
    public ShippingAddressDto? ShippingAddress { get; set; }
    public List<CreateOrderItemRequest> Items { get; set; } = new();
    public decimal DiscountAmount { get; set; }
    public decimal FinalTotal { get; set; }
    public List<OrderPaymentSplitDto> Payments { get; set; } = new();
    public string? Notes { get; set; }
}

public sealed class CreateOrderItemRequest
{
    public long ProductId { get; set; }
    public string? ProductName { get; set; }
    public int? CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public int? BrandId { get; set; }
    public string? BrandName { get; set; }
    public decimal Quantity { get; set; }
    public string SaleUnit { get; set; } = "piece";
    public string? WeightUnit { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public int? SpaceId { get; set; }
    public string? SpaceName { get; set; }
    public int? ZoneId { get; set; }
    public string? ZoneName { get; set; }
    public int? ShelfId { get; set; }
    public string? ShelfCode { get; set; }
}

public sealed class OrderPaymentSplitDto
{
    public string? Id { get; set; }
    public string Method { get; set; } = "cash";
    public decimal Amount { get; set; }
    public PaymentStatus Status { get; set; } = PaymentStatus.Completed;
    public string? GatewayTxnId { get; set; }
    public DateTime? DueDate { get; set; }
    public string? FinanceAccountId { get; set; }
    public string? BranchId { get; set; }
}

public sealed class CreateReturnRequest
{
    public string OrderId { get; set; } = string.Empty;
    public List<ReturnItemDto> Items { get; set; } = new();
    public List<OrderPaymentSplitDto> Refunds { get; set; } = new();
    public decimal TotalRefund { get; set; }
    public string? Notes { get; set; }
}

public sealed class ReturnItemDto
{
    public int OrderItemId { get; set; }
    public long ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public string Reason { get; set; } = "other";
    public string? Note { get; set; }
}
