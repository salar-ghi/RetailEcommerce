namespace Domain.Entities;

public class StorageZone : BaseModel<int>
{
    public int SpaceId { get; set; }
    public StorageSpace Space { get; set; }
    public string Name { get; set; }
    public string Code { get; set; }
    public string Description { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<Shelf> Shelves { get; set; } = new List<Shelf>();
    public ICollection<ProductStock> ProductStocks { get; set; } = new List<ProductStock>();
}
