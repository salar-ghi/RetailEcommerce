namespace Domain.Entities;

public class StorageSpace : BaseModel<int>
{
    public string Name { get; set; }
    public StorageSpaceType Type { get; set; }
    public string Code { get; set; }
    public string Address { get; set; }
    public string Description { get; set; }
    public int Capacity { get; set; }
    public int Used { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<StorageZone> Zones { get; set; } = new List<StorageZone>();
    public ICollection<Shelf> Shelves { get; set; } = new List<Shelf>();
    public ICollection<ProductStock> ProductStocks { get; set; } = new List<ProductStock>();
}
