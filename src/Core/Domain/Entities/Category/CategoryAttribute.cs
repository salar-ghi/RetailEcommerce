namespace Domain.Entities;

public class CategoryAttribute : BaseModel<int>
{
    public string Key { get; set; }
    public string Value { get; set; }

    public int CategoryId { get; set; }
    public Category Category { get; set; }
}