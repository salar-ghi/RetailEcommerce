namespace Application.Interfaces;

public interface IFinanceService
{
    Task EnsureDefaultsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<FinanceBranchDto>> GetBranchesAsync(string tenantId = FinanceDefaults.TenantId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<FinanceAccountDto>> GetAccountsAsync(string tenantId = FinanceDefaults.TenantId, string? branchId = null, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ChartOfAccountDto>> GetChartOfAccountsAsync(string tenantId = FinanceDefaults.TenantId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<FinancialTransactionDto>> GetTransactionsAsync(string tenantId = FinanceDefaults.TenantId, string? branchId = null, FinancialTransactionStatus? status = null, CancellationToken cancellationToken = default);
    Task<FinancialTransactionDto> CreateTransactionAsync(CreateFinancialTransactionDto request, CancellationToken cancellationToken = default);
    Task<FinancialTransactionDto?> UpdateTransactionAsync(string id, UpdateFinancialTransactionDto request, CancellationToken cancellationToken = default);
    Task<FinancialTransactionDto?> ApproveTransactionAsync(string id, ApprovalDecisionDto request, CancellationToken cancellationToken = default);
    Task<FinancialTransactionDto?> RejectTransactionAsync(string id, ApprovalDecisionDto request, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<FinancialApprovalLogDto>> GetApprovalLogsAsync(string tenantId = FinanceDefaults.TenantId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RecurringBillDto>> GetBillsAsync(string tenantId = FinanceDefaults.TenantId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PayrollLineDto>> GetPayrollAsync(string tenantId = FinanceDefaults.TenantId, CancellationToken cancellationToken = default);
    Task<FinanceOverviewDto> GetOverviewAsync(string tenantId = FinanceDefaults.TenantId, DateTime? from = null, DateTime? to = null, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CashFlowPointDto>> GetCashFlowAsync(string tenantId = FinanceDefaults.TenantId, int days = 14, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<BranchPerformanceDto>> GetBranchPerformanceAsync(string tenantId = FinanceDefaults.TenantId, DateTime? from = null, DateTime? to = null, CancellationToken cancellationToken = default);
    Task<FinancialTransactionDto> RecordOrderPaymentAsync(RecordOrderFinanceDto request, CancellationToken cancellationToken = default);
}
