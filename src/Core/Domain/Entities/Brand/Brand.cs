namespace Domain.Entities;

public class Brand : BaseModel<int>
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string? ImageUrl { get; set; }

    public ICollection<Product> Products { get; set; } = new List<Product>();
}