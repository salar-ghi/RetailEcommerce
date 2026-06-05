namespace Application.DTOs;

public sealed record FinanceMoneyDto(decimal Amount, FinanceCurrency Currency = FinanceCurrency.IRR);

public sealed class FinanceBranchDto
{
    public string Id { get; set; } = string.Empty;
    public string TenantId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public FinanceBranchType Type { get; set; }
    public string? City { get; set; }
    public string? ManagerUserId { get; set; }
    public bool IsActive { get; set; }
}

public sealed class FinanceAccountDto
{
    public string Id { get; set; } = string.Empty;
    public string TenantId { get; set; } = string.Empty;
    public string? BranchId { get; set; }
    public string LedgerAccountId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public FinanceAccountType Type { get; set; }
    public FinanceCurrency Currency { get; set; }
    public decimal CurrentBalance { get; set; }
    public string? BankName { get; set; }
    public bool IsActive { get; set; }
}

public sealed class ChartOfAccountDto
{
    public string Id { get; set; } = string.Empty;
    public string TenantId { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public LedgerAccountType Type { get; set; }
    public LedgerAccountNormalBalance NormalBalance { get; set; }
    public string? ParentAccountId { get; set; }
    public bool IsPostingAllowed { get; set; }
    public bool IsSystemAccount { get; set; }
    public bool IsActive { get; set; }
}

public sealed class FinancialTransactionDto
{
    public string Id { get; set; } = string.Empty;
    public string TenantId { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public FinancialTransactionStatus Status { get; set; }
    public FinanceSourceDocumentType SourceDocumentType { get; set; }
    public string? SourceDocumentId { get; set; }
    public FinanceTransactionDirection Direction { get; set; }
    public decimal Amount { get; set; }
    public FinanceCurrency Currency { get; set; }
    public FinancePaymentMethod PaymentMethod { get; set; }
    public string FinanceAccountId { get; set; } = string.Empty;
    public string? BranchId { get; set; }
    public string? CostCenterId { get; set; }
    public string? JournalEntryId { get; set; }
    public string? Category { get; set; }
    public string? CounterpartyId { get; set; }
    public string? CounterpartyName { get; set; }
    public string? Description { get; set; }
    public DateTime TransactionDate { get; set; }
    public string? ApprovedBy { get; set; }
    public DateTime? ApprovedOn { get; set; }
    public bool IsAutomated { get; set; }
}

public sealed class CreateFinancialTransactionDto
{
    public string TenantId { get; set; } = FinanceDefaults.TenantId;
    public FinanceSourceDocumentType SourceDocumentType { get; set; } = FinanceSourceDocumentType.ManualJournal;
    public string? SourceDocumentId { get; set; }
    public FinanceTransactionDirection Direction { get; set; }
    public decimal Amount { get; set; }
    public FinanceCurrency Currency { get; set; } = FinanceCurrency.IRR;
    public FinancePaymentMethod PaymentMethod { get; set; } = FinancePaymentMethod.Cash;
    public string FinanceAccountId { get; set; } = string.Empty;
    public string? BranchId { get; set; }
    public string? CostCenterId { get; set; }
    public string? Category { get; set; }
    public string? CounterpartyId { get; set; }
    public string? CounterpartyName { get; set; }
    public string? Description { get; set; }
    public DateTime? TransactionDate { get; set; }
    public bool AutoPost { get; set; } = true;
}

public sealed class UpdateFinancialTransactionDto : CreateFinancialTransactionDto
{
    public FinancialTransactionStatus? Status { get; set; }
}

public sealed class ApprovalDecisionDto
{
    public string ApproverUserId { get; set; } = string.Empty;
    public string? Note { get; set; }
}

public sealed class FinancialApprovalLogDto
{
    public string Id { get; set; } = string.Empty;
    public string TenantId { get; set; } = string.Empty;
    public string EntityName { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public string ApproverUserId { get; set; } = string.Empty;
    public ApprovalDecision Decision { get; set; }
    public string? Note { get; set; }
    public DateTime DecisionDate { get; set; }
    public decimal? AmountThreshold { get; set; }
}

public sealed class RecurringBillDto
{
    public string Id { get; set; } = string.Empty;
    public string TenantId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public FinanceCurrency Currency { get; set; }
    public string? BranchId { get; set; }
    public string FinanceAccountId { get; set; } = string.Empty;
    public RecurrenceCycle Cycle { get; set; }
    public DateTime NextDueDate { get; set; }
    public bool AutoPay { get; set; }
    public string? VendorName { get; set; }
    public bool IsActive { get; set; }
}

public sealed class PayrollLineDto
{
    public string Id { get; set; } = string.Empty;
    public string PayrollRunId { get; set; } = string.Empty;
    public string EmployeeId { get; set; } = string.Empty;
    public string EmployeeName { get; set; } = string.Empty;
    public string? Position { get; set; }
    public string? BranchId { get; set; }
    public decimal BaseSalary { get; set; }
    public decimal Bonus { get; set; }
    public decimal Deductions { get; set; }
    public decimal TaxWithheld { get; set; }
    public decimal InsuranceWithheld { get; set; }
    public decimal NetPay { get; set; }
    public PayrollRunStatus Status { get; set; }
}

public sealed class CashFlowPointDto
{
    public DateOnly Date { get; set; }
    public decimal Income { get; set; }
    public decimal Expense { get; set; }
    public decimal Net => Income - Expense;
}

public sealed class BranchPerformanceDto
{
    public string? BranchId { get; set; }
    public string BranchName { get; set; } = string.Empty;
    public decimal Income { get; set; }
    public decimal Expense { get; set; }
    public decimal Profit => Income - Expense;
}

public sealed class FinanceOverviewDto
{
    public decimal TotalIncome { get; set; }
    public decimal TotalExpense { get; set; }
    public decimal NetProfit => TotalIncome - TotalExpense;
    public decimal CashOnHand { get; set; }
    public int PendingApprovals { get; set; }
    public decimal PendingApprovalAmount { get; set; }
    public int ScheduledPayments { get; set; }
}

public sealed class RecordOrderFinanceDto
{
    public string TenantId { get; set; } = FinanceDefaults.TenantId;
    public string OrderId { get; set; } = string.Empty;
    public string FinanceAccountId { get; set; } = string.Empty;
    public string? BranchId { get; set; }
    public FinancePaymentMethod PaymentMethod { get; set; } = FinancePaymentMethod.OnlineGateway;
    public string? CounterpartyId { get; set; }
    public string? CounterpartyName { get; set; }
}

public static class FinanceDefaults
{
    public const string TenantId = "default-tenant";
}
