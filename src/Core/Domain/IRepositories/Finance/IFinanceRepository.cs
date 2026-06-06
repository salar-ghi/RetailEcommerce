namespace Domain.IRepositories;

public interface IFinanceRepository
{
    IQueryable<FinanceTenant> FinanceTenants { get; }
    IQueryable<FinanceBranch> FinanceBranches { get; }
    IQueryable<ChartOfAccount> ChartOfAccounts { get; }
    IQueryable<FinanceAccount> FinanceAccounts { get; }
    IQueryable<FinancialTransaction> FinancialTransactions { get; }
    IQueryable<FinancialApprovalLog> FinancialApprovalLogs { get; }
    IQueryable<RecurringFinancialObligation> RecurringFinancialObligations { get; }
    IQueryable<PayrollRun> PayrollRuns { get; }
    IQueryable<PayrollLine> PayrollLines { get; }
    IQueryable<JournalEntry> JournalEntries { get; }

    Task AddTenantAsync(FinanceTenant tenant, CancellationToken cancellationToken = default);
    Task AddBranchAsync(FinanceBranch branch, CancellationToken cancellationToken = default);
    Task AddChartOfAccountAsync(ChartOfAccount account, CancellationToken cancellationToken = default);
    Task AddFinanceAccountAsync(FinanceAccount account, CancellationToken cancellationToken = default);
    Task AddTransactionAsync(FinancialTransaction transaction, CancellationToken cancellationToken = default);
    Task AddJournalEntryAsync(JournalEntry journalEntry, CancellationToken cancellationToken = default);
    Task AddApprovalLogAsync(FinancialApprovalLog approvalLog, CancellationToken cancellationToken = default);
    Task<FinancialTransaction?> GetTransactionForUpdateAsync(string id, CancellationToken cancellationToken = default);
    Task<FinanceAccount?> GetFinanceAccountForUpdateAsync(string id, string tenantId, CancellationToken cancellationToken = default);
}
