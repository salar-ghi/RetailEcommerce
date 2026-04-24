namespace Domain.IRepositories;

public interface IAnalyticsRepository
{
    // ==================== MOST SELLING ====================
    Task<List<ProductSalesModel>> GetTopSellingProductsAsync(int top = 10);
    Task<List<ProductSalesModel>> GetTopSellingProductsInCategoryAsync(string categoryName, int top = 10);
    Task<List<ProductSalesModel>> GetTopSellingProductsByBrandAsync(string brandName, int top = 10);

    // ==================== LEAST SELLING ====================
    Task<List<ProductSalesModel>> GetLeastSellingProductsAsync(int top = 10);
    Task<List<ProductSalesModel>> GetLeastSellingProductsInCategoryAsync(string categoryName, int top = 10);

    // ==================== MOST EXPENSIVE / CHEAPEST ====================
    Task<List<ProductModel>> GetMostExpensiveProductsAsync(int top = 10);
    Task<List<ProductModel>> GetMostExpensiveProductsInCategoryAsync(string categoryName, int top = 10);
    Task<List<ProductModel>> GetCheapestProductsAsync(int top = 10);
    Task<List<ProductModel>> GetCheapestProductsInCategoryAsync(string categoryName, int top = 10);

    // ==================== MOST SEARCHED ====================
    //Task<List<ProductSearchModel>> GetMostSearchedProductsAsync(int top = 10);
    //Task<List<ProductSearchModel>> GetMostSearchedProductsInCategoryAsync(string categoryName, int top = 10);

    // ==================== SALES BY TIMEFRAME ====================
    Task<List<ProductSalesModel>> GetTopSellingProductsDailyAsync(DateTime date, int top = 10);
    Task<List<ProductSalesModel>> GetTopSellingProductsWeeklyAsync(DateTime weekStart, int top = 10);
    Task<List<ProductSalesModel>> GetTopSellingProductsMonthlyAsync(int year, int month, int top = 10);
    Task<List<ProductSalesModel>> GetTopSellingProductsYearlyAsync(int year, int top = 10);
    Task<List<ProductSalesModel>> GetTopSellingProductsQuarterlyAsync(int year, int quarter, int top = 10);

    // ==================== REVENUE BY TIMEFRAME ====================
    Task<RevenueSummaryModel> GetDailyRevenueAsync(DateTime date);
    Task<RevenueSummaryModel> GetWeeklyRevenueAsync(DateTime weekStart);
    Task<RevenueSummaryModel> GetMonthlyRevenueAsync(int year, int month);
    Task<RevenueSummaryModel> GetYearlyRevenueAsync(int year);
    Task<RevenueSummaryModel> GetQuarterlyRevenueAsync(int year, int quarter);

    // ==================== PRODUCT SPECIFIC ====================
    Task<ProductSalesDetailModel> GetProductSalesDetailAsync(long productId);

    // ==================== DASHBOARD ====================
    Task<DashboardSummaryModel> GetDashboardSummaryAsync();
}
