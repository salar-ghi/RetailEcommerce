namespace Domain.Entities;

public class ProductStock : BaseModel<long>
{
    //public Quantity stock { get; set; }        // Current stock level
    public int Quantity { get; set; }           // Current stock level
    public int ReorderThreshold { get; set; }    // Threshold for reordering
    public int MinimumStockLevel { get; set; }

    public int? WarehouseId { get; set; }    // Optional foreign key to Warehouse
    public Warehouse Warehouse { get; set; }


    public int ProductId { get; set; }
    public Product Product { get; set; }


    public ICollection<InventoryTransaction> Transactions { get; set; } = new List<InventoryTransaction>();
    public ICollection<StockReservation> Reservations { get; set; } = new List<StockReservation>();


    [Timestamp]
    public byte[] RowVersion { get; set; } // Concurrency
}
