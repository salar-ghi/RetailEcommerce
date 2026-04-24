namespace Application.Interfaces;

public interface IProductService
{
    Task<IEnumerable<ProductDto>> GetAllProductsAsync();
    Task<ProductDto> GetProductByIdAsync(int id);
    Task<IEnumerable<ProductDto>> GetProductsByCategory(string categoryName);

    //// ==================== MOST SELLING ====================
    //Task<List<ProductSalesDto>> GetTopSellingProductsAsync(int top = 10);
    //Task<List<ProductSalesDto>> GetTopSellingProductsInCategoryAsync(string categoryName, int top = 10);
    //Task<List<ProductSalesDto>> GetTopSellingProductsByBrandAsync(string brandName, int top = 10);

}
