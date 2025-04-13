namespace Domain.IRepositories;

public interface IProductVariantRepository : IRepository<ProductVariant, long> 
{
    Task<IEnumerable<ProductVariant>> GetByProductIdAsync(int productId);
    Task<IEnumerable<ProductVariant>> SearchByVariantNameAsync(string variantName);

}