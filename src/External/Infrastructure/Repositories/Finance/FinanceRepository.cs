namespace Infrastructure.Repositories;

public sealed class FinanceRepository : IFinanceRepository
{
    private readonly AppDbContext _context;

    public FinanceRepository(AppDbContext context)
    {
        _context = context;
    }

    public IQueryable<FinanceTenant> FinanceTenants => _context.FinanceTenants.AsNoTracking();
    public IQueryable<FinanceBranch> FinanceBranches => _context.FinanceBranches.AsNoTracking();
    public IQueryable<ChartOfAccount> ChartOfAccounts => _context.ChartOfAccounts.AsNoTracking();
    public IQueryable<FinanceAccount> FinanceAccounts => _context.FinanceAccounts.AsNoTracking();
    public IQueryable<FinancialTransaction> FinancialTransactions => _context.FinancialTransactions.AsNoTracking();
    public IQueryable<FinancialApprovalLog> FinancialApprovalLogs => _context.FinancialApprovalLogs.AsNoTracking();
    public IQueryable<RecurringFinancialObligation> RecurringFinancialObligations => _context.RecurringFinancialObligations.AsNoTracking();
    public IQueryable<PayrollRun> PayrollRuns => _context.PayrollRuns.AsNoTracking();
    public IQueryable<PayrollLine> PayrollLines => _context.PayrollLines.AsNoTracking();
    public IQueryable<JournalEntry> JournalEntries => _context.JournalEntries.AsNoTracking();

    public async Task AddTenantAsync(FinanceTenant tenant, CancellationToken cancellationToken = default) =>
        await _context.FinanceTenants.AddAsync(tenant, cancellationToken);

    public async Task AddBranchAsync(FinanceBranch branch, CancellationToken cancellationToken = default) =>
        await _context.FinanceBranches.AddAsync(branch, cancellationToken);

    public async Task AddChartOfAccountAsync(ChartOfAccount account, CancellationToken cancellationToken = default) =>
        await _context.ChartOfAccounts.AddAsync(account, cancellationToken);

    public async Task AddFinanceAccountAsync(FinanceAccount account, CancellationToken cancellationToken = default) =>
        await _context.FinanceAccounts.AddAsync(account, cancellationToken);

    public async Task AddTransactionAsync(FinancialTransaction transaction, CancellationToken cancellationToken = default) =>
        await _context.FinancialTransactions.AddAsync(transaction, cancellationToken);

    public async Task AddJournalEntryAsync(JournalEntry journalEntry, CancellationToken cancellationToken = default) =>
        await _context.JournalEntries.AddAsync(journalEntry, cancellationToken);

    public async Task AddApprovalLogAsync(FinancialApprovalLog approvalLog, CancellationToken cancellationToken = default) =>
        await _context.FinancialApprovalLogs.AddAsync(approvalLog, cancellationToken);

    public async Task<FinancialTransaction?> GetTransactionForUpdateAsync(string id, CancellationToken cancellationToken = default) =>
        await _context.FinancialTransactions.AsTracking().FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);

    public async Task<FinanceAccount?> GetFinanceAccountForUpdateAsync(string id, string tenantId, CancellationToken cancellationToken = default) =>
        await _context.FinanceAccounts.AsTracking().FirstOrDefaultAsync(x => x.Id == id && x.TenantId == tenantId && !x.IsDeleted, cancellationToken);
}
