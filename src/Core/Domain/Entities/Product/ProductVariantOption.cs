namespace Domain.Entities;

public class ProductVariantOption : BaseModel<int>
{
    public string Value { get; set; }           // e.g., "قرمز", "XL"
    public int DisplayOrder { get; set; }


    // New fields to match frontend’s richer option data
    public string OptionValue { get; set; }          // e.g., “#FF0000” for colors
    public decimal? PriceAdjustment { get; set; }
    public int? StockQuantity { get; set; }
    public string? Sku { get; set; }
    public bool? IsAvailable { get; set; }

    public int DefinitionId { get; set; }
    public ProductVariantDefinition Definition { get; set; }
}
