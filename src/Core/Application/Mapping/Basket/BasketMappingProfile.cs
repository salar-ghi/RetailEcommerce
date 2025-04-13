namespace Application.Mapping;

public class BasketMappingProfile : Profile
{
    public BasketMappingProfile()
    {
        CreateMap<Basket, BasketDto>()
            .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.TotalPrice))
            .ForMember(dest => dest.TotalItems, opt => opt.MapFrom(src => src.TotalItems));

        CreateMap<BasketItem, BasketItemDto>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name));
    }
}