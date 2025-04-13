namespace Domain.Entities;

public class StockReservation : BaseModel<int>
{
    public int Quantity { get; set; }
    public DateTime ExpiryDate { get; set; } // Reservation timeout
    public Guid OrderId { get; set; } // Nullable if temporary hold

    public int InventoryItemId { get; set; }
    public ProductStock ProductStock { get; set; }
}
