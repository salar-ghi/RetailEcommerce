namespace Application.DTOs;

public class BasketItemDto
{
    public long Id { get; set; }
    public long ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}
