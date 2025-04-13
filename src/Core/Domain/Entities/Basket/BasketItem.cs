namespace Domain.Entities;

public class BasketItem : BaseModel<int>
{

    public long ProductId { get; set; }
    public Product Product { get; set; }
    public int Quantity { get; set; }
    public decimal Subtotal => UnitPrice * Quantity;
    public decimal UnitPrice { get; set; }
    public decimal DiscountedPrice { get; set; }
    public string? AppliedPromotionCode { get; set; }

    public string BasketId { get; set; } 
    public Basket Basket { get; set; }
}
