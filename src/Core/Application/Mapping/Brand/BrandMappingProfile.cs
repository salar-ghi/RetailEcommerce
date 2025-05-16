namespace Application.Mapping;

public class BrandMappingProfile : Profile
{
    public BrandMappingProfile()
    {
        CreateMap<Brand, BrandDto>()
            .ForMember(dest => dest.Logo, opt => opt.MapFrom(src => src.ImageUrl))
            .ForMember(dest => dest.CreatedTime, opt => opt.MapFrom(src => src.CreatedTime))
            .ReverseMap();

        CreateMap<Brand, BrandUpdateDto>()
            .ForMember(dest => dest.Logo, opt => opt.MapFrom(src => src.ImageUrl))
            .ReverseMap();
    }
}
