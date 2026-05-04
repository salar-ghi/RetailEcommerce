namespace Application.DTOs;

public class ProductUnitPriceDto
{
    public int Id { get; set; }
    public decimal Price { get; set; }
    public DateTime EffectiveDate { get; set; }
    public int ProductId { get; set; }
}

public class BatchDto
{
    public string BatchNumber { get; set; }
    public decimal Amount { get; set; }          // selling price
    public decimal CostPrice { get; set; }
    public string Currency { get; set; }
    public string PricingTier { get; set; }
    public DateTime EffectiveDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public int Quantity { get; set; }
    public string Notes { get; set; }
}