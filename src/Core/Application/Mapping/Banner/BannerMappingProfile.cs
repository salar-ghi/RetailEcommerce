namespace Application.Mapping;

public class BannerMappingProfile : Profile
{
    public BannerMappingProfile()
    {
        CreateMap<BannerPlacement, BannerPlacementDto>().ReverseMap();
        CreateMap<CreateBannerPlacementDto, BannerPlacement>().ReverseMap();


        CreateMap<Banner, BannerDto>()
            .ForMember(dest => dest.PlacementCodes, opt => opt.MapFrom(src => src.Placements.Select(p => p.Code).ToList()))
            .ReverseMap();

        CreateMap<CreateBannerDto, Banner>()
            //.ForMember(dest => dest.Priority, opt => opt.MapFrom(src => src.DisplayOrder))
            .ReverseMap();

        CreateMap<UpdateBannerDto, Banner>()
            //.ForMember(dest => dest.Priority, opt => opt.MapFrom(src => src.DisplayOrder))
            .ReverseMap();
    }
}
