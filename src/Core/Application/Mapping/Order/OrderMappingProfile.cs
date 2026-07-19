namespace Application.Mapping;

public class OrderMappingProfile : Profile
{
    public OrderMappingProfile()
    {
        CreateMap<Order, OrderDto>()
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.CustomerId))
            .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.CustomerId))
            .ForMember(dest => dest.Customer, opt => opt.MapFrom(src => src.Customer != null ? ($"{src.Customer.FirstName} {src.Customer.LastName}").Trim() : string.Empty))
            .ForMember(dest => dest.CustomerPhone, opt => opt.MapFrom(src => src.Customer != null ? src.Customer.PhoneNumber : string.Empty))
            .ForMember(dest => dest.OrderDate, opt => opt.MapFrom(src => src.CreatedTime))
            .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.CreatedTime.ToString("yyyy-MM-dd")))
            .ForMember(dest => dest.CustomerAddress, opt => opt.MapFrom(src => src.ShippingAddress != null ? src.ShippingAddress.AddressLine1 : string.Empty))
            .ForMember(dest => dest.Source, opt => opt.MapFrom(src => ToClientEnum(src.Source.ToString())))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => MapOrderStatus(src.Status)))
            .ForMember(dest => dest.DiscountAmount, opt => opt.MapFrom(src => src.DiscountAmount))
            .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => src.TotalAmount))
            .ForMember(dest => dest.Total, opt => opt.MapFrom(src => src.SubtotalAmount))
            .ForMember(dest => dest.PaymentStatus, opt => opt.MapFrom(src => src.Payments.Any(p => p.Status != PaymentStatus.Completed) ? "pending" : "paid"))
            .ForMember(dest => dest.TotalItems, opt => opt.MapFrom(src => src.TotalItems))
            .ForMember(dest => dest.FinalTotal, opt => opt.MapFrom(src => src.FinalAmount))
            .ForMember(dest => dest.ShippingAddress, opt => opt.MapFrom(src => src.ShippingAddress)).ReverseMap();
        
        CreateMap<ShippingAddress, ShippingAddressDto>().ReverseMap();

        CreateMap<OrderItem, OrderItemDto>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : string.Empty))
            .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.Subtotal)).ReverseMap();

    }

    private static string MapOrderStatus(OrderStatus status) => status switch
    {
        OrderStatus.Processing => "approved",
        OrderStatus.Pending => "pending",
        OrderStatus.Rejected => "rejected",
        OrderStatus.Shipped => "shipped",
        OrderStatus.Delivered or OrderStatus.Completed => "delivered",
        _ => ToClientEnum(status.ToString())
    };

    private static string ToClientEnum(string value) => string.Concat(value.Select((c, i) => i > 0 && char.IsUpper(c) ? "_" + char.ToLowerInvariant(c) : char.ToLowerInvariant(c).ToString()));
}
