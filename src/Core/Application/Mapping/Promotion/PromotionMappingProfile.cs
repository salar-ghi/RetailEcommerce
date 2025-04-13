namespace Application.Mapping;

public class PromotionMappingProfile : Profile
{
    public PromotionMappingProfile()
    {
        CreateMap<Promotion, PromotionDto>()
            .ForMember(dest => dest.Scope, opt => opt.MapFrom(src => src.Scope.ToString()))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive));

        CreateMap<Discount, DiscountDto>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()));

        CreateMap<PromotionCondition, PromotionConditionDto>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()));
    }
}