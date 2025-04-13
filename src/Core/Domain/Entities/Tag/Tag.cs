namespace Domain.Entities;

public class Tag : BaseModel<int>
{
    public string Name { get; set; }
    public ICollection<ProductTag> Products { get; set; } = new List<ProductTag>();

}
