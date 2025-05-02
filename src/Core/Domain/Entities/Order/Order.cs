namespace Domain.Entities;

public class Order : BaseModel<string>
{
    public string UserId { get; set; }
    public User User { get; set; }
    
    public OrderStatus Status { get; set; }
    public decimal DiscountAmount { get; set; } = 0m;
    public decimal TotalAmount => Items.Sum(item => item.Quantity * item.UnitPrice) - DiscountAmount;
    public int TotalItems => Items.Sum(item => item.Quantity);
    public ShippingAddress ShippingAddress { get; set; } = new ShippingAddress();

    [Timestamp]
    public byte[] RowVersion { get; set; }
    public DateTime? PaymentDate { get; set; }
    public DateTime? ShippingDate { get; set; }
    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    public List<Payment> Payments { get; set; } = new();
}
