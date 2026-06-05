namespace Domain.Entities;

public class Tag : BaseModel<int>
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Color { get; set; } = string.Empty;
    public ICollection<ProductTag> Products { get; set; } = new List<ProductTag>();

}
