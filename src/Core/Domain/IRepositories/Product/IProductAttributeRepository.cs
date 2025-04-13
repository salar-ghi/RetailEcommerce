namespace Domain.IRepositories;

public interface IProductAttributeRepository : IRepository<ProductAttribute, long> 
{
    Task<IEnumerable<ProductAttribute>> GetByProductIdAsync(int productId);
    Task<IEnumerable<ProductAttribute>> SearchByKeyAsync(string key);

}