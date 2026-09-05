namespace Application.Services;

/// <summary>Read-only admin analytics assembled from repositories exposed by the unit of work.</summary>
public sealed class AnalyticsService : IAnalyticsService
{
    private readonly IUnitOfWork _unitOfWork;

    public AnalyticsService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<AnalyticsOverviewDto> GetOverviewAsync(AnalyticsQueryDto query, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        var range = AnalyticsRange.Parse(query.Range);
        var now = DateTime.UtcNow;
        var (start, end) = range.Bounds(now);
        var previousStart = start - (end - start);

        // Materialise the navigation properties used below. The generic repository deliberately uses
        // no tracking, so relying on lazy loading here would otherwise yield incomplete analytics.
        var orders = (await _unitOfWork.Orders.GetAllAsync(q => q
            .Include(o => o.Items).ThenInclude(i => i.Product).ThenInclude(p => p.Category)
            .Include(o => o.Items).ThenInclude(i => i.Product).ThenInclude(p => p.Brand)
            .Include(o => o.Items).ThenInclude(i => i.Product).ThenInclude(p => p.Batches)))
            .ToList();
        var products = (await _unitOfWork.Products.GetAllAsync(q => q
            .Include(p => p.Batches)
            .Include(p => p.Suppliers).ThenInclude(ps => ps.Supplier)
            .Include(p => p.Brand)
            .Include(p => p.Category)))
            .ToList();
        var payments = (await _unitOfWork.Payments.GetAllAsync()).ToList();

        var paidOrderIds = payments
            .Where(payment => payment.Status == PaymentStatus.Completed && !string.IsNullOrWhiteSpace(payment.OrderId))
            .Select(payment => payment.OrderId)
            .ToHashSet(StringComparer.Ordinal);

        var filtered = FilterOrders(orders, start, end, query).ToList();
        var previous = FilterOrders(orders, previousStart, start, query).ToList();
        var reportOrders = filtered.Where(IsReportable).ToList();
        var previousReportOrders = previous.Where(IsReportable).ToList();
        var revenueOrders = reportOrders.Where(order => RecognizesRevenue(order, paidOrderIds)).ToList();
        var previousRevenueOrders = previousReportOrders.Where(order => RecognizesRevenue(order, paidOrderIds)).ToList();

        var revenue = revenueOrders.Sum(order => Total(order, query));
        var previousRevenue = previousRevenueOrders.Sum(order => Total(order, query));
        var customers = reportOrders.Select(order => order.CustomerId).Where(id => !string.IsNullOrWhiteSpace(id)).Distinct().Count();
        var previousCustomers = previousReportOrders.Select(order => order.CustomerId).Where(id => !string.IsNullOrWhiteSpace(id)).Distinct().Count();
        var activeProducts = products.Count(product => product.IsActive && Matches(product, query));
        var returns = filtered.Count(IsReturn);
        var previousReturns = previous.Count(IsReturn);
        var average = revenueOrders.Count == 0 ? 0 : revenue / revenueOrders.Count;
        var previousAverage = previousRevenueOrders.Count == 0 ? 0 : previousRevenue / previousRevenueOrders.Count;
        var target = previousRevenue <= 0 ? revenue : previousRevenue * 1.10m;
        var labels = range.Buckets(start, end).ToList();

        var sales = labels.Select(bucket =>
        {
            var bucketOrders = FilterOrders(revenueOrders, bucket.Start, bucket.End, query).ToList();
            var bucketRevenue = bucketOrders.Sum(order => Total(order, query));
            var cost = bucketOrders.SelectMany(order => MatchingItems(order, query)).Sum(Cost);
            return new SalesPointDto(bucket.Name, bucketRevenue, bucketOrders.Count, bucketRevenue - cost);
        }).ToList();

        var revenueComparison = labels.Select(bucket => new RevenueComparisonPointDto(
            bucket.Name,
            FilterOrders(revenueOrders, bucket.Start, bucket.End, query).Sum(order => Total(order, query)),
            FilterOrders(previousRevenueOrders, bucket.Start - (end - start), bucket.End - (end - start), query).Sum(order => Total(order, query))))
            .ToList();

        var profitMargin = labels.Select(bucket =>
        {
            var bucketOrders = FilterOrders(revenueOrders, bucket.Start, bucket.End, query).ToList();
            var bucketRevenue = bucketOrders.Sum(order => Total(order, query));
            var cost = bucketOrders.SelectMany(order => MatchingItems(order, query)).Sum(Cost);
            return new ProfitMarginPointDto(bucket.Name, bucketRevenue == 0 ? 0 : Math.Round((bucketRevenue - cost) / bucketRevenue * 100, 2), bucketRevenue, cost);
        }).ToList();

        var growth = labels.Select(bucket =>
        {
            var bucketOrders = FilterOrders(reportOrders, bucket.Start, bucket.End, query).ToList();
            return new GrowthTrendPointDto(bucket.Name, bucketOrders.Where(order => RecognizesRevenue(order, paidOrderIds)).Sum(order => Total(order, query)), bucketOrders.Count, bucketOrders.Select(order => order.CustomerId).Where(id => !string.IsNullOrWhiteSpace(id)).Distinct().Count());
        }).ToList();

        var sold = revenueOrders.SelectMany(order => MatchingItems(order, query)).ToList();
        return new AnalyticsOverviewDto
        {
            Kpis = new AnalyticsKpisDto
            {
                Revenue = revenue,
                Orders = reportOrders.Count,
                Customers = customers,
                AvgOrder = average,
                ActiveProducts = activeProducts,
                ConversionRate = 0,
                ReturnRate = Percent(returns, filtered.Count),
                TargetAchievement = Percent(revenue, target),
                RevenueChange = Change(revenue, previousRevenue),
                OrdersChange = Change(reportOrders.Count, previousReportOrders.Count),
                CustomersChange = Change(customers, previousCustomers),
                AvgOrderChange = Change(average, previousAverage),
                ActiveProductsChange = 0,
                ConversionRateChange = 0,
                ReturnRateChange = Change(Percent(returns, filtered.Count), Percent(previousReturns, previous.Count)),
                TargetAchievementChange = 0
            },
            Sales = sales,
            RevenueComparison = revenueComparison,
            ProfitMargin = profitMargin,
            GrowthTrend = growth,
            CategoryDistribution = sold.GroupBy(item => item.Product.Category?.Name ?? "Uncategorized").Select(group => new NamedValueDto(group.Key, group.Sum(item => ItemTotal(item)))).OrderByDescending(item => item.Value).ToList(),
            TopProducts = sold.GroupBy(item => item.Product.Name).Select(group => new TopProductDto(group.Key, group.Sum(ItemTotal), group.Sum(item => item.Quantity))).OrderByDescending(item => item.Sales).Take(10).ToList(),
            BrandPerformance = sold.GroupBy(item => item.Product.Brand?.Name ?? "Unbranded").Select(group => new BrandPerformanceDto(group.Key, group.Sum(ItemTotal), group.Select(item => item.OrderId).Distinct().Count(), products.Count(product => (product.Brand?.Name ?? "Unbranded") == group.Key))).OrderByDescending(item => item.Sales).ToList(),
            SupplierStats = SupplierStats(products, sold),
            InventoryStatus = Inventory(products, query),
            OrdersHeatmap = Heatmap(reportOrders),
            CustomerSegments = Segments(reportOrders),
            OrderStatus = filtered.GroupBy(order => order.Status.ToString()).Select(group => new NamedValueDto(group.Key, group.Count())).OrderByDescending(item => item.Value).ToList(),
            ReturnRate = ReturnRates(filtered),
            MonthlyTargets = [new TargetRowDto("Revenue", revenue, target, "currency")],
            // Payment methods are payment facts, not delivery facts. Do not discard valid payments
            // merely because an order is still Processing or Shipped.
            PaymentMethods = PaymentMethods(payments, reportOrders.Select(order => order.Id)),
            UpdatedAt = now
        };
    }

