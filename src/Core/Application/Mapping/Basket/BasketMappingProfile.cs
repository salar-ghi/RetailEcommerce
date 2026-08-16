namespace Application.Mapping;

public class BasketMappingProfile : Profile
{
    public BasketMappingProfile()
    {
        CreateMap<Basket, BasketDto>()
            .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.TotalPrice))
            .ForMember(dest => dest.TotalItems, opt => opt.MapFrom(src => src.TotalItems))
            .ForMember(dest => dest.OwnerName, opt => opt.MapFrom(src => src.User != null ? ($"{src.User.FirstName} {src.User.LastName}").Trim() : null))
            .ForMember(dest => dest.AgeHours, opt => opt.MapFrom(src => (int)Math.Max(0, (DateTime.UtcNow - src.CreatedTime).TotalHours)));

        CreateMap<BasketItem, BasketItemDto>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name));
    }
}