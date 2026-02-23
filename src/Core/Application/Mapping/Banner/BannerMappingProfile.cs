namespace Application.Mapping;

public class BannerMappingProfile : Profile
{
    public BannerMappingProfile()
    {
        CreateMap<BannerPlacement, BannerPlacementDto>().ReverseMap();
        CreateMap<CreateBannerPlacementDto, BannerPlacement>().ReverseMap();


        CreateMap<Banner, BannerDto>()
            .ForMember(dest => dest.PlacementCodes, 
                opt => opt.MapFrom(src => src.BannerPlacementMaps.Select(p => p.Placement)))
            .ReverseMap();

        CreateMap<CreateBannerDto, Banner>()
            .ReverseMap();

        CreateMap<UpdateBannerDto, Banner>()
            .ReverseMap();
    }
}
