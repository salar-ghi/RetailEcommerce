namespace Application.Mapping;

public class SupplierMappingProfile : Profile
{
    public SupplierMappingProfile()
    {
        CreateMap<Supplier, SupplierDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ReverseMap();

        CreateMap<SupplierRegistrationDto, Supplier>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.Info, opt => opt.MapFrom(src => src.ContactInfo))
            .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.Phone))
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
            .ReverseMap();


        CreateMap<UpdateSupplierStatusDto, Supplier>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.SupplierName))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.SupplierEmail))
            .ForMember(dest => dest.Info, opt => opt.MapFrom(src => src.SupplierInfo))
            .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.SupplierPhone))
            .ForMember(dest => dest.Status, opt => opt.Condition(src => src.Status.HasValue))
            .ReverseMap();


    }
}
