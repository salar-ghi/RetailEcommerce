namespace Domain.Entities;

public class Shelf : BaseModel<int>
{
    public int SpaceId { get; set; }
    public StorageSpace Space { get; set; }
    public int? ZoneId { get; set; }
    public StorageZone Zone { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public int? Level { get; set; }
    public int? Row { get; set; }
    public int? Column { get; set; }
    public int Capacity { get; set; }
    public int Used { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<ProductStock> ProductStocks { get; set; } = new List<ProductStock>();
}
