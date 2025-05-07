namespace Application.DTOs;

public class CategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int? ParentCategoryId { get; set; }

    public int productCount { get; set; }
    public DateTime CreatedAt { get; set; }

}