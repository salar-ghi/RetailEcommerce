namespace Domain.Entities;

public class ProductVariant : BaseModel<long>
{
    public string SKU { get; set; }          // Stock Keeping Unit for the variant

    public string VariantName { get; set; } // e.g., "Size", "Color"
    public string VariantValue { get; set; } // e.g., "Large", "Red"
    [Column(TypeName = "decimal(18, 2)")]
    public decimal PriceAdjustment { get; set; }

    //public int StockId { get; set; }
    //public ProductStock Stocks { get; set; }
    //public int PriceId { get; set; }
    //public ProductUnitPrice Prices { get; set; }
    public long ProductId { get; set; }
    public Product Product { get; set; }
}
