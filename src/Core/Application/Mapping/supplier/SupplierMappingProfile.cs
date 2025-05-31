namespace Application.Mapping;

public class SupplierMappingProfile : Profile
{
    public SupplierMappingProfile()
    {
        CreateMap<Supplier, SupplierDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.status.ToString()))
            .ReverseMap();

        CreateMap<SupplierRegistrationDto, Supplier>();


    }
}
