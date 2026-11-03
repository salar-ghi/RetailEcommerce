namespace Application.Mapping;

public class PromotionMappingProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Promotion, PromotionDto>()
            .Map(dest => dest.Scope, src => src.Scope.ToString())
            .Map(dest => dest.IsActive, src => src.IsActive);

        config.NewConfig<Discount, DiscountDto>()
            .Map(dest => dest.Type, src => src.Type.ToString());

        config.NewConfig<PromotionCondition, PromotionConditionDto>()
            .Map(dest => dest.Type, src => src.Type.ToString());
    }
}
