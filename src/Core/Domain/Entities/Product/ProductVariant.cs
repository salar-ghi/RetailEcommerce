namespace Domain.Entities;

public class ProductVariant : BaseModel<long>
{
    public string SKU { get; set; }

    public string VariantName { get; set; }
    public string VariantValue { get; set; }
    [Column(TypeName = "decimal(18, 2)")]
    public decimal PriceAdjustment { get; set; }

    //public int StockId { get; set; }
    //public ProductStock Stocks { get; set; }
    //public int PriceId { get; set; }
    //public ProductUnitPrice Prices { get; set; }
    public long ProductId { get; set; }
    public Product Product { get; set; } = new Product();
}
