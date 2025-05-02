namespace Domain.Entities;

public class ProductAttribute : BaseModel<long>
{
    public string Key { get; set; } // e.g., "Color", "Size"
    public string Value { get; set; } // e.g., "Red", "Large"
    public long ProductId { get; set; }
    public Product Product { get; set; }
}