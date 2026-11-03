namespace Application.Mapping;

public class BannerMappingProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<BannerPlacement, BannerPlacementDto>().TwoWays();
        config.NewConfig<CreateBannerPlacementDto, BannerPlacement>().TwoWays();

        config.NewConfig<BannerPlacement, PlacementDto>();

        config.NewConfig<Banner, BannerDto>()
            .Map(dest => dest.Placements,
                src => src.BannerPlacementMaps
                    .Where(m => !m.IsDeleted && !m.Placement.IsDeleted)
                    .Select(m => m.Placement))
            .TwoWays();

        config.NewConfig<CreateBannerDto, Banner>()
            .Ignore(dest => dest.BannerPlacementMaps)
            .TwoWays();

        config.NewConfig<UpdateBannerDto, Banner>()
            .Ignore(dest => dest.BannerPlacementMaps)
            .TwoWays();
    }
}
