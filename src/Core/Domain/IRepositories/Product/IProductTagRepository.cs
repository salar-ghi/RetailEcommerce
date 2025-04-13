namespace Domain.IRepositories;

public interface IProductTagRepository : IRepository<ProductTag, int>
{
    Task<IEnumerable<ProductTag>> GetByProductIdAsync(int productId);
    Task<IEnumerable<ProductTag>> GetByTagIdAsync(int tagId);
    Task DeleteAsync(int productId, int tagId);
}