namespace Domain.Entities;

public class Category : BaseModel<int>
{
    public string Name { get; set; }
    public string Description { get; set; }
    public int? ParentCategoryId { get; set; } // For hierarchical categories

    public ICollection<CategoryAttribute> Attributes { get; set; } = new List<CategoryAttribute>();
    public ICollection<Product> Products { get; set; } = new List<Product>();
}