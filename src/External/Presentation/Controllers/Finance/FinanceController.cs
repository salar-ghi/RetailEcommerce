namespace Presentation.Controllers;

[ApiController]
[Route("api/finance")]
public sealed class FinanceController : ControllerBase
{
    private readonly IFinanceService _financeService;

    public FinanceController(IFinanceService financeService)
    {
        _financeService = financeService;
    }

    [HttpPost("ensure-defaults")]
    public async Task<IActionResult> EnsureDefaults(CancellationToken cancellationToken)
    {
        await _financeService.EnsureDefaultsAsync(cancellationToken);
        return NoContent();
    }

    [HttpGet("branches")]
    public async Task<ActionResult<IReadOnlyList<FinanceBranchDto>>> GetBranches([FromQuery] string tenantId = FinanceDefaults.TenantId, CancellationToken cancellationToken = default)
        => Ok(await _financeService.GetBranchesAsync(tenantId, cancellationToken));

    [HttpGet("accounts")]
    public async Task<ActionResult<IReadOnlyList<FinanceAccountDto>>> GetAccounts([FromQuery] string tenantId = FinanceDefaults.TenantId, [FromQuery] string? branchId = null, CancellationToken cancellationToken = default)
        => Ok(await _financeService.GetAccountsAsync(tenantId, branchId, cancellationToken));

    [HttpGet("chart-of-accounts")]
    public async Task<ActionResult<IReadOnlyList<ChartOfAccountDto>>> GetChartOfAccounts([FromQuery] string tenantId = FinanceDefaults.TenantId, CancellationToken cancellationToken = default)
        => Ok(await _financeService.GetChartOfAccountsAsync(tenantId, cancellationToken));

    [HttpGet("transactions")]
    public async Task<ActionResult<IReadOnlyList<FinancialTransactionDto>>> GetTransactions(
        [FromQuery] string tenantId = FinanceDefaults.TenantId,
        [FromQuery] string? branchId = null,
        [FromQuery] FinancialTransactionStatus? status = null,
        CancellationToken cancellationToken = default)
        => Ok(await _financeService.GetTransactionsAsync(tenantId, branchId, status, cancellationToken));

    [HttpPost("transactions")]
    public async Task<ActionResult<FinancialTransactionDto>> CreateTransaction([FromBody] CreateFinancialTransactionDto request, CancellationToken cancellationToken)
    {
        var transaction = await _financeService.CreateTransactionAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetTransactions), new { tenantId = transaction.TenantId }, transaction);
    }

    [HttpPut("transactions/{id}")]
    public async Task<ActionResult<FinancialTransactionDto>> UpdateTransaction(string id, [FromBody] UpdateFinancialTransactionDto request, CancellationToken cancellationToken)
    {
        var transaction = await _financeService.UpdateTransactionAsync(id, request, cancellationToken);
        return transaction is null ? NotFound() : Ok(transaction);
    }

    [HttpPost("transactions/{id}/approve")]
    public async Task<ActionResult<FinancialTransactionDto>> ApproveTransaction(string id, [FromBody] ApprovalDecisionDto request, CancellationToken cancellationToken)
    {
        var transaction = await _financeService.ApproveTransactionAsync(id, request, cancellationToken);
        return transaction is null ? NotFound() : Ok(transaction);
    }

    [HttpPost("transactions/{id}/reject")]
    public async Task<ActionResult<FinancialTransactionDto>> RejectTransaction(string id, [FromBody] ApprovalDecisionDto request, CancellationToken cancellationToken)
    {
        var transaction = await _financeService.RejectTransactionAsync(id, request, cancellationToken);
        return transaction is null ? NotFound() : Ok(transaction);
    }

    [HttpGet("approval-logs")]
    public async Task<ActionResult<IReadOnlyList<FinancialApprovalLogDto>>> GetApprovalLogs([FromQuery] string tenantId = FinanceDefaults.TenantId, CancellationToken cancellationToken = default)
        => Ok(await _financeService.GetApprovalLogsAsync(tenantId, cancellationToken));

    [HttpGet("bills")]
    public async Task<ActionResult<IReadOnlyList<RecurringBillDto>>> GetBills([FromQuery] string tenantId = FinanceDefaults.TenantId, CancellationToken cancellationToken = default)
        => Ok(await _financeService.GetBillsAsync(tenantId, cancellationToken));

    [HttpGet("payroll")]
    public async Task<ActionResult<IReadOnlyList<PayrollLineDto>>> GetPayroll([FromQuery] string tenantId = FinanceDefaults.TenantId, CancellationToken cancellationToken = default)
        => Ok(await _financeService.GetPayrollAsync(tenantId, cancellationToken));

    [HttpGet("overview")]
    public async Task<ActionResult<FinanceOverviewDto>> GetOverview([FromQuery] string tenantId = FinanceDefaults.TenantId, [FromQuery] DateTime? from = null, [FromQuery] DateTime? to = null, CancellationToken cancellationToken = default)
        => Ok(await _financeService.GetOverviewAsync(tenantId, from, to, cancellationToken));

    [HttpGet("reports/cash-flow")]
    public async Task<ActionResult<IReadOnlyList<CashFlowPointDto>>> GetCashFlow([FromQuery] string tenantId = FinanceDefaults.TenantId, [FromQuery] int days = 14, CancellationToken cancellationToken = default)
        => Ok(await _financeService.GetCashFlowAsync(tenantId, days, cancellationToken));

    [HttpGet("reports/branch-performance")]
    public async Task<ActionResult<IReadOnlyList<BranchPerformanceDto>>> GetBranchPerformance([FromQuery] string tenantId = FinanceDefaults.TenantId, [FromQuery] DateTime? from = null, [FromQuery] DateTime? to = null, CancellationToken cancellationToken = default)
        => Ok(await _financeService.GetBranchPerformanceAsync(tenantId, from, to, cancellationToken));

    [HttpPost("source-documents/orders/post-payment")]
    public async Task<ActionResult<FinancialTransactionDto>> RecordOrderPayment([FromBody] RecordOrderFinanceDto request, CancellationToken cancellationToken)
        => Ok(await _financeService.RecordOrderPaymentAsync(request, cancellationToken));
}
