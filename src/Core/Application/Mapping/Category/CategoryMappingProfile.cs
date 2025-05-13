namespace Application.Mapping;

public class CategoryMappingProfile : Profile
{
    public CategoryMappingProfile()
    {
        CreateMap<Category, CategoryDto>().ReverseMap();
        CreateMap<Category, CategoryDetailsDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedTime))
            .ForMember(dest => dest.ParentCategoryId, opt => opt.MapFrom(src => src.ParentCategoryId))
            .ForMember(dest => dest.ProductCount, opt => opt.MapFrom(src => src.Products.Count()))
            .ForMember(dest => dest.Brands, opt => opt.MapFrom(src => src.Products.Select(p => p.Brand).Distinct()));
        CreateMap<CategoryAttribute, CategoryAttributeDto>().ReverseMap();
    }
}
