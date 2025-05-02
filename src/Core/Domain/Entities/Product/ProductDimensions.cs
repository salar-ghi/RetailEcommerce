namespace Domain.Entities;

public class ProductDimensions : BaseModel<int>
{
    public decimal Length { get; set; }
    public decimal Width { get; set; }
    public decimal Height { get; set; }
    public decimal Weight { get; set; }
    public string Unit { get; set; }
    public long ProductId { get; set; }
    public Product Product { get; set; }
}
