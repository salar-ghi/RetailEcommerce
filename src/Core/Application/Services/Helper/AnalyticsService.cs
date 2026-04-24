using Domain.IRepositories;

namespace Application.Services;

public class AnalyticsService : IAnalyticsService
{
    private readonly IAnalyticsRepository _analyticsRepository;

    private readonly IMapper _mapper;

    public AnalyticsService(
        IAnalyticsRepository analyticsRepository,
        IMapper mapper)
    {
        _analyticsRepository = analyticsRepository;
        _mapper = mapper;
    }

    public Task<List<ProductDto>> GetCheapestProductsAsync(int top = 10)
    {
        throw new NotImplementedException();
    }

    public Task<List<ProductDto>> GetCheapestProductsInCategoryAsync(string categoryName, int top = 10)
    {
        throw new NotImplementedException();
    }

    public Task<RevenueSummaryDto> GetDailyRevenueAsync(DateTime date)
    {
        throw new NotImplementedException();
    }

    public Task<DashboardSummaryDto> GetDashboardSummaryAsync()
    {
        throw new NotImplementedException();
    }

    public Task<List<ProductSalesDto>> GetLeastSellingProductsAsync(int top = 10)
    {
        throw new NotImplementedException();
    }

    public Task<List<ProductSalesDto>> GetLeastSellingProductsInCategoryAsync(string categoryName, int top = 10)
    {
        throw new NotImplementedException();
    }

    public Task<RevenueSummaryDto> GetMonthlyRevenueAsync(int year, int month)
    {
        throw new NotImplementedException();
    }

    public Task<List<ProductDto>> GetMostExpensiveProductsAsync(int top = 10)
    {
        throw new NotImplementedException();
    }

    public Task<List<ProductDto>> GetMostExpensiveProductsInCategoryAsync(string categoryName, int top = 10)
    {
        throw new NotImplementedException();
    }

    public Task<List<ProductSearchDto>> GetMostSearchedProductsAsync(int top = 10)
    {
        throw new NotImplementedException();
    }

    public Task<List<ProductSearchDto>> GetMostSearchedProductsInCategoryAsync(string categoryName, int top = 10)
    {
        throw new NotImplementedException();
    }

    public Task<ProductSalesDetailDto> GetProductSalesDetailAsync(long productId)
    {
        throw new NotImplementedException();
    }

    public Task<RevenueSummaryDto> GetQuarterlyRevenueAsync(int year, int quarter)
    {
        throw new NotImplementedException();
    }

    public Task<List<ProductSalesDto>> GetTopSellingProductsAsync(int top = 10)
    {
        throw new NotImplementedException();
    }

    public Task<List<ProductSalesDto>> GetTopSellingProductsByBrandAsync(string brandName, int top = 10)
    {
        throw new NotImplementedException();
    }

    public Task<List<ProductSalesDto>> GetTopSellingProductsDailyAsync(DateTime date, int top = 10)
    {
        throw new NotImplementedException();
    }

    public Task<List<ProductSalesDto>> GetTopSellingProductsInCategoryAsync(string categoryName, int top = 10)
    {
        throw new NotImplementedException();
    }

    public Task<List<ProductSalesDto>> GetTopSellingProductsMonthlyAsync(int year, int month, int top = 10)
    {
        throw new NotImplementedException();
    }

    public Task<List<ProductSalesDto>> GetTopSellingProductsQuarterlyAsync(int year, int quarter, int top = 10)
    {
        throw new NotImplementedException();
    }

    public Task<List<ProductSalesDto>> GetTopSellingProductsWeeklyAsync(DateTime weekStart, int top = 10)
    {
        throw new NotImplementedException();
    }

    public Task<List<ProductSalesDto>> GetTopSellingProductsYearlyAsync(int year, int top = 10)
    {
        throw new NotImplementedException();
    }

    public Task<RevenueSummaryDto> GetWeeklyRevenueAsync(DateTime weekStart)
    {
        throw new NotImplementedException();
    }

    public Task<RevenueSummaryDto> GetYearlyRevenueAsync(int year)
    {
        throw new NotImplementedException();
    }

    // ==================== MOST SELLING ====================

    //public async Task<List<ProductSalesDto>> GetTopSellingProductsAsync(int top = 10)
    //{
    //    return await _analyticsRepository.GetTopSellingProductsAsync(top);
    //}

    //public async Task<List<ProductSalesDto>> GetTopSellingProductsInCategoryAsync(
    //    string categoryName,
    //    int top = 10)
    //{
    //    return await _analyticsRepository.GetTopSellingProductsInCategoryAsync(categoryName, top);
    //}

    //public async Task<List<ProductSalesDto>> GetTopSellingProductsByBrandAsync(
    //    string brandName,
    //    int top = 10)
    //{
    //     var result = await _analyticsRepository.GetTopSellingProductsByBrandAsync(brandName, top);
    //    return result;
    //}

    // ==================== LEAST SELLING ====================

