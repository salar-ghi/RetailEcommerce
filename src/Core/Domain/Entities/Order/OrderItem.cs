namespace Domain.Entities;

public class OrderItem : BaseModel<int>
{
    public int ProductId { get; set; }
    public Product Product { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal DiscountedPrice { get; set; }
    public decimal Subtotal => Quantity * (DiscountedPrice > 0 ? DiscountedPrice : UnitPrice);
    public string OrderId { get; set; }
    public Order Order { get; set; }
}
