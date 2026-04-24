using Domain.Enums;
using Domain.Models;

namespace Infrastructure.Repositories;

public class AnalyticsRepository : IAnalyticsRepository
{
    private readonly AppDbContext _context;

    public AnalyticsRepository(AppDbContext context)
    {
        _context = context;
    }



    // ═══════════════════════════════════════════════════════════════
    //                    MOST SELLING PRODUCTS
    // ═══════════════════════════════════════════════════════════════

    public async Task<List<ProductSalesModel>> GetTopSellingProductsAsync(int top = 10)
    {
        return await _context.Products
            .Where(p => p.IsActive)
            .Select(p => new ProductSalesModel
            {
                ProductId = p.Id,
                ProductName = p.Name,
                CategoryName = p.Category.Name,
                BrandName = p.Brand.Name,
                TotalQuantitySold = p.OrderItems.Sum(oi => oi.Quantity),
                TotalRevenue = p.OrderItems.Sum(oi => oi.Quantity * oi.UnitPrice),
                OrderCount = p.OrderItems.Select(oi => oi.OrderId).Distinct().Count()
            })
            .OrderByDescending(x => x.TotalQuantitySold)
            .Take(top)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<ProductSalesModel>> GetTopSellingProductsInCategoryAsync(
        string categoryName,
        int top = 10)
    {
        return await _context.Products
            .Where(p => p.Category.Name == categoryName && p.IsActive)
            .Select(p => new ProductSalesModel
            {
                ProductId = p.Id,
                ProductName = p.Name,
                CategoryName = p.Category.Name,
                BrandName = p.Brand.Name,
                TotalQuantitySold = p.OrderItems.Sum(oi => oi.Quantity),
                TotalRevenue = p.OrderItems.Sum(oi => oi.Quantity * oi.UnitPrice),
                OrderCount = p.OrderItems.Select(oi => oi.OrderId).Distinct().Count()
            })
            .OrderByDescending(x => x.TotalQuantitySold)
            .Take(top)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<ProductSalesModel>> GetTopSellingProductsByBrandAsync(
        string brandName,
        int top = 10)
    {
        return await _context.Products
            .Where(p => p.Brand.Name == brandName && p.IsActive)
            .Select(p => new ProductSalesModel
            {
                ProductId = p.Id,
                ProductName = p.Name,
                CategoryName = p.Category.Name,
                BrandName = p.Brand.Name,
                TotalQuantitySold = p.OrderItems.Sum(oi => oi.Quantity),
                TotalRevenue = p.OrderItems.Sum(oi => oi.Quantity * oi.UnitPrice),
                OrderCount = p.OrderItems.Select(oi => oi.OrderId).Distinct().Count()
            })
            .OrderByDescending(x => x.TotalQuantitySold)
            .Take(top)
            .AsNoTracking()
            .ToListAsync();
    }

    // ═══════════════════════════════════════════════════════════════
    //                    LEAST SELLING PRODUCTS
    // ═══════════════════════════════════════════════════════════════

    public async Task<List<ProductSalesModel>> GetLeastSellingProductsAsync(int top = 10)
    {
        return await _context.Products
            .Where(p => p.IsActive)
            .Select(p => new ProductSalesModel
            {
                ProductId = p.Id,
                ProductName = p.Name,
                CategoryName = p.Category.Name,
                BrandName = p.Brand.Name,
                TotalQuantitySold = p.OrderItems.Sum(oi => (int?)oi.Quantity) ?? 0,
                TotalRevenue = p.OrderItems.Sum(oi => (decimal?)(oi.Quantity * oi.UnitPrice)) ?? 0,
                OrderCount = p.OrderItems.Select(oi => oi.OrderId).Distinct().Count()
            })
            .OrderBy(x => x.TotalQuantitySold)
            .Take(top)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<ProductSalesModel>> GetLeastSellingProductsInCategoryAsync(
        string categoryName,
        int top = 10)
    {
        return await _context.Products
            .Where(p => p.Category.Name == categoryName && p.IsActive)
            .Select(p => new ProductSalesModel
            {
                ProductId = p.Id,
                ProductName = p.Name,
                CategoryName = p.Category.Name,
                BrandName = p.Brand.Name,
                TotalQuantitySold = p.OrderItems.Sum(oi => (int?)oi.Quantity) ?? 0,
                TotalRevenue = p.OrderItems.Sum(oi => (decimal?)(oi.Quantity * oi.UnitPrice)) ?? 0,
                OrderCount = p.OrderItems.Select(oi => oi.OrderId).Distinct().Count()
            })
            .OrderBy(x => x.TotalQuantitySold)
            .Take(top)
            .AsNoTracking()
            .ToListAsync();
    }

    // ═══════════════════════════════════════════════════════════════
    //                    MOST EXPENSIVE PRODUCTS
    // ═══════════════════════════════════════════════════════════════

    public async Task<List<ProductModel>> GetMostExpensiveProductsAsync(int top = 10)
    {
        return await _context.Products
            .Where(p => p.IsActive)
            .OrderByDescending(p => p.Price)
            .Take(top)
            .Select(p => new ProductModel
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                CategoryName = p.Category.Name,
                BrandName = p.Brand.Name,
                //ImageUrl = p.Images.FirstOrDefault(i => i.IsMain).Url,
                CreatedAt = p.CreatedTime
            })
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<ProductModel>> GetMostExpensiveProductsInCategoryAsync(
        string categoryName,
        int top = 10)
    {
        return await _context.Products
            .Where(p => p.Category.Name == categoryName && p.IsActive)
            .OrderByDescending(p => p.Price)
            .Take(top)
            .Select(p => new ProductModel
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                CategoryName = p.Category.Name,
                BrandName = p.Brand.Name,
                //ImageUrl = p.Images.FirstOrDefault(i => i.IsMain).Url,
                CreatedAt = p.CreatedTime
            })
            .AsNoTracking()
            .ToListAsync();
    }

    // ═══════════════════════════════════════════════════════════════
    //                    CHEAPEST PRODUCTS
    // ═══════════════════════════════════════════════════════════════

    public async Task<List<ProductModel>> GetCheapestProductsAsync(int top = 10)
    {
        return await _context.Products
            .Where(p => p.IsActive)
            .OrderBy(p => p.Price)
            .Take(top)
            .Select(p => new ProductModel
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                CategoryName = p.Category.Name,
                BrandName = p.Brand.Name,
                //ImageUrl = p.Images.FirstOrDefault(i => i.IsMain).Url,
                CreatedAt = p.CreatedTime
            })
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<ProductModel>> GetCheapestProductsInCategoryAsync(
        string categoryName,
        int top = 10)
    {
        return await _context.Products
            .Where(p => p.Category.Name == categoryName && p.IsActive)
            .OrderBy(p => p.Price)
            .Take(top)
            .Select(p => new ProductModel
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                CategoryName = p.Category.Name,
                BrandName = p.Brand.Name,
                //ImageUrl = p.Images.FirstOrDefault(i => i.IsMain).Url,
                CreatedAt = p.CreatedTime
            })
            .AsNoTracking()
            .ToListAsync();
    }

    // ═══════════════════════════════════════════════════════════════
    //                    MOST SEARCHED PRODUCTS
    // ═══════════════════════════════════════════════════════════════

    //public async Task<List<ProductSearchModel>> GetMostSearchedProductsAsync(int top = 10)
    //{
    //    return await _context.ProductSearches
    //        .Where(s => s.Product.IsActive)
    //        .GroupBy(s => new
    //        {
    //            s.ProductId,
    //            s.Product.Name,
    //            s.Product.Category.Name,
    //            s.Product.Brand.Name
    //        })
    //        .Select(g => new ProductSearchModel
    //        {
    //            ProductId = g.Key.ProductId,
    //            ProductName = g.Key.Name,
    //            CategoryName = g.Key.Name,
    //            BrandName = g.Key.Brand.Name,
    //            SearchCount = g.Count(),
    //            UniqueUsers = g.Select(s => s.UserId).Distinct().Count()
    //        })
    //        .OrderByDescending(x => x.SearchCount)
    //        .Take(top)
    //        .AsNoTracking()
    //        .ToListAsync();
    //}

    //public async Task<List<ProductSearchModel>> GetMostSearchedProductsInCategoryAsync(
    //    string categoryName,
    //    int top = 10)
    //{
    //    return await _context.ProductSearches
    //        .Where(s => s.Product.Category.Name == categoryName && s.Product.IsActive)
    //        .GroupBy(s => new
    //        {
    //            s.ProductId,
    //            s.Product.Name,
    //            s.Product.Category.Name,
    //            s.Product.Brand.Name
    //        })
    //        .Select(g => new ProductSearchModel
    //        {
    //            ProductId = g.Key.ProductId,
    //            ProductName = g.Key.Name,
    //            CategoryName = g.Key.Name,
    //            BrandName = g.Key.Brand.Name,
    //            SearchCount = g.Count(),
    //            UniqueUsers = g.Select(s => s.UserId).Distinct().Count()
    //        })
    //        .OrderByDescending(x => x.SearchCount)
    //        .Take(top)
    //        .AsNoTracking()
    //        .ToListAsync();
    //}

    // ═══════════════════════════════════════════════════════════════
    //                    SALES BY TIMEFRAME - DAILY
    // ═══════════════════════════════════════════════════════════════

    public async Task<List<ProductSalesModel>> GetTopSellingProductsDailyAsync(
        DateTime date,
        int top = 10)
    {
        var startOfDay = date.Date;
        var endOfDay = date.Date.AddDays(1);

        return await _context.Products
            .Where(p => p.IsActive)
            .Select(p => new ProductSalesModel
            {
                ProductId = p.Id,
                ProductName = p.Name,
                CategoryName = p.Category.Name,
                BrandName = p.Brand.Name,
                TotalQuantitySold = p.OrderItems
                    .Where(oi => oi.Order.CreatedTime >= startOfDay && oi.Order.CreatedTime < endOfDay)
                    .Sum(oi => oi.Quantity),
                TotalRevenue = p.OrderItems
                    .Where(oi => oi.Order.CreatedTime >= startOfDay && oi.Order.CreatedTime < endOfDay)
                    .Sum(oi => oi.Quantity * oi.UnitPrice),
                OrderCount = p.OrderItems
                    .Where(oi => oi.Order.CreatedTime >= startOfDay && oi.Order.CreatedTime < endOfDay)
                    .Select(oi => oi.OrderId).Distinct().Count()
            })
            .Where(x => x.TotalQuantitySold > 0)
            .OrderByDescending(x => x.TotalQuantitySold)
            .Take(top)
            .AsNoTracking()
            .ToListAsync();
    }

    // ═══════════════════════════════════════════════════════════════
    //                    SALES BY TIMEFRAME - WEEKLY
    // ═══════════════════════════════════════════════════════════════

    public async Task<List<ProductSalesModel>> GetTopSellingProductsWeeklyAsync(
        DateTime weekStart,
        int top = 10)
    {
        var startOfWeek = weekStart.Date;
        var endOfWeek = startOfWeek.AddDays(7);

        return await _context.Products
            .Where(p => p.IsActive)
            .Select(p => new ProductSalesModel
            {
                ProductId = p.Id,
                ProductName = p.Name,
                CategoryName = p.Category.Name,
                BrandName = p.Brand.Name,
                TotalQuantitySold = p.OrderItems
                    .Where(oi => oi.Order.CreatedTime >= startOfWeek && oi.Order.CreatedTime < endOfWeek)
                    .Sum(oi => oi.Quantity),
                TotalRevenue = p.OrderItems
                    .Where(oi => oi.Order.CreatedTime >= startOfWeek && oi.Order.CreatedTime < endOfWeek)
                    .Sum(oi => oi.Quantity * oi.UnitPrice),
                OrderCount = p.OrderItems
                    .Where(oi => oi.Order.CreatedTime >= startOfWeek && oi.Order.CreatedTime < endOfWeek)
                    .Select(oi => oi.OrderId).Distinct().Count()
            })
            .Where(x => x.TotalQuantitySold > 0)
            .OrderByDescending(x => x.TotalQuantitySold)
            .Take(top)
            .AsNoTracking()
            .ToListAsync();
    }

    // ═══════════════════════════════════════════════════════════════
    //                    SALES BY TIMEFRAME - MONTHLY
    // ═══════════════════════════════════════════════════════════════

    public async Task<List<ProductSalesModel>> GetTopSellingProductsMonthlyAsync(
        int year,
        int month,
        int top = 10)
    {
        var startOfMonth = new DateTime(year, month, 1);
        var endOfMonth = startOfMonth.AddMonths(1);

        return await _context.Products
            .Where(p => p.IsActive)
            .Select(p => new ProductSalesModel
            {
                ProductId = p.Id,
                ProductName = p.Name,
                CategoryName = p.Category.Name,
                BrandName = p.Brand.Name,
                TotalQuantitySold = p.OrderItems
                    .Where(oi => oi.Order.CreatedTime >= startOfMonth && oi.Order.CreatedTime < endOfMonth)
                    .Sum(oi => oi.Quantity),
                TotalRevenue = p.OrderItems
                    .Where(oi => oi.Order.CreatedTime >= startOfMonth && oi.Order.CreatedTime < endOfMonth)
                    .Sum(oi => oi.Quantity * oi.UnitPrice),
                OrderCount = p.OrderItems
                    .Where(oi => oi.Order.CreatedTime >= startOfMonth && oi.Order.CreatedTime < endOfMonth)
                    .Select(oi => oi.OrderId).Distinct().Count()
            })
            .Where(x => x.TotalQuantitySold > 0)
            .OrderByDescending(x => x.TotalQuantitySold)
            .Take(top)
            .AsNoTracking()
            .ToListAsync();
    }

    // ═══════════════════════════════════════════════════════════════
    //                    SALES BY TIMEFRAME - YEARLY
    // ═══════════════════════════════════════════════════════════════

    public async Task<List<ProductSalesModel>> GetTopSellingProductsYearlyAsync(
        int year,
        int top = 10)
    {
        var startOfYear = new DateTime(year, 1, 1);
        var endOfYear = startOfYear.AddYears(1);

        return await _context.Products
            .Where(p => p.IsActive)
            .Select(p => new ProductSalesModel
            {
                ProductId = p.Id,
                ProductName = p.Name,
                CategoryName = p.Category.Name,
                BrandName = p.Brand.Name,
                TotalQuantitySold = p.OrderItems
                    .Where(oi => oi.Order.CreatedTime >= startOfYear && oi.Order.CreatedTime < endOfYear)
                    .Sum(oi => oi.Quantity),
                TotalRevenue = p.OrderItems
                    .Where(oi => oi.Order.CreatedTime >= startOfYear && oi.Order.CreatedTime < endOfYear)
                    .Sum(oi => oi.Quantity * oi.UnitPrice),
                OrderCount = p.OrderItems
                    .Where(oi => oi.Order.CreatedTime >= startOfYear && oi.Order.CreatedTime < endOfYear)
                    .Select(oi => oi.OrderId).Distinct().Count()
            })
            .Where(x => x.TotalQuantitySold > 0)
            .OrderByDescending(x => x.TotalQuantitySold)
            .Take(top)
            .AsNoTracking()
            .ToListAsync();
    }

    // ═══════════════════════════════════════════════════════════════
    //                    SALES BY TIMEFRAME - QUARTERLY
    // ═══════════════════════════════════════════════════════════════

    public async Task<List<ProductSalesModel>> GetTopSellingProductsQuarterlyAsync(
        int year,
        int quarter,
        int top = 10)
    {
        var startQuarter = new DateTime(year, (quarter - 1) * 3 + 1, 1);
        var endQuarter = startQuarter.AddMonths(3);

        return await _context.Products
            .Where(p => p.IsActive)
            .Select(p => new ProductSalesModel
            {
                ProductId = p.Id,
                ProductName = p.Name,
                CategoryName = p.Category.Name,
                BrandName = p.Brand.Name,
                TotalQuantitySold = p.OrderItems
                    .Where(oi => oi.Order.CreatedTime >= startQuarter && oi.Order.CreatedTime < endQuarter)
                    .Sum(oi => oi.Quantity),
                TotalRevenue = p.OrderItems
                    .Where(oi => oi.Order.CreatedTime >= startQuarter && oi.Order.CreatedTime < endQuarter)
                    .Sum(oi => oi.Quantity * oi.UnitPrice),
                OrderCount = p.OrderItems
                    .Where(oi => oi.Order.CreatedTime >= startQuarter && oi.Order.CreatedTime < endQuarter)
                    .Select(oi => oi.OrderId).Distinct().Count()
            })
            .Where(x => x.TotalQuantitySold > 0)
            .OrderByDescending(x => x.TotalQuantitySold)
            .Take(top)
            .AsNoTracking()
            .ToListAsync();
    }

    // ═══════════════════════════════════════════════════════════════
    //                    REVENUE BY TIMEFRAME - DAILY
    // ═══════════════════════════════════════════════════════════════

    public async Task<RevenueSummaryModel> GetDailyRevenueAsync(DateTime date)
    {
        var startOfDay = date.Date;
        var endOfDay = date.Date.AddDays(1);

        var result = await _context.Orders
            .Where(o => o.CreatedTime >= startOfDay && o.CreatedTime < endOfDay
                && o.Status == OrderStatus.Completed)
            .GroupBy(o => 1)
            .Select(g => new RevenueSummaryModel
            {
                Period = date.ToString("yyyy-MM-dd"),
                TotalRevenue = g.Sum(o => o.TotalAmount),
                TotalOrders = g.Count(),
                TotalItemsSold = g.Sum(o => o.Items.Sum(oi => oi.Quantity))
            })
            .FirstOrDefaultAsync();

        return result ?? new RevenueSummaryModel
        {
            Period = date.ToString("yyyy-MM-dd"),
            TotalRevenue = 0,
            TotalOrders = 0,
            TotalItemsSold = 0
        };
    }

    // ═══════════════════════════════════════════════════════════════
    //                    REVENUE BY TIMEFRAME - WEEKLY
    // ═══════════════════════════════════════════════════════════════

    public async Task<RevenueSummaryModel> GetWeeklyRevenueAsync(DateTime weekStart)
    {
        var startOfWeek = weekStart.Date;
        var endOfWeek = startOfWeek.AddDays(7);

        var result = await _context.Orders
            .Where(o => o.CreatedTime >= startOfWeek && o.CreatedTime < endOfWeek
                && o.Status == OrderStatus.Completed)
            .GroupBy(o => 1)
            .Select(g => new RevenueSummaryModel
            {
                Period = $"{startOfWeek:yyyy-MM-dd} to {endOfWeek.AddDays(-1):yyyy-MM-dd}",
                TotalRevenue = g.Sum(o => o.TotalAmount),
                TotalOrders = g.Count(),
                TotalItemsSold = g.Sum(o => o.Items.Sum(oi => oi.Quantity))
            })
            .FirstOrDefaultAsync();

        return result ?? new RevenueSummaryModel
        {
            Period = "No data",
            TotalRevenue = 0,
            TotalOrders = 0,
            TotalItemsSold = 0
        };
    }

    // ═══════════════════════════════════════════════════════════════
    //                    REVENUE BY TIMEFRAME - MONTHLY
    // ═══════════════════════════════════════════════════════════════

    public async Task<RevenueSummaryModel> GetMonthlyRevenueAsync(int year, int month)
    {
        var startOfMonth = new DateTime(year, month, 1);
        var endOfMonth = startOfMonth.AddMonths(1);

        var result = await _context.Orders
            .Where(o => o.CreatedTime >= startOfMonth && o.CreatedTime < endOfMonth
                && o.Status == OrderStatus.Completed)
            .GroupBy(o => 1)
            .Select(g => new RevenueSummaryModel
            {
                Period = startOfMonth.ToString("yyyy-MM"),
                TotalRevenue = g.Sum(o => o.TotalAmount),
                TotalOrders = g.Count(),
                TotalItemsSold = g.Sum(o => o.Items.Sum(oi => oi.Quantity))
            })
            .FirstOrDefaultAsync();

        return result ?? new RevenueSummaryModel
        {
            Period = $"{year}-{month:D2}",
            TotalRevenue = 0,
            TotalOrders = 0,
            TotalItemsSold = 0
        };
    }

    // ═══════════════════════════════════════════════════════════════
    //                    REVENUE BY TIMEFRAME - YEARLY
    // ═══════════════════════════════════════════════════════════════

    public async Task<RevenueSummaryModel> GetYearlyRevenueAsync(int year)
    {
        var startOfYear = new DateTime(year, 1, 1);
        var endOfYear = startOfYear.AddYears(1);

        var result = await _context.Orders
            .Where(o => o.CreatedTime >= startOfYear && o.CreatedTime < endOfYear
                && o.Status == OrderStatus.Completed)
            .GroupBy(o => 1)
            .Select(g => new RevenueSummaryModel
            {
                Period = year.ToString(),
                TotalRevenue = g.Sum(o => o.TotalAmount),
                TotalOrders = g.Count(),
                TotalItemsSold = g.Sum(o => o.Items.Sum(oi => oi.Quantity))
            })
            .FirstOrDefaultAsync();

        return result ?? new RevenueSummaryModel
        {
            Period = year.ToString(),
            TotalRevenue = 0,
            TotalOrders = 0,
            TotalItemsSold = 0
        };
    }

    // ═══════════════════════════════════════════════════════════════
    //                    REVENUE BY TIMEFRAME - QUARTERLY
    // ═══════════════════════════════════════════════════════════════

    public async Task<RevenueSummaryModel> GetQuarterlyRevenueAsync(int year, int quarter)
    {
        var startQuarter = new DateTime(year, (quarter - 1) * 3 + 1, 1);
        var endQuarter = startQuarter.AddMonths(3);

        var result = await _context.Orders
            .Where(o => o.CreatedTime >= startQuarter && o.CreatedTime < endQuarter
                && o.Status == OrderStatus.Completed)
            .GroupBy(o => 1)
            .Select(g => new RevenueSummaryModel
            {
                Period = $"Q{quarter} {year}",
                TotalRevenue = g.Sum(o => o.TotalAmount),
                TotalOrders = g.Count(),
                TotalItemsSold = g.Sum(o => o.Items.Sum(oi => oi.Quantity))
            })
            .FirstOrDefaultAsync();

        return result ?? new RevenueSummaryModel
        {
            Period = $"Q{quarter} {year}",
            TotalRevenue = 0,
            TotalOrders = 0,
            TotalItemsSold = 0
        };
    }

    // ═══════════════════════════════════════════════════════════════
    //                    PRODUCT SPECIFIC SALES
    // ═══════════════════════════════════════════════════════════════

    public async Task<ProductSalesDetailModel> GetProductSalesDetailAsync(long productId)
    {
        return await _context.Products
            .Where(p => p.Id == productId)
            .Select(p => new ProductSalesDetailModel
            {
                ProductId = p.Id,
                ProductName = p.Name,
                CategoryName = p.Category.Name,
                BrandName = p.Brand.Name,
                CurrentPrice = p.Price,
                TotalQuantitySold = p.OrderItems.Sum(oi => oi.Quantity),
                TotalRevenue = p.OrderItems.Sum(oi => oi.Quantity * oi.UnitPrice),
                OrderCount = p.OrderItems.Select(oi => oi.OrderId).Distinct().Count(),
                AverageRating = p.Reviews.Average(r => (decimal?)r.Rating) ?? 0,
                ReviewCount = p.Reviews.Count()
            })
            .FirstOrDefaultAsync();
    }

    // ═══════════════════════════════════════════════════════════════
    //                    DASHBOARD SUMMARY
    // ═══════════════════════════════════════════════════════════════

    public async Task<DashboardSummaryModel> GetDashboardSummaryAsync()
    {
        var now = DateTime.UtcNow;
        var startOfToday = now.Date;
        var startOfWeek = now.Date.AddDays(-(int)now.DayOfWeek);
        var startOfMonth = new DateTime(now.Year, now.Month, 1);
        var startOfYear = new DateTime(now.Year, 1, 1);

        // Total stats
        var totalProducts = await _context.Products.CountAsync(p => p.IsActive);
        var totalOrders = await _context.Orders.CountAsync();
        var totalRevenue = await _context.Orders
            .Where(o => o.Status == OrderStatus.Completed)
            .SumAsync(o => o.TotalAmount);

        // Today's stats
        var todayRevenue = await _context.Orders
            .Where(o => o.CreatedTime >= startOfToday && o.Status == OrderStatus.Completed)
            .SumAsync(o => o.TotalAmount);
        var todayOrders = await _context.Orders
            .CountAsync(o => o.CreatedTime >= startOfToday);
        var todayItemsSold = await _context.OrderItems
            .CountAsync(oi => oi.Order.CreatedTime >= startOfToday);

        // This week's stats
        var weekRevenue = await _context.Orders
            .Where(o => o.CreatedTime >= startOfWeek && o.Status == OrderStatus.Completed)
            .SumAsync(o => o.TotalAmount);

        // This month's stats
        var monthRevenue = await _context.Orders
            .Where(o => o.CreatedTime >= startOfMonth && o.Status == OrderStatus.Completed)
            .SumAsync(o => o.TotalAmount);

        // This year's stats
        var yearRevenue = await _context.Orders
            .Where(o => o.CreatedTime >= startOfYear && o.Status == OrderStatus.Completed)
            .SumAsync(o => o.TotalAmount);

        // Top selling products (all time)
        var topSelling = await GetTopSellingProductsAsync(5);

        // Recent orders
        var recentOrders = await _context.Orders
            .OrderByDescending(o => o.CreatedTime)
            .Take(5)
            .Select(o => new RecentOrderModel
            {
                OrderId = o.Id,
                CustomerName = $"{o.Customer.FirstName} {o.Customer.LastName}",
                TotalAmount = o.TotalAmount,
                Status = o.Status,
                CreatedAt = o.CreatedTime
            })
            .AsNoTracking()
            .ToListAsync();

        return new DashboardSummaryModel
        {
            TotalProducts = totalProducts,
            TotalOrders = totalOrders,
            TotalRevenue = totalRevenue,
            TodayRevenue = todayRevenue,
            TodayOrders = todayOrders,
            TodayItemsSold = todayItemsSold,
            WeekRevenue = weekRevenue,
            MonthRevenue = monthRevenue,
            YearRevenue = yearRevenue,
            TopSellingProducts = topSelling,
            RecentOrders = recentOrders
        };
    }

}
