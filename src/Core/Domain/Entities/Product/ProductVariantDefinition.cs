namespace Domain.Entities;

public class ProductVariantDefinition : BaseModel<int>
{
    public string Name { get; set; }            // e.g., "رنگ", "سایز"
    public string Type { get; set; }            // "color", "size", "material", "style", "capacity", "custom"
    public bool Required { get; set; }
    public int DisplayOrder { get; set; }

    public long ProductId { get; set; }
    public Product Product { get; set; }

    public ICollection<ProductVariantOption> Options { get; set; } = new List<ProductVariantOption>();
}
