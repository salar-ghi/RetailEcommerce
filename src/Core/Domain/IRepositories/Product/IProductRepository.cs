namespace Domain.IRepositories;

public interface IProductRepository : IRepository<Product, long>
{
    Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId);
    Task<IEnumerable<Product>> SearchByNameAsync(string name);

    Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm, int page = 1, int pageSize = 10);
    Task<IEnumerable<Product>> GetProductsByBrandAsync(int brandId);
    Task<IEnumerable<Product>> GetProductsBySupplierAsync(int supplierId);
    Task<IEnumerable<Product>> GetProductsByTagAsync(int tagId);
    Task<IEnumerable<Product>> GetProductsByPriceRangeAsync(decimal minPrice, decimal maxPrice);

}
