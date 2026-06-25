namespace Domain.Entities;

public class ProductVariantOption : BaseModel<int>
{
    public string DisplayValue { get; set; }           // e.g., "قرمز", "XL"
    public string ActualValue { get; set; }          // e.g., “#FF0000” for colors
    public int DisplayOrder { get; set; }


    // New fields to match frontend’s richer option data
    public decimal? PriceAdjustment { get; set; }
    // UI/default stock input only. ProductStock records are the inventory source of truth.
    public int? StockQuantity { get; set; }
    public string? Sku { get; set; }
    public bool? IsAvailable { get; set; }

    public int DefinitionId { get; set; }
    public ProductVariantDefinition Definition { get; set; }
}
