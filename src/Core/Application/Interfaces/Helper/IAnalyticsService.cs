namespace Application.Interfaces;

public interface IAnalyticsService
{
    // ==================== MOST SELLING ====================
    Task<List<ProductSalesDto>> GetTopSellingProductsAsync(int top = 10);
    Task<List<ProductSalesDto>> GetTopSellingProductsInCategoryAsync(string categoryName, int top = 10);
    Task<List<ProductSalesDto>> GetTopSellingProductsByBrandAsync(string brandName, int top = 10);

    // ==================== LEAST SELLING ====================
    Task<List<ProductSalesDto>> GetLeastSellingProductsAsync(int top = 10);
    Task<List<ProductSalesDto>> GetLeastSellingProductsInCategoryAsync(string categoryName, int top = 10);

    // ==================== MOST EXPENSIVE / CHEAPEST ====================
    Task<List<ProductDto>> GetMostExpensiveProductsAsync(int top = 10);
    Task<List<ProductDto>> GetMostExpensiveProductsInCategoryAsync(string categoryName, int top = 10);
    Task<List<ProductDto>> GetCheapestProductsAsync(int top = 10);
    Task<List<ProductDto>> GetCheapestProductsInCategoryAsync(string categoryName, int top = 10);

    // ==================== MOST SEARCHED ====================
    Task<List<ProductSearchDto>> GetMostSearchedProductsAsync(int top = 10);
    Task<List<ProductSearchDto>> GetMostSearchedProductsInCategoryAsync(string categoryName, int top = 10);

    // ==================== SALES BY TIMEFRAME ====================
    Task<List<ProductSalesDto>> GetTopSellingProductsDailyAsync(DateTime date, int top = 10);
    Task<List<ProductSalesDto>> GetTopSellingProductsWeeklyAsync(DateTime weekStart, int top = 10);
    Task<List<ProductSalesDto>> GetTopSellingProductsMonthlyAsync(int year, int month, int top = 10);
    Task<List<ProductSalesDto>> GetTopSellingProductsYearlyAsync(int year, int top = 10);
    Task<List<ProductSalesDto>> GetTopSellingProductsQuarterlyAsync(int year, int quarter, int top = 10);

    // ==================== REVENUE BY TIMEFRAME ====================
    Task<RevenueSummaryDto> GetDailyRevenueAsync(DateTime date);
    Task<RevenueSummaryDto> GetWeeklyRevenueAsync(DateTime weekStart);
    Task<RevenueSummaryDto> GetMonthlyRevenueAsync(int year, int month);
    Task<RevenueSummaryDto> GetYearlyRevenueAsync(int year);
    Task<RevenueSummaryDto> GetQuarterlyRevenueAsync(int year, int quarter);

    // ==================== PRODUCT SPECIFIC ====================
    Task<ProductSalesDetailDto> GetProductSalesDetailAsync(long productId);

    // ==================== DASHBOARD ====================
    Task<DashboardSummaryDto> GetDashboardSummaryAsync();

}
