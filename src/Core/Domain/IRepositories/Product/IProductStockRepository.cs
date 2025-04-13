namespace Domain.IRepositories;

public interface IProductStockRepository : IRepository<ProductStock, long> 
{
    Task<ProductStock> GetByProductIdAsync(int productId);
    Task<IEnumerable<ProductStock>> SearchByLowStockAsync(int threshold);

}
