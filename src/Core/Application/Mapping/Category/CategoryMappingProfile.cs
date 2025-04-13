namespace Application.Mapping;

public class CategoryMappingProfile : Profile
{
    public CategoryMappingProfile()
    {
        CreateMap<Category, CategoryDto>().ReverseMap();
        CreateMap<CategoryAttribute, CategoryAttributeDto>().ReverseMap();
    }
}
