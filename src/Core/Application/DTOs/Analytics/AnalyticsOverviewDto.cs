namespace Application.DTOs;

public sealed class AnalyticsQueryDto
{
    public string Range { get; set; } = "monthly";
    public int? CategoryId { get; set; }
    public int? BrandId { get; set; }
}

public sealed class AnalyticsOverviewDto
{
    public AnalyticsKpisDto Kpis { get; init; } = new();
    public IReadOnlyList<SalesPointDto> Sales { get; init; } = [];
    public IReadOnlyList<RevenueComparisonPointDto> RevenueComparison { get; init; } = [];
    public IReadOnlyList<ProfitMarginPointDto> ProfitMargin { get; init; } = [];
    public IReadOnlyList<GrowthTrendPointDto> GrowthTrend { get; init; } = [];
    public IReadOnlyList<NamedValueDto> CategoryDistribution { get; init; } = [];
    public IReadOnlyList<TopProductDto> TopProducts { get; init; } = [];
    public IReadOnlyList<BrandPerformanceDto> BrandPerformance { get; init; } = [];
    public IReadOnlyList<SupplierStatDto> SupplierStats { get; init; } = [];
    public IReadOnlyList<NamedValueDto> InventoryStatus { get; init; } = [];
    public IReadOnlyList<HeatmapRowDto> OrdersHeatmap { get; init; } = [];
    public IReadOnlyList<CustomerSegmentStatDto> CustomerSegments { get; init; } = [];
    public IReadOnlyList<NamedValueDto> OrderStatus { get; init; } = [];
    public IReadOnlyList<ReturnRateRowDto> ReturnRate { get; init; } = [];
    public IReadOnlyList<TargetRowDto> MonthlyTargets { get; init; } = [];
    public IReadOnlyList<NamedValueDto> PaymentMethods { get; init; } = [];
    public DateTime UpdatedAt { get; init; }
}

public sealed class AnalyticsKpisDto { public decimal Revenue { get; init; } public int Orders { get; init; } public int Customers { get; init; } public decimal AvgOrder { get; init; } public int ActiveProducts { get; init; } public decimal ConversionRate { get; init; } public decimal ReturnRate { get; init; } public decimal TargetAchievement { get; init; } public decimal RevenueChange { get; init; } public decimal OrdersChange { get; init; } public decimal CustomersChange { get; init; } public decimal AvgOrderChange { get; init; } public decimal ActiveProductsChange { get; init; } public decimal ConversionRateChange { get; init; } public decimal ReturnRateChange { get; init; } public decimal TargetAchievementChange { get; init; } }
public sealed record SalesPointDto(string Name, decimal Sales, int Orders, decimal Profit);
public sealed record RevenueComparisonPointDto(string Name, decimal Current, decimal Previous);
public sealed record ProfitMarginPointDto(string Name, decimal Margin, decimal Revenue, decimal Cost);
public sealed record GrowthTrendPointDto(string Name, decimal Revenue, int Orders, int Customers);
public sealed record NamedValueDto(string Name, decimal Value);
public sealed record TopProductDto(string Name, decimal Sales, decimal Quantity);
public sealed record BrandPerformanceDto(string Name, decimal Sales, int Orders, int Products);
public sealed record SupplierStatDto(string Id, string Name, int TotalProducts, decimal TotalSales, int OrdersFulfilled, decimal Performance, string Trend);
public sealed record HeatmapRowDto(string Day, IReadOnlyList<int> Hours);
public sealed record CustomerSegmentStatDto(string Name, int Size);
public sealed record ReturnRateRowDto(string Category, decimal Rate, int Returns);
public sealed record TargetRowDto(string Name, decimal Current, decimal Target, string Unit);
