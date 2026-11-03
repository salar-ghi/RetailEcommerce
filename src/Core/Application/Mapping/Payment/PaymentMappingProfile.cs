namespace Application.Mapping;

public class PaymentMappingProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Payment, PaymentDto>()
            .Map(dest => dest.PaymentMethod, src => ToClientEnum(src.Method.ToString()))
            .Map(dest => dest.Method, src => ToClientEnum(src.Method.ToString()))
            .Map(dest => dest.GatewayTxnId, src => src.TransactionId)
            .Map(dest => dest.Status, src => ToClientEnum(src.Status.ToString()));

        config.NewConfig<PaymentDto, Payment>()
            .Map(dest => dest.Method, src => Enum.Parse<PaymentMethod>(src.PaymentMethod, true))
            .Map(dest => dest.Status, src => Enum.Parse<PaymentStatus>(src.Status, true));
    }

    private static string ToClientEnum(string value) => string.Concat(value.Select((c, i) => i > 0 && char.IsUpper(c) ? "_" + char.ToLowerInvariant(c) : char.ToLowerInvariant(c).ToString()));
}
