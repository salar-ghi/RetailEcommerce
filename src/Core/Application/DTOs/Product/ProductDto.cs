using Domain;
using Domain.Models;
using System.Text.Json.Serialization;

namespace Application.DTOs;

public class ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Status { get; set; }
    public string Availability { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public string CategoryName { get; set; }
    public string BrandName { get; set; }
    public string SupplierName { get; set; }
    public List<string> Images { get; set; }
    public string CoverImage { get; set; }
    public DimensionDto Dimensions { get; set; }
    public List<BatchDto> Prices { get; set; }
    public List<AttributeDto> Attributes { get; set; }
    public List<string> Tags { get; set; }
    public string? PricingStrategy { get; set; }
    public SalesUnitConfigDto? SalesUnit { get; set; }
}

public class BatchDto
{
    public int Id { get; set; }
    public string BatchNumber { get; set; }
    public decimal Amount { get; set; }          // selling price
    public decimal CostPrice { get; set; }
    public string Currency { get; set; }
    public string PricingTier { get; set; }
    public DateTime EffectiveDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public int Quantity { get; set; }
    public int SoldQuantity { get; set; }
    public string Notes { get; set; }
}


public class ProductSalesDto
{
    public long ProductId { get; set; }
    public string ProductName { get; set; }
    public string CategoryName { get; set; }
    public string BrandName { get; set; }
    public int TotalQuantitySold { get; set; }
    public decimal TotalRevenue { get; set; }
    public int OrderCount { get; set; }
}


public class ProductSearchDto
{
    public long ProductId { get; set; }
    public string ProductName { get; set; }
    public string CategoryName { get; set; }
    public int SearchCount { get; set; }
    public int UniqueUsers { get; set; }
}

public class RevenueSummaryDto
{
    public string Period { get; set; }
    public decimal TotalRevenue { get; set; }
    public int TotalOrders { get; set; }
    public int TotalItemsSold { get; set; }
}

public class ProductSalesDetailDto
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

public class DashboardSummaryDto
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
    public List<ProductSalesDto> TopSellingProducts { get; set; } = new();
    public List<RecentOrderDto> RecentOrders { get; set; } = new();
}

public class RecentOrderDto
{
    public string OrderId { get; set; }
    public string CustomerName { get; set; }
    public decimal TotalAmount { get; set; }
    public OrderStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
}

    //[JsonPropertyName("name")]
public class CreateProductRequest
{
    public string Name { get; set; }
    public string Description { get; set; }
    public int CategoryId { get; set; }
    public int BrandId { get; set; }
    public int SupplierId { get; set; }
    public List<string> Images { get; set; }
    public string CoverImage { get; set; }
    public List<string> Tags { get; set; }

    // Warehouse & Threshold
    public string? Location { get; set; }
    public int? ReorderLevel { get; set; }
    public string? Status { get; set; }
    public string? Availability { get; set; }


    //public int? ReorderThreshold { get; set; }
    //public int? WarehouseId { get; set; }
    public DimensionDto? Dimensions { get; set; }
    public StockDto? Stock { get; set; }
    public List<PriceDto>? Prices { get; set; }
    public List<AttributeDto>? Attributes { get; set; }
    public List<VariantDto>? Variants { get; set; }

    public string? PricingStrategy { get; set; }              // fifo | latest | average
    public SalesUnitConfigDto? SalesUnit { get; set; }        // piece/weight/both mode
}

public class SalesUnitConfigDto
{
    public string Mode { get; set; }                             // piece | weight | both
    public string? WeightUnit { get; set; }                      // gram | kilogram | mithqal | ounce | pound
    public decimal? PricePerWeightUnit { get; set; }
    public decimal? PackWeight { get; set; }
    public string? PackLabel { get; set; }
}
