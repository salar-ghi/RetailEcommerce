namespace Application.Mapping;

public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
            .ForMember(dest => dest.Roles, opt => opt.MapFrom(src => src.UserRoles.Select(ur => ur.Role.Name)))
            .ReverseMap();
        CreateMap<Role, RoleDto>().ReverseMap();
        CreateMap<UserRole, UserRoleDto>().ReverseMap();
        CreateMap<UserAddress, UserAddressDto>().ReverseMap();
    }
}
