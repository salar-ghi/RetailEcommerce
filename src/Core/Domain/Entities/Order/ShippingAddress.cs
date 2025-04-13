namespace Domain.Entities;

public class ShippingAddress
{
    public string AddressLine1 { get; set; }
    public string Street { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string PostalCode { get; set; }
    public string Country { get; set; }

    public string OrderId { get; set; }
    public Order Order { get; set; } = new Order();

}
