namespace Domain.IRepositories;

public interface IProductSupplierRepository : IRepository<ProductSupplier, int> 
{
    Task<IEnumerable<ProductSupplier>> GetByProductIdAsync(int productId);
    Task<IEnumerable<ProductSupplier>> GetBySupplierIdAsync(int supplierId);
    Task DeleteAsync(int productId, int supplierId);
}