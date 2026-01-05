namespace Application.Mapping;

public class CategoryMappingProfile : Profile
{
    public CategoryMappingProfile()
    {
        CreateMap<Category, CategoryDto>()
            .ForMember(dest => dest.ParentId, opt => opt.MapFrom(src => src.ParentId))
            .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.ImageUrl))
            .ReverseMap();

        CreateMap<Category, CategoryDetailsDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedTime))
            .ForMember(dest => dest.ParentId, opt => opt.MapFrom(src => src.ParentId))
            .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.ImageUrl))
            .ForMember(dest => dest.ProductCount, opt => opt.MapFrom(src => src.Products.Count()))
            .ForMember(dest => dest.Brands, opt => opt.MapFrom(src => src.Products.Select(p => p.Brand).Distinct()))
            .ReverseMap();

        CreateMap<CategoryAttribute, CategoryAttributeDto>().ReverseMap();
    }
}
