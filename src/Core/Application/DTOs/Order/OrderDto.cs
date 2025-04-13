namespace Application.DTOs;

public class OrderDto
{
    public string Id { get; set; }
    public string UserId { get; set; }
    public DateTime OrderDate { get; set; }
    public string Status { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public int TotalItems { get; set; }
    public ShippingAddressDto ShippingAddress { get; set; }
    public string PaymentMethod { get; set; }
    public DateTime? PaymentDate { get; set; }
    public DateTime? ShippingDate { get; set; }
    public List<OrderItemDto> Items { get; set; } = new List<OrderItemDto>();
}
