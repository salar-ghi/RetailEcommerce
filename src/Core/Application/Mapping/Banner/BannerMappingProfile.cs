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
                    src.BannerPlacementMaps
                        .Where(m => !m.IsDeleted && !m.Placement.IsDeleted)
                        .Select(m => m.Placement)))
            .ReverseMap();

        CreateMap<CreateBannerDto, Banner>()
            .ForMember(dest => dest.BannerPlacementMaps, opt => opt.Ignore())
            .ReverseMap();

        CreateMap<UpdateBannerDto, Banner>()
            .ForMember(dest => dest.BannerPlacementMaps, opt => opt.Ignore())
            .ReverseMap();
    }
}
