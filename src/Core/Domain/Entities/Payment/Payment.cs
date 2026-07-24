namespace Domain.Entities;

public class Payment : BaseModel<string>
{
    public decimal Amount { get; set; }
    public PaymentStatus Status { get; set; }
    public PaymentMethod Method { get; set; }
    public DateTime PaymentDate { get; set; }
    public string TransactionId { get; set; } = string.Empty;
    public DateTime? DueDate { get; set; }
    public string FinanceAccountId { get; set; } = string.Empty;
    public string BranchId { get; set; } = string.Empty;

    public int? SupplierId { get; set; }
    public Supplier Supplier { get; set; }

    [Timestamp]
    public byte[] RowVersion { get; set; }
    public string OrderId { get; set; } = string.Empty;
    public Order Order { get; set; }
}
