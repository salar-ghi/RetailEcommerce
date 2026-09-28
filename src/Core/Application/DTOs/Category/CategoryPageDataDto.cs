namespace Application.DTOs;

public class CategoryPageDataDto
{
    public CategoryDto? Category { get; set; }
    public IReadOnlyList<CategoryAttributeDefinitionDto> AttributeDefinitions { get; set; } = new List<CategoryAttributeDefinitionDto>();
    public IReadOnlyList<ProductDto> Products { get; set; } = new List<ProductDto>();
    public IReadOnlyList<BannerDto> Banners { get; set; } = new List<BannerDto>();
}
