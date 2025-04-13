using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class Supplier : BaseModel<int>
{
    public string Name { get; set; }
    public string ContactInfo { get; set; }
    public string ContactName { get; set; }
    public string ContactEmail { get; set; }
    public string ContactPhone { get; set; }
    public string Address { get; set; }

    public int UserId { get; set; }
    public User User { get; set; }
    public ICollection<ProductSupplier> ProductSuppliers { get; set; } = new List<ProductSupplier>();
}