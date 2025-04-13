namespace Application.DTOs;

public class ProductStockDto
{
    public int Id { get; set; }
    public int Quantity { get; set; }
    public int LowStockThreshold { get; set; }
    public int ProductId { get; set; }
}