namespace Application.Interfaces;

public interface IProductService
{
    Task<IEnumerable<ProductDto>> GetAllProductsAsync();
    Task<ProductDto> GetProductByIdAsync(int id);
    Task<IEnumerable<ProductDto>> GetProductsByCategory(string categoryName);

    Task AddProductAsync(CreateProductRequest dto);
    Task UpdateProductAsync(ProductDto productDto);
    Task DeleteProductAsync(int id);
    Task<IEnumerable<ProductDto>> SearchProductsByNameAsync(string name, int page = 1, int pageSize = 10);
    Task<IEnumerable<ProductDto>> SearchProductsByPriceRangeAsync(decimal minPrice, decimal maxPrice);
    Task<IEnumerable<ProductDto>> SearchProductsByCategoryAsync(int categoryId);
    //// ==================== MOST SELLING ====================
    //Task<List<ProductSalesDto>> GetTopSellingProductsAsync(int top = 10);
    //Task<List<ProductSalesDto>> GetTopSellingProductsInCategoryAsync(string categoryName, int top = 10);
    //Task<List<ProductSalesDto>> GetTopSellingProductsByBrandAsync(string brandName, int top = 10);

}
