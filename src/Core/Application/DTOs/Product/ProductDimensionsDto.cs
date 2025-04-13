namespace Application.DTOs;

public class ProductDimensionsDto
{
    public int Id { get; set; }
    public decimal Length { get; set; }
    public decimal Width { get; set; }
    public decimal Height { get; set; }
    public string Unit { get; set; }
    public int ProductId { get; set; }
}