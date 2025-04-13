namespace Domain.IRepositories;

public interface IProductImageRepository : IRepository<ProductImage, long> 
{
    Task<IEnumerable<ProductImage>> GetByProductIdAsync(int productId);
    Task<IEnumerable<ProductImage>> SearchByPrimaryAsync(bool isPrimary);
}