    private static IEnumerable<Order> FilterOrders(IEnumerable<Order> orders, DateTime start, DateTime end, AnalyticsQueryDto query) =>
        orders.Where(order => order.CreatedTime >= start && order.CreatedTime < end &&
            (query.CategoryId is null || MatchingItems(order, query).Any()) &&
            (query.BrandId is null || MatchingItems(order, query).Any()));

    private static IEnumerable<OrderItem> MatchingItems(Order order, AnalyticsQueryDto query) => order.Items.Where(item => Matches(item.Product, query));
    private static bool Matches(Product product, AnalyticsQueryDto query) => (query.CategoryId is null || product.CategoryId == query.CategoryId) && (query.BrandId is null || product.BrandId == query.BrandId);
    private static bool IsReportable(Order order) => order.Status is not (OrderStatus.Cancelled or OrderStatus.Rejected);
    private static bool IsReturn(Order order) => order.Status is OrderStatus.Returned or OrderStatus.PartiallyReturned;
    private static bool RecognizesRevenue(Order order, ISet<string> paidOrderIds) => !IsReturn(order) && (order.Status is OrderStatus.Completed or OrderStatus.Delivered || paidOrderIds.Contains(order.Id));

    private static decimal Total(Order order, AnalyticsQueryDto query)
    {
        var allItemsTotal = order.Items.Sum(ItemTotal);
        var matchingItemsTotal = MatchingItems(order, query).Sum(ItemTotal);
        if (allItemsTotal == 0 || matchingItemsTotal == 0)
            return 0;

        // An order-level discount applies proportionally when the dashboard is filtered by category/brand.
        return Math.Max(0, matchingItemsTotal - order.DiscountAmount * matchingItemsTotal / allItemsTotal);
    }

    private static decimal ItemTotal(OrderItem item) => item.Quantity * (item.DiscountedPrice > 0 ? item.DiscountedPrice : item.UnitPrice);
    private static decimal Cost(OrderItem item) => item.Quantity * (item.Product.Batches.OrderByDescending(batch => batch.EffectiveDate).FirstOrDefault()?.CostPrice ?? 0);
    private static decimal Percent(decimal numerator, decimal denominator) => denominator == 0 ? 0 : Math.Round(numerator / denominator * 100, 2);
    private static decimal Change(decimal current, decimal previous) => previous == 0 ? (current == 0 ? 0 : 100) : Math.Round((current - previous) / previous * 100, 2);

