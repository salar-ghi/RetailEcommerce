namespace Application.DTOs;

public class ProductVariantDto
{
    public int Id { get; set; }
    public string VariantName { get; set; }
    public string VariantValue { get; set; }
    public decimal PriceAdjustment { get; set; }
    public int ProductId { get; set; }
}

public class VariantDto
{
    public int? Id { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
    public List<VariantOptionDto>? Options { get; set; } = new();
    public bool? Required { get; set; }
    public int? DisplayOrder { get; set; }
}

public class VariantOptionDto
{
    public int? Id { get; set; }
    public string Name { get; set; }                      // display value, e.g., "قرمز"
    public string Value { get; set; }                     // underlying value, e.g., "#FF0000"
    public decimal? PriceAdjustment { get; set; }
    public int? StockQuantity { get; set; }
    public string? Sku { get; set; }
    public bool? IsAvailable { get; set; }
}