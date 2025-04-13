namespace Domain.IRepositories;

public interface IProductUnitPriceRepository : IRepository<ProductUnitPrice, long> 
{
    Task<IEnumerable<ProductUnitPrice>> GetByProductIdAsync(int productId);
    Task<IEnumerable<ProductUnitPrice>> SearchByPriceRangeAsync(decimal minPrice, decimal maxPrice);
}