    private static List<SupplierStatDto> SupplierStats(IEnumerable<Product> products, IEnumerable<OrderItem> sold) => products
        .SelectMany(product => product.Suppliers.Where(link => link.Supplier is not null).Select(link => new { Product = product, link.Supplier }))
        .GroupBy(item => item.Supplier!)
        .Select(group =>
        {
            var productIds = group.Select(item => item.Product.Id).ToHashSet();
            var rows = sold.Where(item => productIds.Contains(item.ProductId)).ToList();
            var sales = rows.Sum(ItemTotal);
            return new SupplierStatDto(group.Key.Id.ToString(), group.Key.Name, productIds.Count, sales, rows.Select(item => item.OrderId).Distinct().Count(), sales, "neutral");
        })
        .OrderByDescending(item => item.TotalSales)
        .ToList();

    private static List<NamedValueDto> Inventory(IEnumerable<Product> products, AnalyticsQueryDto query)
    {
        var filtered = products.Where(product => Matches(product, query)).ToList();
        var stock = filtered.Select(product => product.Batches.Sum(batch => batch.Quantity - batch.SoldQuantity)).ToList();
        return [new("InStock", stock.Count(quantity => quantity > 10)), new("LowStock", stock.Count(quantity => quantity is > 0 and <= 10)), new("OutOfStock", stock.Count(quantity => quantity <= 0))];
    }

    private static List<HeatmapRowDto> Heatmap(IEnumerable<Order> orders) => Enumerable.Range(0, 7).Select(day => new HeatmapRowDto(((DayOfWeek)day).ToString(), Enumerable.Range(0, 8).Select(hour => orders.Count(order => (int)order.CreatedTime.DayOfWeek == day && order.CreatedTime.Hour >= hour * 3 && order.CreatedTime.Hour < hour * 3 + 3)).ToList())).ToList();
    private static List<CustomerSegmentStatDto> Segments(IEnumerable<Order> orders) => [new("New", orders.Where(order => !string.IsNullOrWhiteSpace(order.CustomerId)).GroupBy(order => order.CustomerId).Count(group => group.Count() == 1)), new("Returning", orders.Where(order => !string.IsNullOrWhiteSpace(order.CustomerId)).GroupBy(order => order.CustomerId).Count(group => group.Count() is > 1 and <= 5)), new("Loyal", orders.Where(order => !string.IsNullOrWhiteSpace(order.CustomerId)).GroupBy(order => order.CustomerId).Count(group => group.Count() > 5))];
    private static List<ReturnRateRowDto> ReturnRates(IEnumerable<Order> orders) => orders.SelectMany(order => order.Items.Select(item => new { Item = item, Returned = IsReturn(order) })).GroupBy(item => item.Item.Product.Category?.Name ?? "Uncategorized").Select(group => new ReturnRateRowDto(group.Key, Percent(group.Count(item => item.Returned), group.Count()), group.Count(item => item.Returned))).OrderByDescending(item => item.Rate).ToList();
    private static List<NamedValueDto> PaymentMethods(IEnumerable<Payment> payments, IEnumerable<string> orderIds)
    {
        var ids = orderIds.ToHashSet(StringComparer.Ordinal);
        return payments.Where(payment => ids.Contains(payment.OrderId) && payment.Status == PaymentStatus.Completed).GroupBy(payment => payment.Method.ToString()).Select(group => new NamedValueDto(group.Key, group.Sum(payment => payment.Amount))).OrderByDescending(item => item.Value).ToList();
    }
}

internal sealed class AnalyticsRange
{
    private readonly string _value;
    private AnalyticsRange(string value) => _value = value;

    public static AnalyticsRange Parse(string? value) => value?.ToLowerInvariant() switch { "daily" or "weekly" or "monthly" or "yearly" => new AnalyticsRange(value.ToLowerInvariant()), _ => new AnalyticsRange("monthly") };
    public (DateTime Start, DateTime End) Bounds(DateTime now) => _value switch { "daily" => (now.Date, now.Date.AddDays(1)), "weekly" => (now.Date.AddDays(-6), now.Date.AddDays(1)), "yearly" => (new DateTime(now.Year, 1, 1), new DateTime(now.Year + 1, 1, 1)), _ => (new DateTime(now.Year, now.Month, 1), new DateTime(now.Year, now.Month, 1).AddMonths(1)) };
    public IEnumerable<(string Name, DateTime Start, DateTime End)> Buckets(DateTime start, DateTime end) { var count = _value == "daily" ? 8 : _value == "weekly" ? 7 : _value == "monthly" ? 4 : 12; var span = (end - start).Ticks / count; for (var i = 0; i < count; i++) { var bucketStart = start.AddTicks(span * i); yield return (bucketStart.ToString(_value == "yearly" ? "MMM" : "MM/dd"), bucketStart, i == count - 1 ? end : start.AddTicks(span * (i + 1))); } }
}
