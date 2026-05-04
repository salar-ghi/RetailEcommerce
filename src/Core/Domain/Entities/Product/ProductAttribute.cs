namespace Domain.Entities;

public class ProductAttribute : BaseModel<long>
{
    public string Key { get; set; } 
    public string Value { get; set; }
    public long ProductId { get; set; }
    public Product Product { get; set; }
}