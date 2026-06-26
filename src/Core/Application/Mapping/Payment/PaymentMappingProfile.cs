namespace Application.Mapping;

public class PaymentMappingProfile : Profile
{
    public PaymentMappingProfile()
    {
        CreateMap<Payment, PaymentDto>()
            .ForMember(dest => dest.PaymentMethod, opt => opt.MapFrom(src => src.Method.ToString()))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));
        CreateMap<PaymentDto, Payment>()
            .ForMember(dest => dest.Method, opt => opt.MapFrom(src => Enum.Parse<PaymentMethod>(src.PaymentMethod, true)))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => Enum.Parse<PaymentStatus>(src.Status, true)));
    }
}
