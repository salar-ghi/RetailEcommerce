namespace Domain.Entities;

public class InventoryTransaction : BaseModel<Guid>
{
    public TransactionType Type { get; set; }
    public int Quantity { get; set; }
    public DateTime TransactionDate { get; set; }
    public string ReferenceId { get; set; } // e.g., OrderId, POId
    public string Notes { get; set; }

    public int InventoryItemId { get; set; }
    public ProductStock ProductStock { get; set; }

}
