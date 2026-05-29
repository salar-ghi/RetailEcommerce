namespace Domain.Entities;

public class ProductStock : BaseModel<long>
{
    //public Quantity stock { get; set; }        // Current stock level
    public int Quantity { get; set; }           // Current stock level
    public int ReorderThreshold { get; set; }    // Threshold for reordering
    public int MinimumStockLevel { get; set; }

    public int? WarehouseId { get; set; }    // Optional foreign key to Warehouse
    public Warehouse Warehouse { get; set; }

    public int? SpaceId { get; set; }
    public StorageSpace Space { get; set; }
    public int? ZoneId { get; set; }
    public StorageZone Zone { get; set; }
    public int? ShelfId { get; set; }
    public Shelf Shelf { get; set; }
    public long? ProductInventoryBatchId { get; set; }
    public ProductInventoryBatch ProductInventoryBatch { get; set; }
    public int? ProductVariantOptionId { get; set; }
    public ProductVariantOption ProductVariantOption { get; set; }
    public int ReservedQuantity { get; set; }
    public string Sku { get; set; }
    public string LocationNote { get; set; }

    public int AvailableQuantity => Math.Max(0, Quantity - ReservedQuantity);

    public long ProductId { get; set; }
    public Product Product { get; set; }


    public ICollection<InventoryTransaction> Transactions { get; set; } = new List<InventoryTransaction>();
    public ICollection<StockReservation> Reservations { get; set; } = new List<StockReservation>();


    [Timestamp]
    public byte[] RowVersion { get; set; } // Concurrency
}
