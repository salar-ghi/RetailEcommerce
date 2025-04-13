namespace Domain.Entities;

public class ProductImage : BaseModel<long>
{
    public string ImageUrl { get; set; }
    public bool IsPrimary { get; set; }
    public string Description { get; set; }
    public int ProductId { get; set; }
    public Product Product { get; set; }
}