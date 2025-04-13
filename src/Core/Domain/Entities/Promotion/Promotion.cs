namespace Domain.Entities;

public class Promotion : BaseModel<int>
{
    public string Code { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive => DateTime.UtcNow >= StartDate && DateTime.UtcNow <= EndDate;
    public PromotionScope Scope { get; set; }
    public ICollection<PromotionCondition> Conditions { get; set; } = new List<PromotionCondition>();
    public ICollection<Discount> Discounts { get; set; } = new List<Discount>();

    public ICollection<ProductPromotion> Products { get; set; } = new List<ProductPromotion>();
    public ICollection<CategoryPromotion> Categories { get; set; } = new List<CategoryPromotion>();
    public ICollection<OrderPromotion> Orders { get; set; } = new List<OrderPromotion>();
}
