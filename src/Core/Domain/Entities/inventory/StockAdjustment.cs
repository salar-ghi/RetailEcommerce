namespace Domain.Entities;

public class StockAdjustment : BaseModel<int>
{
    public Guid InventoryItemId { get; set; }
    public int AdjustedQuantity { get; set; }
    public AdjustmentReason Reason { get; set; }
    public string Notes { get; set; }
    public DateTime AdjustmentDate { get; set; }
    public string ApprovedBy { get; set; }
    public AdjustmentStatus Status { get; set; } = AdjustmentStatus.Pending;

    public ProductStock InventoryItem { get; set; }
}
