namespace Domain.Entities;

public class FinancialTransaction : BaseModel<string>
{
    public string TenantId { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public FinancialTransactionStatus Status { get; set; } = FinancialTransactionStatus.PendingApproval;
    public FinanceSourceDocumentType SourceDocumentType { get; set; }
    public string? SourceDocumentId { get; set; }
    public FinanceTransactionDirection Direction { get; set; }
    public decimal Amount { get; set; }
    public FinanceCurrency Currency { get; set; } = FinanceCurrency.IRR;
    public FinancePaymentMethod PaymentMethod { get; set; }
    public string FinanceAccountId { get; set; } = string.Empty;
    public FinanceAccount FinanceAccount { get; set; }
    public string? BranchId { get; set; }
    public FinanceBranch? Branch { get; set; }
    public string? CostCenterId { get; set; }
    public CostCenter? CostCenter { get; set; }
    public string? JournalEntryId { get; set; }
    public JournalEntry? JournalEntry { get; set; }
    public string? Category { get; set; }
    public string? CounterpartyId { get; set; }
    public string? CounterpartyName { get; set; }
    public string? Description { get; set; }
    public DateTime TransactionDate { get; set; }
    public string? ApprovedBy { get; set; }
    public DateTime? ApprovedOn { get; set; }
    public bool IsAutomated { get; set; }
}
