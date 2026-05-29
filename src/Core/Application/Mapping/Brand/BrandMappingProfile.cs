namespace Application.Mapping;

public class BrandMappingProfile : Profile
{
    public BrandMappingProfile()
    {
        CreateMap<Brand, BrandDto>()
            .ForMember(dest => dest.Logo, opt => opt.MapFrom(src => src.ImageUrl))
            .ForMember(dest => dest.CreatedTime, opt => opt.MapFrom(src => src.CreatedTime))
            .ForMember(dest => dest.CategoryIds, opt => opt.MapFrom(src => src.BrandCategories.Select(bc => bc.CategoryId)))
            .ForMember(dest => dest.Categories, opt => opt.MapFrom(src => src.BrandCategories
                .Select(bc => new BrandCategories { Id = bc.Category.Id, Name = bc.Category.Name })));

        CreateMap<BrandDto, Brand>()
            .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.Logo))
            .ForMember(dest => dest.BrandCategories, opt => opt.Ignore())
            .ForMember(dest => dest.Products, opt => opt.Ignore());

        CreateMap<Brand, BrandUpdateDto>()
            .ForMember(dest => dest.Logo, opt => opt.MapFrom(src => src.ImageUrl))
            .ReverseMap();
    }
}
