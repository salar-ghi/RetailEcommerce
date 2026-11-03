namespace Application.Mapping;

public class OrderMappingProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Order, OrderDto>()
            .Map(dest => dest.UserId, src => src.CustomerId)
            .Map(dest => dest.CustomerId, src => src.CustomerId)
            .Map(dest => dest.Customer, src => src.Customer != null ? ($"{src.Customer.FirstName} {src.Customer.LastName}").Trim() : string.Empty)
            .Map(dest => dest.CustomerPhone, src => src.Customer != null ? src.Customer.PhoneNumber : string.Empty)
            .Map(dest => dest.OrderDate, src => src.CreatedTime)
            .Map(dest => dest.Date, src => src.CreatedTime.ToString("yyyy-MM-dd"))
            .Map(dest => dest.CustomerAddress, src => src.ShippingAddress != null ? src.ShippingAddress.AddressLine1 : string.Empty)
            .Map(dest => dest.Source, src => ToClientEnum(src.Source.ToString()))
            .Map(dest => dest.Channel, src => src.Source == OrderSource.Storefront ? "online" : "panel")
            .Map(dest => dest.Status, src => MapOrderStatus(src.Status))
            .Map(dest => dest.DiscountAmount, src => src.DiscountAmount)
            .Map(dest => dest.TotalAmount, src => src.TotalAmount)
            .Map(dest => dest.TotalPrice, src => src.FinalAmount)
            .Map(dest => dest.Total, src => src.SubtotalAmount)
            .Map(dest => dest.PaymentStatus, src => src.Payments.Any(p => p.Status != PaymentStatus.Completed) ? "pending" : "paid")
            .Map(dest => dest.TotalItems, src => src.TotalItems)
            .Map(dest => dest.FinalTotal, src => src.FinalAmount)
            .Map(dest => dest.ShippingAddress, src => src.ShippingAddress)
            .TwoWays();
        
        config.NewConfig<ShippingAddress, ShippingAddressDto>().TwoWays();

        config.NewConfig<OrderItem, OrderItemDto>()
            .Map(dest => dest.ProductName, src => src.Product != null ? src.Product.Name : string.Empty)
            .Map(dest => dest.TotalPrice, src => src.Subtotal)
            .TwoWays();
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
