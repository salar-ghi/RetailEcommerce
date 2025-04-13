namespace Application.DTOs;

public class ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string CategoryName { get; set; }
    public string BrandName { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public string SupplierName { get; set; }

}