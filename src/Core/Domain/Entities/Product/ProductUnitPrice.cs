namespace Domain.Entities;

public class ProductUnitPrice : BaseModel<long>
{
    [Column(TypeName = "decimal(18, 2)")]
    public decimal Price { get; set; }
    public string Currency { get; set; }     // e.g., "USD", "EUR"
    public string PricingTier { get; set; }  // e.g., "Retail", "Wholesale"
    public DateTime EffectiveDate { get; set; }

    public long ProductId { get; set; }
    public Product Product { get; set; }
}