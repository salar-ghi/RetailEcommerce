namespace Domain.IRepositories;

public interface IProductStockRepository : IRepository<ProductStock, long> 
{
    Task<ProductStock> GetByProductIdAsync(int productId);
    Task<IEnumerable<ProductStock>> SearchByLowStockAsync(int threshold);
    Task<IEnumerable<ProductStock>> GetByProductLocationsAsync(long productId);
    Task<IEnumerable<ProductStock>> GetByStorageScopeAsync(int? spaceId, int? zoneId, int? shelfId);

}
