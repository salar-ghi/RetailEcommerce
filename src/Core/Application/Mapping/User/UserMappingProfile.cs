namespace Application.Mapping;

public class UserMappingProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<User, UserDto>()
            .Map(dest => dest.FirstName, src => src.FirstName)
            .Map(dest => dest.LastName, src => src.LastName)
            .Map(dest => dest.Roles, src => src.UserRoles.Select(ur => ur.Role.Name))
            .TwoWays();
        config.NewConfig<Role, RoleDto>().TwoWays();
        config.NewConfig<UserRole, UserRoleDto>().TwoWays();
        config.NewConfig<UserRoleDto, UserRole>()
            .Ignore(dest => dest.Id);
        config.NewConfig<UserAddress, UserAddressDto>().TwoWays();
    }
}
