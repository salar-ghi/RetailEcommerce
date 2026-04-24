namespace Domain.Models;

internal class AnalyticsModel
{
}


public class ProductSalesModel
{
    public long ProductId { get; set; }
    public string ProductName { get; set; }
    public string CategoryName { get; set; }
    public string BrandName { get; set; }
    public int TotalQuantitySold { get; set; }
    public decimal TotalRevenue { get; set; }
    public int OrderCount { get; set; }
}

public class ProductModel
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public string CategoryName { get; set; }
    public string BrandName { get; set; }
    public string ImageUrl { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class ProductSearchModel
{
    public long ProductId { get; set; }
    public string ProductName { get; set; }
    public string CategoryName { get; set; }
    public string BrandName { get; set; }
    public int SearchCount { get; set; }
    public int UniqueUsers { get; set; }
}

public class RevenueSummaryModel
{
    public string Period { get; set; }
    public decimal TotalRevenue { get; set; }
    public int TotalOrders { get; set; }
    public int TotalItemsSold { get; set; }
}

public class ProductSalesDetailModel
{
    public long ProductId { get; set; }
    public string ProductName { get; set; }
    public string CategoryName { get; set; }
    public string BrandName { get; set; }
    public decimal CurrentPrice { get; set; }
    public int TotalQuantitySold { get; set; }
    public decimal TotalRevenue { get; set; }
    public int OrderCount { get; set; }
    public decimal AverageRating { get; set; }
    public int ReviewCount { get; set; }
}

public class RecentOrderModel
{
    public string OrderId { get; set; }
    public string CustomerName { get; set; }
    public decimal TotalAmount { get; set; }
    public OrderStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class DashboardSummaryModel
{
    public int TotalProducts { get; set; }
    public int TotalOrders { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal TodayRevenue { get; set; }
    public int TodayOrders { get; set; }
    public int TodayItemsSold { get; set; }
    public decimal WeekRevenue { get; set; }
    public decimal MonthRevenue { get; set; }
    public decimal YearRevenue { get; set; }
    public List<ProductSalesModel> TopSellingProducts { get; set; } = new();
    public List<RecentOrderModel> RecentOrders { get; set; } = new();
}