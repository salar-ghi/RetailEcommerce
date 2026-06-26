namespace Domain.Entities;

public class OrderItem : BaseModel<int>
{
    public long ProductId { get; set; }
    public Product Product { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal DiscountedPrice { get; set; }
    public decimal Subtotal => Quantity * (DiscountedPrice > 0 ? DiscountedPrice : UnitPrice);
    public string SaleUnit { get; set; } = "piece";
    public string WeightUnit { get; set; }
    public int? SpaceId { get; set; }
    public string SpaceName { get; set; }
    public int? ZoneId { get; set; }
    public string ZoneName { get; set; }
    public int? ShelfId { get; set; }
    public string ShelfCode { get; set; }
    public string OrderId { get; set; }
    public Order Order { get; set; }
}
