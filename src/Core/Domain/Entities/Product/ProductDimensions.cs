namespace Domain.Entities;

public class ProductDimensions : BaseModel<int>
{
    public decimal Length { get; set; }
    public decimal Width { get; set; }
    public decimal Height { get; set; }
    public decimal Weight { get; set; }
    public string DimensionUnit { get; set; }   // cm, m, mm, inch, ft
    public string WeightUnit { get; set; }      // gram, kilogram, mithqal, ounce, pound
    public long ProductId { get; set; }
    public Product Product { get; set; }
}
