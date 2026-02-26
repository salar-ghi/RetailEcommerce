namespace Application.Mapping;

public class BannerMappingProfile : Profile
{
    public BannerMappingProfile()
    {
        CreateMap<BannerPlacement, BannerPlacementDto>().ReverseMap();
        CreateMap<CreateBannerPlacementDto, BannerPlacement>().ReverseMap();

        //CreateMap<BannerPlacement, PlacementDto>();
        //CreateMap<Banner, BannerDto>()
        //    .ForMember(dest => dest.Placement, 
        //        opt => opt.MapFrom(src => 
        //            src.BannerPlacementMaps.Select(p => p.Placement)))
        //    .ReverseMap();

        CreateMap<BannerPlacement, PlacementDto>();

        CreateMap<Banner, BannerDto>()
            .ForMember(dest => dest.Placements,
                opt => opt.MapFrom(src =>
                    src.BannerPlacementMaps.Select(m => m.Placement)))
            .ReverseMap();

        CreateMap<CreateBannerDto, Banner>()
            .ReverseMap();

        CreateMap<UpdateBannerDto, Banner>()
            .ReverseMap();
    }
}
