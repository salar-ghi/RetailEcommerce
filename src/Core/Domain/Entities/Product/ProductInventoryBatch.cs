namespace Domain.Entities;

public class ProductInventoryBatch : BaseModel<long>
{
    public string BatchNumber { get; set; }
    public decimal CostPrice { get; set; }
    public decimal SellingPrice { get; set; }
    public string Currency { get; set; }
    public string PricingTier { get; set; }
    public DateTime EffectiveDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public int Quantity { get; set; }
    public int SoldQuantity { get; set; }
    public string Notes { get; set; }

    public long ProductId { get; set; }
    public Product Product { get; set; }
}
