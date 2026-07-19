namespace Application.Mapping;

public class PaymentMappingProfile : Profile
{
    public PaymentMappingProfile()
    {
        CreateMap<Payment, PaymentDto>()
            .ForMember(dest => dest.PaymentMethod, opt => opt.MapFrom(src => ToClientEnum(src.Method.ToString())))
            .ForMember(dest => dest.Method, opt => opt.MapFrom(src => ToClientEnum(src.Method.ToString())))
            .ForMember(dest => dest.GatewayTxnId, opt => opt.MapFrom(src => src.TransactionId))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => ToClientEnum(src.Status.ToString())));
        CreateMap<PaymentDto, Payment>()
            .ForMember(dest => dest.Method, opt => opt.MapFrom(src => Enum.Parse<PaymentMethod>(src.PaymentMethod, true)))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => Enum.Parse<PaymentStatus>(src.Status, true)));
    }

    private static string ToClientEnum(string value) => string.Concat(value.Select((c, i) => i > 0 && char.IsUpper(c) ? "_" + char.ToLowerInvariant(c) : char.ToLowerInvariant(c).ToString()));
}
