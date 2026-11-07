namespace Application.Mapping;

public class UserActivityMappingProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<UserActivity, UserActivityDto>();
        config.NewConfig<TrackUserActivityDto, UserActivity>();
    }
}
