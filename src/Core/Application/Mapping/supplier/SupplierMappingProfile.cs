namespace Application.Mapping;

public class SupplierMappingProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Supplier, SupplierDto>()
            .Map(dest => dest.Status, src => src.Status.ToString())
            .Map(dest => dest.IsApproved, src => src.Status == SupplierStatus.Approved)
            .Map(dest => dest.ContactInfo, src => src.Info)
            .TwoWays();

        config.NewConfig<SupplierRegistrationDto, Supplier>()
            .Map(dest => dest.Name, src => src.Name)
            .Map(dest => dest.Email, src => src.Email)
            .Map(dest => dest.Info, src => src.ContactInfo)
            .Map(dest => dest.Phone, src => src.Phone)
            .Map(dest => dest.Address, src => src.Address)
            .Map(dest => dest.Website, src => src.Website)
            .Map(dest => dest.Description, src => src.Description)
            .TwoWays();

        config.NewConfig<UpdateSupplierStatusDto, Supplier>()
            .Map(dest => dest.Name, src => src.Name)
            .Map(dest => dest.Email, src => src.Email)
            .Map(dest => dest.Info, src => src.ContactInfo)
            .Map(dest => dest.Phone, src => src.Phone)
            .Map(dest => dest.Address, src => src.Address)
            .Map(dest => dest.Website, src => src.Website)
            .Map(dest => dest.Description, src => src.Description)
            .IgnoreIf((src, dest) => !src.Status.HasValue, dest => dest.Status)
            .TwoWays();
    }
}
