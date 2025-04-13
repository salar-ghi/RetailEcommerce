namespace Domain.Entities;

public class Warehouse : BaseModel<int>
{
    public Guid Id { get; set; }
    public string Code { get; set; } // e.g., "WH-USA-01"
    public string Name { get; set; }
    //public Address Location { get; set; } // Value Object
    public string Location { get; set; } // Value Object
    public bool IsActive { get; set; }
    public decimal StorageCapacity { get; set; } // In cubic meters

    public ICollection<ProductStock> InventoryItems { get; set; }
}
