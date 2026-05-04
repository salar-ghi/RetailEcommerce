namespace Domain.Entities;

public class ProductSupplier : BaseModel<int>
{
    public long ProductId { get; set; }
    public Product Product { get; set; } = new Product();
    public int SupplierId { get; set; }
    public Supplier Supplier { get; set; } = new Supplier();
}