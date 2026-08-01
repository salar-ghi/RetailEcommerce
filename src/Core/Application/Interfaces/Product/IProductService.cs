namespace Application.Interfaces;

public interface IProductService
{
    Task<IEnumerable<ProductDto>> GetAllProductsAsync();
    Task<ProductDto> GetProductByIdAsync(int id);
    Task<IEnumerable<ProductDto>> GetProductsByCategory(string categoryName);

    Task<ProductDto> AddProductAsync(CreateProductRequest dto);
    Task<ProductDto> UpdateProductAsync(int id, UpdateProductRequest dto);
    Task DeleteProductAsync(int id);
    Task<IEnumerable<ProductDto>> SearchProductsByNameAsync(string name, int page = 1, int pageSize = 10);
    Task<IEnumerable<ProductDto>> SearchProductsByPriceRangeAsync(decimal minPrice, decimal maxPrice);
    Task<IEnumerable<ProductDto>> SearchProductsByCategoryAsync(int categoryId);
    Task<IEnumerable<ProductAttributeValueDto>> GetProductAttributeValuesAsync(long productId);
    Task SaveProductAttributeValuesAsync(long productId, List<ProductAttributeValueDto> values);
    //// ==================== MOST SELLING ====================
    //Task<List<ProductSalesDto>> GetTopSellingProductsAsync(int top = 10);
    //Task<List<ProductSalesDto>> GetTopSellingProductsInCategoryAsync(string categoryName, int top = 10);
    //Task<List<ProductSalesDto>> GetTopSellingProductsByBrandAsync(string brandName, int top = 10);

}
