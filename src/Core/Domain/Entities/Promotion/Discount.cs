namespace Domain.Entities;

public class Discount : BaseModel<int>
{
    public DiscountType Type { get; set; }
    public decimal Value { get; set; } // e.g., 10 for 10% or $5
    public int PromotionId { get; set; }
    public Promotion Promotion { get; set; }
}