namespace Application.Services;

/// <summary>Read-only admin analytics assembled from repositories exposed by the unit of work.</summary>
public sealed class AnalyticsService : IAnalyticsService
{
    private readonly IUnitOfWork _unitOfWork;
    public AnalyticsService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<AnalyticsOverviewDto> GetOverviewAsync(AnalyticsQueryDto query, CancellationToken cancellationToken = default)
    {
        var range = AnalyticsRange.Parse(query.Range);
        var now = DateTime.UtcNow;
        var (start, end) = range.Bounds(now);
        var previousStart = start - (end - start);
        var orders = (await _unitOfWork.Orders.GetAllAsync(q => q.Include(o => o.Items).ThenInclude(i => i.Product).ThenInclude(p => p.Category).Include(o => o.Items).ThenInclude(i => i.Product).ThenInclude(p => p.Brand).Include(o => o.Items).ThenInclude(i => i.Product).ThenInclude(p => p.Batches))).ToList();
        var products = (await _unitOfWork.Products.GetAllAsync(q => q.Include(p => p.Batches).Include(p => p.Suppliers).ThenInclude(ps => ps.Supplier).Include(p => p.Brand).Include(p => p.Category))).ToList();
        var payments = (await _unitOfWork.Payments.GetAllAsync()).ToList();
        var filtered = FilterOrders(orders, start, end, query).ToList();
        var previous = FilterOrders(orders, previousStart, start, query).ToList();
        var completed = filtered.Where(IsCompleted).ToList();
        var previousCompleted = previous.Where(IsCompleted).ToList();
        var revenue = completed.Sum(o => Total(o, query)); var previousRevenue = previousCompleted.Sum(o => Total(o, query));
        var customers = completed.Select(o => o.CustomerId).Where(id => !string.IsNullOrWhiteSpace(id)).Distinct().Count();
        var previousCustomers = previousCompleted.Select(o => o.CustomerId).Where(id => !string.IsNullOrWhiteSpace(id)).Distinct().Count();
        var activeProducts = products.Count(p => p.IsActive && Matches(p, query));
        var previousActiveProducts = activeProducts;
        var returns = filtered.Count(o => o.Status is OrderStatus.Returned or OrderStatus.PartiallyReturned);
        var previousReturns = previous.Count(o => o.Status is OrderStatus.Returned or OrderStatus.PartiallyReturned);
        var average = completed.Count == 0 ? 0 : revenue / completed.Count;
        var previousAverage = previousCompleted.Count == 0 ? 0 : previousRevenue / previousCompleted.Count;
        var target = previousRevenue <= 0 ? revenue : previousRevenue * 1.10m;
        var labels = range.Buckets(start, end).ToList();
        var sales = labels.Select(b => { var os = FilterOrders(completed, b.Start, b.End, query).ToList(); var r = os.Sum(o => Total(o, query)); var cost = os.SelectMany(o => o.Items.Where(i => Matches(i.Product, query))).Sum(Cost); return new SalesPointDto(b.Name, r, os.Count, r - cost); }).ToList();
        var revenueComparison = labels.Select(b => new RevenueComparisonPointDto(b.Name, FilterOrders(completed, b.Start, b.End, query).Sum(o => Total(o, query)), FilterOrders(previousCompleted, b.Start - (end - start), b.End - (end - start), query).Sum(o => Total(o, query)))).ToList();
        var profitMargin = labels.Select(b => { var os = FilterOrders(completed, b.Start, b.End, query).ToList(); var r = os.Sum(o => Total(o, query)); var c = os.SelectMany(o => o.Items.Where(i => Matches(i.Product, query))).Sum(Cost); return new ProfitMarginPointDto(b.Name, r == 0 ? 0 : Math.Round((r - c) / r * 100, 2), r, c); }).ToList();
        var growth = labels.Select(b => { var os = FilterOrders(completed, b.Start, b.End, query).ToList(); return new GrowthTrendPointDto(b.Name, os.Sum(o => Total(o, query)), os.Count, os.Select(x => x.CustomerId).Distinct().Count()); }).ToList();
        var sold = completed.SelectMany(o => o.Items.Where(i => Matches(i.Product, query))).ToList();
        return new AnalyticsOverviewDto
        {
            Kpis = new AnalyticsKpisDto { Revenue = revenue, Orders = completed.Count, Customers = customers, AvgOrder = average, ActiveProducts = activeProducts, ConversionRate = 0, ReturnRate = Percent(returns, filtered.Count), TargetAchievement = Percent(revenue, target), RevenueChange = Change(revenue, previousRevenue), OrdersChange = Change(completed.Count, previousCompleted.Count), CustomersChange = Change(customers, previousCustomers), AvgOrderChange = Change(average, previousAverage), ActiveProductsChange = Change(activeProducts, previousActiveProducts), ConversionRateChange = 0, ReturnRateChange = Change(Percent(returns, filtered.Count), Percent(previousReturns, previous.Count)), TargetAchievementChange = 0 },
            Sales = sales, RevenueComparison = revenueComparison, ProfitMargin = profitMargin, GrowthTrend = growth,
            CategoryDistribution = sold.GroupBy(i => i.Product.Category.Name).Select(g => new NamedValueDto(g.Key, g.Sum(i => i.Quantity * i.UnitPrice))).OrderByDescending(x => x.Value).ToList(),
            TopProducts = sold.GroupBy(i => i.Product.Name).Select(g => new TopProductDto(g.Key, g.Sum(i => i.Quantity * i.UnitPrice), g.Sum(i => i.Quantity))).OrderByDescending(x => x.Sales).Take(10).ToList(),
            BrandPerformance = sold.GroupBy(i => i.Product.Brand.Name).Select(g => new BrandPerformanceDto(g.Key, g.Sum(i => i.Quantity * i.UnitPrice), g.Select(i => i.OrderId).Distinct().Count(), products.Count(p => p.Brand.Name == g.Key))).OrderByDescending(x => x.Sales).ToList(),
            SupplierStats = SupplierStats(products, sold), InventoryStatus = Inventory(products, query), OrdersHeatmap = Heatmap(completed), CustomerSegments = Segments(completed),
            OrderStatus = filtered.GroupBy(o => o.Status.ToString()).Select(g => new NamedValueDto(g.Key, g.Count())).ToList(), ReturnRate = ReturnRates(filtered),
            MonthlyTargets = [new TargetRowDto("Revenue", revenue, target, "currency")], PaymentMethods = PaymentMethods(payments, completed.Select(o => o.Id)), UpdatedAt = now
        };
    }
    private static IEnumerable<Order> FilterOrders(IEnumerable<Order> orders, DateTime s, DateTime e, AnalyticsQueryDto q) => orders.Where(o => o.CreatedTime >= s && o.CreatedTime < e && (q.CategoryId is null || o.Items.Any(i => i.Product.CategoryId == q.CategoryId)) && (q.BrandId is null || o.Items.Any(i => i.Product.BrandId == q.BrandId)));
    private static bool Matches(Product p, AnalyticsQueryDto q) => (q.CategoryId is null || p.CategoryId == q.CategoryId) && (q.BrandId is null || p.BrandId == q.BrandId);
    private static bool IsCompleted(Order o) => o.Status is OrderStatus.Completed or OrderStatus.Delivered;
    private static decimal Total(Order o, AnalyticsQueryDto? query = null)
    {
        var items = query is null ? o.Items : o.Items.Where(i => Matches(i.Product, query));
        return items.Sum(i => i.Quantity * (i.DiscountedPrice > 0 ? i.DiscountedPrice : i.UnitPrice));
    }
    private static decimal Cost(OrderItem i) => i.Quantity * (i.Product.Batches.OrderByDescending(b => b.EffectiveDate).FirstOrDefault()?.CostPrice ?? 0);
    private static decimal Percent(decimal n, decimal d) => d == 0 ? 0 : Math.Round(n / d * 100, 2);
    private static decimal Change(decimal n, decimal p) => p == 0 ? (n == 0 ? 0 : 100) : Math.Round((n - p) / p * 100, 2);
    private static List<SupplierStatDto> SupplierStats(IEnumerable<Product> products, IEnumerable<OrderItem> sold) => products.SelectMany(p => p.Suppliers.Select(s => new { p, s.Supplier })).Where(x => x.Supplier is not null).GroupBy(x => x.Supplier!).Select(g => { var ids = g.Select(x => x.p.Id).ToHashSet(); var rows = sold.Where(i => ids.Contains(i.ProductId)).ToList(); var sale = rows.Sum(i => i.Quantity * i.UnitPrice); return new SupplierStatDto(g.Key.Id.ToString(), g.Key.Name, ids.Count, sale, rows.Select(i => i.OrderId).Distinct().Count(), sale, "neutral"); }).OrderByDescending(x => x.TotalSales).ToList();
    private static List<NamedValueDto> Inventory(IEnumerable<Product> p, AnalyticsQueryDto q) { var x = p.Where(p => Matches(p, q)).ToList(); return [new("InStock", x.Count(p => p.Batches.Sum(b => b.Quantity - b.SoldQuantity) > 10)), new("LowStock", x.Count(p => { var n = p.Batches.Sum(b => b.Quantity - b.SoldQuantity); return n > 0 && n <= 10; })), new("OutOfStock", x.Count(p => p.Batches.Sum(b => b.Quantity - b.SoldQuantity) <= 0))]; }
    private static List<HeatmapRowDto> Heatmap(IEnumerable<Order> os) => Enumerable.Range(0, 7).Select(d => new HeatmapRowDto(((DayOfWeek)d).ToString(), Enumerable.Range(0, 8).Select(h => os.Count(o => (int)o.CreatedTime.DayOfWeek == d && o.CreatedTime.Hour >= h * 3 && o.CreatedTime.Hour < h * 3 + 3)).ToList())).ToList();
    private static List<CustomerSegmentStatDto> Segments(IEnumerable<Order> os) => [new("New", os.GroupBy(x => x.CustomerId).Count(g => g.Count() == 1)), new("Returning", os.GroupBy(x => x.CustomerId).Count(g => g.Count() is > 1 and <= 5)), new("Loyal", os.GroupBy(x => x.CustomerId).Count(g => g.Count() > 5))];
    private static List<ReturnRateRowDto> ReturnRates(IEnumerable<Order> os) => os.SelectMany(o => o.Items.Select(i => new { i, Returned = o.Status is OrderStatus.Returned or OrderStatus.PartiallyReturned })).GroupBy(x => x.i.Product.Category.Name).Select(g => new ReturnRateRowDto(g.Key, Percent(g.Count(x => x.Returned), g.Count()), g.Count(x => x.Returned))).ToList();
    private static List<NamedValueDto> PaymentMethods(IEnumerable<Payment> p, IEnumerable<string> ids) { var set = ids.ToHashSet(); return p.Where(x => set.Contains(x.OrderId) && x.Status == PaymentStatus.Completed).GroupBy(x => x.Method.ToString()).Select(g => new NamedValueDto(g.Key, g.Sum(x => x.Amount))).ToList(); }
}

