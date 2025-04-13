namespace Application.DTOs;

public class ProductUnitPriceDto
{
    public int Id { get; set; }
    public decimal Price { get; set; }
    public DateTime EffectiveDate { get; set; }
    public int ProductId { get; set; }
}
