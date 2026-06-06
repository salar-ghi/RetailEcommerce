namespace Infrastructure.Services;

public sealed class FinanceService : IFinanceService
{
    private const string RevenueAccountCode = "4000";
    private const string ExpenseAccountCode = "5000";
    private const string InventoryAccountCode = "1300";
    private const string CashAccountCode = "1100";
    private readonly AppDbContext _context;

    public FinanceService(AppDbContext context)
    {
        _context = context;
    }

    public async Task EnsureDefaultsAsync(CancellationToken cancellationToken = default)
    {
        if (!await _context.FinanceTenants.AnyAsync(x => x.Id == FinanceDefaults.TenantId, cancellationToken))
        {
            _context.FinanceTenants.Add(new FinanceTenant
            {
                Id = FinanceDefaults.TenantId,
                Code = "DEFAULT",
                Name = "Default Retail Tenant",
                BaseCurrency = FinanceCurrency.IRR,
                IsActive = true,
                CreatedTime = DateTime.UtcNow,
                ModifiedTime = DateTime.UtcNow
            });
        }

        var accounts = new[]
        {
            (CashAccountCode, "Cash and Bank", LedgerAccountType.Asset, LedgerAccountNormalBalance.Debit),
            (InventoryAccountCode, "Inventory", LedgerAccountType.Asset, LedgerAccountNormalBalance.Debit),
            ("2100", "Accounts Payable", LedgerAccountType.Liability, LedgerAccountNormalBalance.Credit),
            ("2200", "Tax Payable", LedgerAccountType.Liability, LedgerAccountNormalBalance.Credit),
            ("3000", "Owner Equity", LedgerAccountType.Equity, LedgerAccountNormalBalance.Credit),
            (RevenueAccountCode, "Sales Revenue", LedgerAccountType.Revenue, LedgerAccountNormalBalance.Credit),
            ("4100", "Promotion Discounts", LedgerAccountType.ContraRevenue, LedgerAccountNormalBalance.Debit),
            (ExpenseAccountCode, "Operating Expense", LedgerAccountType.Expense, LedgerAccountNormalBalance.Debit),
            ("5100", "Payroll Expense", LedgerAccountType.Expense, LedgerAccountNormalBalance.Debit),
            ("5200", "Bill Expense", LedgerAccountType.Expense, LedgerAccountNormalBalance.Debit),
            ("5300", "Cost of Goods Sold", LedgerAccountType.Expense, LedgerAccountNormalBalance.Debit)
        };

        foreach (var account in accounts)
        {
            if (!await _context.ChartOfAccounts.AnyAsync(x => x.TenantId == FinanceDefaults.TenantId && x.Code == account.Item1, cancellationToken))
            {
                _context.ChartOfAccounts.Add(new ChartOfAccount
                {
                    Id = Guid.NewGuid().ToString(),
                    TenantId = FinanceDefaults.TenantId,
                    Code = account.Item1,
                    Name = account.Item2,
                    Type = account.Item3,
                    NormalBalance = account.Item4,
                    IsPostingAllowed = true,
                    IsSystemAccount = true,
                    IsActive = true,
                    CreatedTime = DateTime.UtcNow,
                    ModifiedTime = DateTime.UtcNow
                });
            }
        }

        if (!await _context.FinanceBranches.AnyAsync(x => x.Id == "default-branch", cancellationToken))
        {
            _context.FinanceBranches.Add(new FinanceBranch
            {
                Id = "default-branch",
                TenantId = FinanceDefaults.TenantId,
                Code = "MAIN",
                Name = "Main Store",
                Type = FinanceBranchType.Supermarket,
                City = "Tehran",
                IsActive = true,
                CreatedTime = DateTime.UtcNow,
                ModifiedTime = DateTime.UtcNow
            });
        }

        await _context.SaveChangesAsync(cancellationToken);

        var cashLedgerId = await _context.ChartOfAccounts
            .Where(x => x.TenantId == FinanceDefaults.TenantId && x.Code == CashAccountCode)
            .Select(x => x.Id)
            .FirstAsync(cancellationToken);

        if (!await _context.FinanceAccounts.AnyAsync(x => x.Id == "default-cash", cancellationToken))
        {
            _context.FinanceAccounts.Add(new FinanceAccount
            {
                Id = "default-cash",
                TenantId = FinanceDefaults.TenantId,
                BranchId = "default-branch",
                LedgerAccountId = cashLedgerId,
                Name = "Main Cash / Bank",
                Type = FinanceAccountType.Cash,
                Currency = FinanceCurrency.IRR,
                CurrentBalance = 0m,
                RequiresReconciliation = true,
                IsActive = true,
                CreatedTime = DateTime.UtcNow,
                ModifiedTime = DateTime.UtcNow
            });
        }

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<FinanceBranchDto>> GetBranchesAsync(string tenantId = FinanceDefaults.TenantId, CancellationToken cancellationToken = default)
    {
        await EnsureDefaultsAsync(cancellationToken);
        return await _context.FinanceBranches
            .Where(x => x.TenantId == tenantId && !x.IsDeleted)
            .OrderBy(x => x.Code)
            .Select(x => new FinanceBranchDto { Id = x.Id, TenantId = x.TenantId, Name = x.Name, Code = x.Code, Type = x.Type, City = x.City, ManagerUserId = x.ManagerUserId, IsActive = x.IsActive })
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<FinanceAccountDto>> GetAccountsAsync(string tenantId = FinanceDefaults.TenantId, string? branchId = null, CancellationToken cancellationToken = default)
    {
        await EnsureDefaultsAsync(cancellationToken);
        return await _context.FinanceAccounts
            .Where(x => x.TenantId == tenantId && !x.IsDeleted && (branchId == null || x.BranchId == branchId))
            .OrderBy(x => x.BranchId).ThenBy(x => x.Name)
            .Select(x => new FinanceAccountDto { Id = x.Id, TenantId = x.TenantId, BranchId = x.BranchId, LedgerAccountId = x.LedgerAccountId, Name = x.Name, Type = x.Type, Currency = x.Currency, CurrentBalance = x.CurrentBalance, BankName = x.BankName, IsActive = x.IsActive })
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ChartOfAccountDto>> GetChartOfAccountsAsync(string tenantId = FinanceDefaults.TenantId, CancellationToken cancellationToken = default)
    {
        await EnsureDefaultsAsync(cancellationToken);
        return await _context.ChartOfAccounts
            .Where(x => x.TenantId == tenantId && !x.IsDeleted)
            .OrderBy(x => x.Code)
            .Select(x => new ChartOfAccountDto { Id = x.Id, TenantId = x.TenantId, Code = x.Code, Name = x.Name, Type = x.Type, NormalBalance = x.NormalBalance, ParentAccountId = x.ParentAccountId, IsPostingAllowed = x.IsPostingAllowed, IsSystemAccount = x.IsSystemAccount, IsActive = x.IsActive })
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<FinancialTransactionDto>> GetTransactionsAsync(string tenantId = FinanceDefaults.TenantId, string? branchId = null, FinancialTransactionStatus? status = null, CancellationToken cancellationToken = default)
    {
        await EnsureDefaultsAsync(cancellationToken);
        return await _context.FinancialTransactions
            .Where(x => x.TenantId == tenantId && !x.IsDeleted && (branchId == null || x.BranchId == branchId) && (status == null || x.Status == status))
            .OrderByDescending(x => x.TransactionDate)
            .ThenByDescending(x => x.CreatedTime)
            .Select(x => ToTransactionDto(x))
            .ToListAsync(cancellationToken);
    }

    public async Task<FinancialTransactionDto> CreateTransactionAsync(CreateFinancialTransactionDto request, CancellationToken cancellationToken = default)
    {
        await EnsureDefaultsAsync(cancellationToken);
        ValidateTransactionRequest(request);

        var amount = FinanceMoney.Normalize(request.Amount, request.Currency);
        var status = request.AutoPost ? FinancialTransactionStatus.Completed : FinancialTransactionStatus.PendingApproval;
        var tx = new FinancialTransaction
        {
            Id = Guid.NewGuid().ToString(),
            TenantId = request.TenantId,
            Code = await NextTransactionCodeAsync(request.TenantId, cancellationToken),
            Status = status,
            SourceDocumentType = request.SourceDocumentType,
            SourceDocumentId = request.SourceDocumentId,
            Direction = request.Direction,
            Amount = amount,
            Currency = request.Currency,
            PaymentMethod = request.PaymentMethod,
            FinanceAccountId = request.FinanceAccountId,
            BranchId = request.BranchId,
            CostCenterId = request.CostCenterId,
            Category = request.Category,
            CounterpartyId = request.CounterpartyId,
            CounterpartyName = request.CounterpartyName,
            Description = request.Description,
            TransactionDate = request.TransactionDate ?? DateTime.UtcNow,
            IsAutomated = request.SourceDocumentType != FinanceSourceDocumentType.ManualJournal,
            CreatedTime = DateTime.UtcNow,
            ModifiedTime = DateTime.UtcNow
        };

        _context.FinancialTransactions.Add(tx);

        if (request.AutoPost)
        {
            await PostTransactionAsync(tx, null, cancellationToken);
        }

        await _context.SaveChangesAsync(cancellationToken);
        return ToTransactionDto(tx);
    }

    public async Task<FinancialTransactionDto?> UpdateTransactionAsync(string id, UpdateFinancialTransactionDto request, CancellationToken cancellationToken = default)
    {
        var tx = await _context.FinancialTransactions.AsTracking().FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
        if (tx is null) return null;
        if (tx.Status == FinancialTransactionStatus.Completed || tx.Status == FinancialTransactionStatus.Reversed)
        {
            throw new InvalidOperationException("Posted financial transactions cannot be edited; create a reversal instead.");
        }

        ValidateTransactionRequest(request);
        tx.SourceDocumentType = request.SourceDocumentType;
        tx.SourceDocumentId = request.SourceDocumentId;
        tx.Direction = request.Direction;
        tx.Amount = FinanceMoney.Normalize(request.Amount, request.Currency);
        tx.Currency = request.Currency;
        tx.PaymentMethod = request.PaymentMethod;
        tx.FinanceAccountId = request.FinanceAccountId;
        tx.BranchId = request.BranchId;
        tx.CostCenterId = request.CostCenterId;
        tx.Category = request.Category;
        tx.CounterpartyId = request.CounterpartyId;
        tx.CounterpartyName = request.CounterpartyName;
        tx.Description = request.Description;
        tx.TransactionDate = request.TransactionDate ?? tx.TransactionDate;
        tx.Status = request.Status ?? (request.AutoPost ? FinancialTransactionStatus.Completed : FinancialTransactionStatus.PendingApproval);
        tx.ModifiedTime = DateTime.UtcNow;

        if (tx.Status == FinancialTransactionStatus.Completed)
        {
            await PostTransactionAsync(tx, null, cancellationToken);
        }

        await _context.SaveChangesAsync(cancellationToken);
        return ToTransactionDto(tx);
    }

    public async Task<FinancialTransactionDto?> ApproveTransactionAsync(string id, ApprovalDecisionDto request, CancellationToken cancellationToken = default)
    {
        var tx = await _context.FinancialTransactions.AsTracking().FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
        if (tx is null) return null;
        if (tx.Status != FinancialTransactionStatus.PendingApproval && tx.Status != FinancialTransactionStatus.Scheduled)
        {
            throw new InvalidOperationException("Only pending or scheduled transactions can be approved.");
        }

        tx.Status = FinancialTransactionStatus.Completed;
        tx.ApprovedBy = request.ApproverUserId;
        tx.ApprovedOn = DateTime.UtcNow;
        tx.ModifiedTime = DateTime.UtcNow;
        _context.FinancialApprovalLogs.Add(CreateApprovalLog(tx, request, ApprovalDecision.Approved));
        await PostTransactionAsync(tx, request.ApproverUserId, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return ToTransactionDto(tx);
    }

    public async Task<FinancialTransactionDto?> RejectTransactionAsync(string id, ApprovalDecisionDto request, CancellationToken cancellationToken = default)
    {
        var tx = await _context.FinancialTransactions.AsTracking().FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
        if (tx is null) return null;
        tx.Status = FinancialTransactionStatus.Rejected;
        tx.ApprovedBy = request.ApproverUserId;
        tx.ApprovedOn = DateTime.UtcNow;
        tx.ModifiedTime = DateTime.UtcNow;
        _context.FinancialApprovalLogs.Add(CreateApprovalLog(tx, request, ApprovalDecision.Rejected));
        await _context.SaveChangesAsync(cancellationToken);
        return ToTransactionDto(tx);
    }

    public async Task<IReadOnlyList<FinancialApprovalLogDto>> GetApprovalLogsAsync(string tenantId = FinanceDefaults.TenantId, CancellationToken cancellationToken = default)
    {
        return await _context.FinancialApprovalLogs
            .Where(x => x.TenantId == tenantId && !x.IsDeleted)
            .OrderByDescending(x => x.DecisionDate)
            .Select(x => new FinancialApprovalLogDto { Id = x.Id, TenantId = x.TenantId, EntityName = x.EntityName, EntityId = x.EntityId, ApproverUserId = x.ApproverUserId, Decision = x.Decision, Note = x.Note, DecisionDate = x.DecisionDate, AmountThreshold = x.AmountThreshold })
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<RecurringBillDto>> GetBillsAsync(string tenantId = FinanceDefaults.TenantId, CancellationToken cancellationToken = default)
    {
        await EnsureDefaultsAsync(cancellationToken);
        return await _context.RecurringFinancialObligations
            .Where(x => x.TenantId == tenantId && !x.IsDeleted)
            .OrderBy(x => x.NextDueDate)
            .Select(x => new RecurringBillDto { Id = x.Id, TenantId = x.TenantId, Name = x.Name, Category = x.Category, Amount = x.Amount, Currency = x.Currency, BranchId = x.BranchId, FinanceAccountId = x.FinanceAccountId, Cycle = x.Cycle, NextDueDate = x.NextDueDate, AutoPay = x.AutoPay, VendorName = x.VendorName, IsActive = x.IsActive })
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<PayrollLineDto>> GetPayrollAsync(string tenantId = FinanceDefaults.TenantId, CancellationToken cancellationToken = default)
    {
        return await _context.PayrollLines
            .Join(_context.PayrollRuns.Where(r => r.TenantId == tenantId && !r.IsDeleted), line => line.PayrollRunId, run => run.Id, (line, run) => new { line, run })
            .Where(x => !x.line.IsDeleted)
            .OrderByDescending(x => x.run.CreatedTime)
            .ThenBy(x => x.line.EmployeeName)
            .Select(x => new PayrollLineDto { Id = x.line.Id, PayrollRunId = x.line.PayrollRunId, EmployeeId = x.line.EmployeeId, EmployeeName = x.line.EmployeeName, Position = x.line.Position, BranchId = x.line.BranchId, BaseSalary = x.line.BaseSalary, Bonus = x.line.Bonus, Deductions = x.line.Deductions, TaxWithheld = x.line.TaxWithheld, InsuranceWithheld = x.line.InsuranceWithheld, NetPay = x.line.NetPay, Status = x.run.Status })
            .ToListAsync(cancellationToken);
    }

    public async Task<FinanceOverviewDto> GetOverviewAsync(string tenantId = FinanceDefaults.TenantId, DateTime? from = null, DateTime? to = null, CancellationToken cancellationToken = default)
    {
        var tx = FilterCompletedTransactions(tenantId, from, to);
        var income = await tx.Where(x => x.Direction == FinanceTransactionDirection.Debit).SumAsync(x => x.Amount, cancellationToken);
        var expense = await tx.Where(x => x.Direction == FinanceTransactionDirection.Credit).SumAsync(x => x.Amount, cancellationToken);
        var pending = _context.FinancialTransactions.Where(x => x.TenantId == tenantId && x.Status == FinancialTransactionStatus.PendingApproval && !x.IsDeleted);
        return new FinanceOverviewDto
        {
            TotalIncome = income,
            TotalExpense = expense,
            CashOnHand = await _context.FinanceAccounts.Where(x => x.TenantId == tenantId && x.IsActive && !x.IsDeleted).SumAsync(x => x.CurrentBalance, cancellationToken),
            PendingApprovals = await pending.CountAsync(cancellationToken),
            PendingApprovalAmount = await pending.SumAsync(x => x.Amount, cancellationToken),
            ScheduledPayments = await _context.FinancialTransactions.CountAsync(x => x.TenantId == tenantId && x.Status == FinancialTransactionStatus.Scheduled && !x.IsDeleted, cancellationToken)
        };
    }

    public async Task<IReadOnlyList<CashFlowPointDto>> GetCashFlowAsync(string tenantId = FinanceDefaults.TenantId, int days = 14, CancellationToken cancellationToken = default)
    {
        days = Math.Clamp(days, 1, 90);
        var startDate = DateTime.UtcNow.Date.AddDays(-(days - 1));
        var grouped = await _context.FinancialTransactions
            .Where(x => x.TenantId == tenantId && x.Status == FinancialTransactionStatus.Completed && x.TransactionDate >= startDate && !x.IsDeleted)
            .GroupBy(x => x.TransactionDate.Date)
            .Select(g => new { Date = g.Key, Income = g.Where(x => x.Direction == FinanceTransactionDirection.Debit).Sum(x => x.Amount), Expense = g.Where(x => x.Direction == FinanceTransactionDirection.Credit).Sum(x => x.Amount) })
            .ToListAsync(cancellationToken);

        return Enumerable.Range(0, days)
            .Select(offset => DateOnly.FromDateTime(startDate.AddDays(offset)))
            .Select(day =>
            {
                var row = grouped.FirstOrDefault(x => DateOnly.FromDateTime(x.Date) == day);
                return new CashFlowPointDto { Date = day, Income = row?.Income ?? 0m, Expense = row?.Expense ?? 0m };
            })
            .ToList();
    }

    public async Task<IReadOnlyList<BranchPerformanceDto>> GetBranchPerformanceAsync(string tenantId = FinanceDefaults.TenantId, DateTime? from = null, DateTime? to = null, CancellationToken cancellationToken = default)
    {
        var branches = await _context.FinanceBranches.Where(x => x.TenantId == tenantId && !x.IsDeleted).ToListAsync(cancellationToken);
        var grouped = await FilterCompletedTransactions(tenantId, from, to)
            .GroupBy(x => x.BranchId)
            .Select(g => new { BranchId = g.Key, Income = g.Where(x => x.Direction == FinanceTransactionDirection.Debit).Sum(x => x.Amount), Expense = g.Where(x => x.Direction == FinanceTransactionDirection.Credit).Sum(x => x.Amount) })
            .ToListAsync(cancellationToken);

        return branches.Select(branch =>
        {
            var row = grouped.FirstOrDefault(x => x.BranchId == branch.Id);
            return new BranchPerformanceDto { BranchId = branch.Id, BranchName = branch.Name, Income = row?.Income ?? 0m, Expense = row?.Expense ?? 0m };
        }).ToList();
    }

    public async Task<FinancialTransactionDto> RecordOrderPaymentAsync(RecordOrderFinanceDto request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.OrderId)) throw new ArgumentException("Order id is required.", nameof(request));
        var order = await _context.Orders.Include(x => x.Items).FirstOrDefaultAsync(x => x.Id == request.OrderId && !x.IsDeleted, cancellationToken)
            ?? throw new KeyNotFoundException($"Order {request.OrderId} was not found.");
        if (order.TotalAmount <= 0m) throw new InvalidOperationException("Cannot post a zero or negative order total.");

        return await CreateTransactionAsync(new CreateFinancialTransactionDto
        {
            TenantId = request.TenantId,
            SourceDocumentType = FinanceSourceDocumentType.SalesOrder,
            SourceDocumentId = order.Id,
            Direction = FinanceTransactionDirection.Debit,
            Amount = order.TotalAmount,
            Currency = FinanceCurrency.IRR,
            PaymentMethod = request.PaymentMethod,
            FinanceAccountId = request.FinanceAccountId,
            BranchId = request.BranchId,
            Category = "Sales",
            CounterpartyId = request.CounterpartyId ?? order.CustomerId,
            CounterpartyName = request.CounterpartyName,
            Description = $"Sales order {order.Id}",
            TransactionDate = order.PaymentDate ?? DateTime.UtcNow,
            AutoPost = true
        }, cancellationToken);
    }

    private IQueryable<FinancialTransaction> FilterCompletedTransactions(string tenantId, DateTime? from, DateTime? to)
    {
        return _context.FinancialTransactions.Where(x => x.TenantId == tenantId && x.Status == FinancialTransactionStatus.Completed && !x.IsDeleted && (from == null || x.TransactionDate >= from) && (to == null || x.TransactionDate <= to));
    }

    private async Task PostTransactionAsync(FinancialTransaction tx, string? userId, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrEmpty(tx.JournalEntryId)) return;

        var financeAccount = await _context.FinanceAccounts.AsTracking().FirstOrDefaultAsync(x => x.Id == tx.FinanceAccountId && x.TenantId == tx.TenantId && !x.IsDeleted, cancellationToken)
            ?? throw new KeyNotFoundException($"Finance account {tx.FinanceAccountId} was not found.");
        if (financeAccount.Currency != tx.Currency)
        {
            throw new InvalidOperationException("Transaction currency must match the finance account currency.");
        }

        var offsetAccount = await ResolveOffsetLedgerAccountAsync(tx, cancellationToken);
        var journal = new JournalEntry
        {
            Id = Guid.NewGuid().ToString(),
            TenantId = tx.TenantId,
            JournalNumber = await NextJournalNumberAsync(tx.TenantId, cancellationToken),
            AccountingDate = tx.TransactionDate,
            Status = JournalEntryStatus.Posted,
            SourceDocumentType = tx.SourceDocumentType,
            SourceDocumentId = tx.SourceDocumentId,
            BranchId = tx.BranchId,
            Description = tx.Description,
            PostedOn = DateTime.UtcNow,
            PostedBy = userId,
            CreatedTime = DateTime.UtcNow,
            ModifiedTime = DateTime.UtcNow
        };

        var cashDebit = tx.Direction == FinanceTransactionDirection.Debit ? tx.Amount : 0m;
        var cashCredit = tx.Direction == FinanceTransactionDirection.Credit ? tx.Amount : 0m;
        var offsetDebit = tx.Direction == FinanceTransactionDirection.Credit ? tx.Amount : 0m;
        var offsetCredit = tx.Direction == FinanceTransactionDirection.Debit ? tx.Amount : 0m;

        journal.Lines.Add(CreateJournalLine(journal.Id, financeAccount.LedgerAccountId, tx, cashDebit, cashCredit, tx.Description));
        journal.Lines.Add(CreateJournalLine(journal.Id, offsetAccount.Id, tx, offsetDebit, offsetCredit, tx.Category));
        _context.JournalEntries.Add(journal);

        financeAccount.CurrentBalance = FinanceMoney.NormalizeSigned(financeAccount.CurrentBalance + (tx.Direction == FinanceTransactionDirection.Debit ? tx.Amount : -tx.Amount), financeAccount.Currency);
        financeAccount.ModifiedTime = DateTime.UtcNow;
        tx.JournalEntryId = journal.Id;
        tx.Status = FinancialTransactionStatus.Completed;
    }

    private static JournalEntryLine CreateJournalLine(string journalId, string ledgerAccountId, FinancialTransaction tx, decimal debit, decimal credit, string? description)
    {
        return new JournalEntryLine
        {
            Id = Guid.NewGuid().ToString(),
            JournalEntryId = journalId,
            LedgerAccountId = ledgerAccountId,
            CostCenterId = tx.CostCenterId,
            BranchId = tx.BranchId,
            DebitAmount = debit,
            CreditAmount = credit,
            Currency = tx.Currency,
            ExchangeRateToBase = 1m,
            Description = description,
            CreatedTime = DateTime.UtcNow,
            ModifiedTime = DateTime.UtcNow
        };
    }

    private async Task<ChartOfAccount> ResolveOffsetLedgerAccountAsync(FinancialTransaction tx, CancellationToken cancellationToken)
    {
        var code = tx.SourceDocumentType switch
        {
            FinanceSourceDocumentType.SalesOrder or FinanceSourceDocumentType.CustomerRefund => RevenueAccountCode,
            FinanceSourceDocumentType.InventoryAdjustment or FinanceSourceDocumentType.PurchaseOrder or FinanceSourceDocumentType.SupplierInvoice => InventoryAccountCode,
            FinanceSourceDocumentType.Payroll => "5100",
            FinanceSourceDocumentType.RecurringBill => "5200",
            FinanceSourceDocumentType.TaxSettlement => "2200",
            _ => tx.Direction == FinanceTransactionDirection.Debit ? RevenueAccountCode : ExpenseAccountCode
        };

        return await _context.ChartOfAccounts.FirstAsync(x => x.TenantId == tx.TenantId && x.Code == code && !x.IsDeleted, cancellationToken);
    }

    private static FinancialApprovalLog CreateApprovalLog(FinancialTransaction tx, ApprovalDecisionDto request, ApprovalDecision decision) => new()
    {
        Id = Guid.NewGuid().ToString(),
        TenantId = tx.TenantId,
        EntityName = nameof(FinancialTransaction),
        EntityId = tx.Id,
        ApproverUserId = request.ApproverUserId,
        Decision = decision,
        Note = request.Note,
        DecisionDate = DateTime.UtcNow,
        AmountThreshold = tx.Amount,
        CreatedTime = DateTime.UtcNow,
        ModifiedTime = DateTime.UtcNow
    };

    private async Task<string> NextTransactionCodeAsync(string tenantId, CancellationToken cancellationToken)
    {
        var count = await _context.FinancialTransactions.CountAsync(x => x.TenantId == tenantId, cancellationToken) + 1;
        return $"FT-{DateTime.UtcNow:yyyyMMdd}-{count:000000}";
    }

    private async Task<string> NextJournalNumberAsync(string tenantId, CancellationToken cancellationToken)
    {
        var count = await _context.JournalEntries.CountAsync(x => x.TenantId == tenantId, cancellationToken) + 1;
        return $"JE-{DateTime.UtcNow:yyyyMMdd}-{count:000000}";
    }

    private static void ValidateTransactionRequest(CreateFinancialTransactionDto request)
    {
        if (string.IsNullOrWhiteSpace(request.TenantId)) throw new ArgumentException("Tenant id is required.");
        if (string.IsNullOrWhiteSpace(request.FinanceAccountId)) throw new ArgumentException("Finance account id is required.");
        _ = FinanceMoney.Normalize(request.Amount, request.Currency);
    }

    private static FinancialTransactionDto ToTransactionDto(FinancialTransaction x) => new()
    {
        Id = x.Id,
        TenantId = x.TenantId,
        Code = x.Code,
        Status = x.Status,
        SourceDocumentType = x.SourceDocumentType,
        SourceDocumentId = x.SourceDocumentId,
        Direction = x.Direction,
        Amount = x.Amount,
        Currency = x.Currency,
        PaymentMethod = x.PaymentMethod,
        FinanceAccountId = x.FinanceAccountId,
        BranchId = x.BranchId,
        CostCenterId = x.CostCenterId,
        JournalEntryId = x.JournalEntryId,
        Category = x.Category,
        CounterpartyId = x.CounterpartyId,
        CounterpartyName = x.CounterpartyName,
        Description = x.Description,
        TransactionDate = x.TransactionDate,
        ApprovedBy = x.ApprovedBy,
        ApprovedOn = x.ApprovedOn,
        IsAutomated = x.IsAutomated
    };
}
