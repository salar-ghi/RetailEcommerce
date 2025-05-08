namespace Application.Mapping;

public class BrandMappingProfile : Profile
{
    public BrandMappingProfile()
    {
        CreateMap<Brand, BrandDto>()
            .ForMember(dest => dest.createdAt, opt => opt.MapFrom(src => src.CreatedTime))
            .ReverseMap();
    }
}
