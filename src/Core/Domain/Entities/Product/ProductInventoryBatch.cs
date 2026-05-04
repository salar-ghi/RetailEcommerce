namespace Domain.Entities;

public class ProductInventoryBatch : BaseModel<long>
{
    public string BatchNumber { get; set; }
    public decimal CostPrice { get; set; }               // Purchase cost per unit
    public decimal SellingPrice { get; set; }            // Retail sell price per unit
    public string Currency { get; set; }
    public string PricingTier { get; set; }              // retail / wholesale / discount / premium
    public DateTime EffectiveDate { get; set; }           // Date of receipt
    public DateTime? ExpiryDate { get; set; }
    public int Quantity { get; set; }                     // Current available quantity
    public int SoldQuantity { get; set; }
    public string Notes { get; set; }

    public long ProductId { get; set; }
    public Product Product { get; set; }
}
