namespace Domain.Entities;

public class JournalEntryLine : BaseModel<string>
{
    public string JournalEntryId { get; set; } = string.Empty;
    public JournalEntry JournalEntry { get; set; }
    public string LedgerAccountId { get; set; } = string.Empty;
    public ChartOfAccount LedgerAccount { get; set; }
    public string? CostCenterId { get; set; }
    public CostCenter? CostCenter { get; set; }
    public string? BranchId { get; set; }
    public FinanceBranch? Branch { get; set; }
    public decimal DebitAmount { get; set; }
    public decimal CreditAmount { get; set; }
    public FinanceCurrency Currency { get; set; } = FinanceCurrency.IRR;
    public decimal ExchangeRateToBase { get; set; } = 1m;
    public string? Description { get; set; }
}