    //public async Task<List<ProductSalesDto>> GetLeastSellingProductsAsync(int top = 10)
    //{
    //    return await _analyticsRepository.GetLeastSellingProductsAsync(top);
    //}

    //public async Task<List<ProductSalesDto>> GetLeastSellingProductsInCategoryAsync(
    //    string categoryName,
    //    int top = 10)
    //{
    //    return await _analyticsRepository.GetLeastSellingProductsInCategoryAsync(categoryName, top);
    //}

    // ==================== MOST EXPENSIVE / CHEAPEST ====================

    //public async Task<List<ProductDto>> GetMostExpensiveProductsAsync(int top = 10)
    //{
    //    return await _analyticsRepository.GetMostExpensiveProductsAsync(top);
    //}

    //public async Task<List<ProductDto>> GetMostExpensiveProductsInCategoryAsync(
    //    string categoryName,
    //    int top = 10)
    //{
    //    return await _analyticsRepository.GetMostExpensiveProductsInCategoryAsync(categoryName, top);
    //}

    //public async Task<List<ProductDto>> GetCheapestProductsAsync(int top = 10)
    //{
    //    return await _analyticsRepository.GetCheapestProductsAsync(top);
    //}

    //public async Task<List<ProductDto>> GetCheapestProductsInCategoryAsync(
    //    string categoryName,
    //    int top = 10)
    //{
    //    return await _analyticsRepository.GetCheapestProductsInCategoryAsync(categoryName, top);
    //}

    // ==================== MOST SEARCHED ====================

    //public async Task<List<ProductSearchDto>> GetMostSearchedProductsAsync(int top = 10)
    //{
    //    return await _analyticsRepository.GetMostSearchedProductsAsync(top);
    //}

    //public async Task<List<ProductSearchDto>> GetMostSearchedProductsInCategoryAsync(
    //    string categoryName,
    //    int top = 10)
    //{
    //    return await _analyticsRepository.GetMostSearchedProductsInCategoryAsync(categoryName, top);
    //}

    // ==================== SALES BY TIMEFRAME ====================

    //public async Task<List<ProductSalesDto>> GetTopSellingProductsDailyAsync(
    //    DateTime date,
    //    int top = 10)
    //{
    //    return await _analyticsRepository.GetTopSellingProductsDailyAsync(date, top);
    //}

    //public async Task<List<ProductSalesDto>> GetTopSellingProductsWeeklyAsync(
    //    DateTime weekStart,
    //    int top = 10)
    //{
    //    return await _analyticsRepository.GetTopSellingProductsWeeklyAsync(weekStart, top);
    //}

    //public async Task<List<ProductSalesDto>> GetTopSellingProductsMonthlyAsync(
    //    int year,
    //    int month,
    //    int top = 10)
    //{
    //    return await _analyticsRepository.GetTopSellingProductsMonthlyAsync(year, month, top);
    //}

    //public async Task<List<ProductSalesDto>> GetTopSellingProductsYearlyAsync(
    //    int year,
    //    int top = 10)
    //{
    //    return await _analyticsRepository.GetTopSellingProductsYearlyAsync(year, top);
    //}

    //public async Task<List<ProductSalesDto>> GetTopSellingProductsQuarterlyAsync(
    //    int year,
    //    int quarter,
    //    int top = 10)
    //{
    //    return await _analyticsRepository.GetTopSellingProductsQuarterlyAsync(year, quarter, top);
    //}

    // ==================== REVENUE BY TIMEFRAME ====================

    //public async Task<RevenueSummaryDto> GetDailyRevenueAsync(DateTime date)
    //{
    //    return await _analyticsRepository.GetDailyRevenueAsync(date);
    //}

    //public async Task<RevenueSummaryDto> GetWeeklyRevenueAsync(DateTime weekStart)
    //{
    //    return await _analyticsRepository.GetWeeklyRevenueAsync(weekStart);
    //}

    //public async Task<RevenueSummaryDto> GetMonthlyRevenueAsync(int year, int month)
    //{
    //    return await _analyticsRepository.GetMonthlyRevenueAsync(year, month);
    //}

    //public async Task<RevenueSummaryDto> GetYearlyRevenueAsync(int year)
    //{
    //    return await _analyticsRepository.GetYearlyRevenueAsync(year);
    //}

    //public async Task<RevenueSummaryDto> GetQuarterlyRevenueAsync(int year, int quarter)
    //{
    //    return await _analyticsRepository.GetQuarterlyRevenueAsync(year, quarter);
    //}

    // ==================== PRODUCT SPECIFIC ====================

    //public async Task<ProductSalesDetailDto> GetProductSalesDetailAsync(long productId)
    //{
    //    var detail = await _analyticsRepository.GetProductSalesDetailAsync(productId);
    //    return detail ?? throw new KeyNotFoundException($"Product with ID {productId} not found.");
    //}

    // ==================== DASHBOARD ====================

    //public async Task<DashboardSummaryDto> GetDashboardSummaryAsync()
    //{
    //    return await _analyticsRepository.GetDashboardSummaryAsync();
    //}
}
