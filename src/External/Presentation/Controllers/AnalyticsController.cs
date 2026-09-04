namespace Presentation.Controllers;

/// <summary>Admin dashboard analytics. Every endpoint accepts range, categoryId, and brandId.</summary>
[Route("api/[controller]")]
[ApiController]
public sealed class AnalyticsController : ControllerBase
{
    private readonly IAnalyticsService _analyticsService;
    public AnalyticsController(IAnalyticsService analyticsService) => _analyticsService = analyticsService;

    [HttpGet("overview")]
    public Task<AnalyticsOverviewDto> Overview([FromQuery] AnalyticsQueryDto query, CancellationToken cancellationToken)
        => _analyticsService.GetOverviewAsync(query, cancellationToken);

    [HttpGet("kpis")] public async Task<ActionResult<AnalyticsKpisDto>> Kpis([FromQuery] AnalyticsQueryDto q, CancellationToken ct) => Ok((await Overview(q, ct)).Kpis);
    [HttpGet("sales")] public async Task<ActionResult<IReadOnlyList<SalesPointDto>>> Sales([FromQuery] AnalyticsQueryDto q, CancellationToken ct) => Ok((await Overview(q, ct)).Sales);
    [HttpGet("revenue-comparison")] public async Task<ActionResult<IReadOnlyList<RevenueComparisonPointDto>>> RevenueComparison([FromQuery] AnalyticsQueryDto q, CancellationToken ct) => Ok((await Overview(q, ct)).RevenueComparison);
    [HttpGet("profit-margin")] public async Task<ActionResult<IReadOnlyList<ProfitMarginPointDto>>> ProfitMargin([FromQuery] AnalyticsQueryDto q, CancellationToken ct) => Ok((await Overview(q, ct)).ProfitMargin);
    [HttpGet("growth-trend")] public async Task<ActionResult<IReadOnlyList<GrowthTrendPointDto>>> GrowthTrend([FromQuery] AnalyticsQueryDto q, CancellationToken ct) => Ok((await Overview(q, ct)).GrowthTrend);
    [HttpGet("category-distribution")] public async Task<ActionResult<IReadOnlyList<NamedValueDto>>> CategoryDistribution([FromQuery] AnalyticsQueryDto q, CancellationToken ct) => Ok((await Overview(q, ct)).CategoryDistribution);
    [HttpGet("top-products")] public async Task<ActionResult<IReadOnlyList<TopProductDto>>> TopProducts([FromQuery] AnalyticsQueryDto q, CancellationToken ct) => Ok((await Overview(q, ct)).TopProducts);
    [HttpGet("brand-performance")] public async Task<ActionResult<IReadOnlyList<BrandPerformanceDto>>> BrandPerformance([FromQuery] AnalyticsQueryDto q, CancellationToken ct) => Ok((await Overview(q, ct)).BrandPerformance);
    [HttpGet("supplier-stats")] public async Task<ActionResult<IReadOnlyList<SupplierStatDto>>> SupplierStats([FromQuery] AnalyticsQueryDto q, CancellationToken ct) => Ok((await Overview(q, ct)).SupplierStats);
    [HttpGet("inventory-status")] public async Task<ActionResult<IReadOnlyList<NamedValueDto>>> InventoryStatus([FromQuery] AnalyticsQueryDto q, CancellationToken ct) => Ok((await Overview(q, ct)).InventoryStatus);
    [HttpGet("orders-heatmap")] public async Task<ActionResult<IReadOnlyList<HeatmapRowDto>>> OrdersHeatmap([FromQuery] AnalyticsQueryDto q, CancellationToken ct) => Ok((await Overview(q, ct)).OrdersHeatmap);
    [HttpGet("customer-segments")] public async Task<ActionResult<IReadOnlyList<CustomerSegmentStatDto>>> CustomerSegments([FromQuery] AnalyticsQueryDto q, CancellationToken ct) => Ok((await Overview(q, ct)).CustomerSegments);
    [HttpGet("order-status")] public async Task<ActionResult<IReadOnlyList<NamedValueDto>>> OrderStatus([FromQuery] AnalyticsQueryDto q, CancellationToken ct) => Ok((await Overview(q, ct)).OrderStatus);
    [HttpGet("return-rate")] public async Task<ActionResult<IReadOnlyList<ReturnRateRowDto>>> ReturnRate([FromQuery] AnalyticsQueryDto q, CancellationToken ct) => Ok((await Overview(q, ct)).ReturnRate);
    [HttpGet("targets")] public async Task<ActionResult<IReadOnlyList<TargetRowDto>>> Targets([FromQuery] AnalyticsQueryDto q, CancellationToken ct) => Ok((await Overview(q, ct)).MonthlyTargets);
    [HttpGet("payment-methods")] public async Task<ActionResult<IReadOnlyList<NamedValueDto>>> PaymentMethods([FromQuery] AnalyticsQueryDto q, CancellationToken ct) => Ok((await Overview(q, ct)).PaymentMethods);
}
