namespace Domain.Entities;

public class PayrollRun : BaseModel<string>
{
    public string TenantId { get; set; } = string.Empty;
    public string PeriodCode { get; set; } = string.Empty;
    public string? BranchId { get; set; }
    public FinanceBranch? Branch { get; set; }
    public PayrollRunStatus Status { get; set; } = PayrollRunStatus.Draft;
    public FinanceCurrency Currency { get; set; } = FinanceCurrency.IRR;
    public DateTime? ScheduledPaymentDate { get; set; }
    public DateTime? PaidOn { get; set; }
    public string? JournalEntryId { get; set; }
    public JournalEntry? JournalEntry { get; set; }
    public ICollection<PayrollLine> Lines { get; set; } = new List<PayrollLine>();

    [NotMapped]
    public decimal TotalNetPay => Lines.Sum(line => line.NetPay);
}
