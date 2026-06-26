namespace Application.DTOs;

public class OrderDto
{
    public string Id { get; set; }
    public string UserId { get; set; }
    public string CustomerId { get; set; }
    public string Customer { get; set; }
    public string CustomerPhone { get; set; }
    public DateTime OrderDate { get; set; }
    public string Status { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal TotalItems { get; set; }
    public decimal FinalTotal { get; set; }
    public string Source { get; set; }
    public string Notes { get; set; }
    public ShippingAddressDto ShippingAddress { get; set; }
    public string PaymentMethod { get; set; }
    public DateTime? PaymentDate { get; set; }
    public DateTime? ShippingDate { get; set; }
    public List<OrderItemDto> Items { get; set; } = new List<OrderItemDto>();
    public List<PaymentDto> Payments { get; set; } = new();
}