internal sealed class AnalyticsRange
{
    private readonly string _value; private AnalyticsRange(string value) => _value = value;
    public static AnalyticsRange Parse(string? value) => value?.ToLowerInvariant() switch { "daily" or "weekly" or "monthly" or "yearly" => new AnalyticsRange(value.ToLowerInvariant()), _ => new AnalyticsRange("monthly") };
    public (DateTime Start, DateTime End) Bounds(DateTime now) => _value switch { "daily" => (now.Date, now.Date.AddDays(1)), "weekly" => (now.Date.AddDays(-6), now.Date.AddDays(1)), "yearly" => (new DateTime(now.Year, 1, 1), new DateTime(now.Year + 1, 1, 1)), _ => (new DateTime(now.Year, now.Month, 1), new DateTime(now.Year, now.Month, 1).AddMonths(1)) };
    public IEnumerable<(string Name, DateTime Start, DateTime End)> Buckets(DateTime start, DateTime end) { var count = _value == "daily" ? 8 : _value == "weekly" ? 7 : _value == "monthly" ? 4 : 12; var span = (end - start).Ticks / count; for (var i = 0; i < count; i++) { var s = start.AddTicks(span * i); yield return (s.ToString(_value == "yearly" ? "MMM" : "MM/dd"), s, i == count - 1 ? end : start.AddTicks(span * (i + 1))); } }
}
