namespace Application.DTOs;

public class ProductVariantDto
{
    public int Id { get; set; }
    public string VariantName { get; set; }
    public string VariantValue { get; set; }
    public decimal PriceAdjustment { get; set; }
    public int ProductId { get; set; }
}