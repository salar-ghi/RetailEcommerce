namespace Application.DTOs;

public class OrderItemDto
{
    public int Id { get; set; }
    public long ProductId { get; set; }
    public string ProductName { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public string SaleUnit { get; set; }
    public string WeightUnit { get; set; }
    public int? SpaceId { get; set; }
    public string SpaceName { get; set; }
    public int? ZoneId { get; set; }
    public string ZoneName { get; set; }
    public int? ShelfId { get; set; }
    public string ShelfCode { get; set; }
}