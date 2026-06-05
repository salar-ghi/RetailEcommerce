namespace Domain.Entities;

public class RecurringFinancialObligation : BaseModel<string>
{
    public string TenantId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public FinanceCurrency Currency { get; set; } = FinanceCurrency.IRR;
    public string? BranchId { get; set; }
    public FinanceBranch? Branch { get; set; }
    public string FinanceAccountId { get; set; } = string.Empty;
    public FinanceAccount FinanceAccount { get; set; }
    public RecurrenceCycle Cycle { get; set; }
    public DateTime NextDueDate { get; set; }
    public bool AutoPay { get; set; }
    public string? VendorName { get; set; }
    public bool IsActive { get; set; } = true;
}
