namespace Domain.Entities;

public class FinanceAccount : BaseModel<string>
{
    public string TenantId { get; set; } = string.Empty;
    public string? BranchId { get; set; }
    public FinanceBranch? Branch { get; set; }
    public string LedgerAccountId { get; set; } = string.Empty;
    public ChartOfAccount LedgerAccount { get; set; }
    public string Name { get; set; } = string.Empty;
    public FinanceAccountType Type { get; set; }
    public FinanceCurrency Currency { get; set; } = FinanceCurrency.IRR;
    public decimal CurrentBalance { get; set; }
    public string? BankName { get; set; }
    public string? AccountNumberMasked { get; set; }
    public bool RequiresReconciliation { get; set; } = true;
    public bool IsActive { get; set; } = true;
}
