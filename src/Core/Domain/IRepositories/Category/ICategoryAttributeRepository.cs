namespace Domain.IRepositories;

public interface ICategoryAttributeRepository : IRepository<CategoryAttribute, int> 
{
    Task<IEnumerable<CategoryAttribute>> GetByCategoryIdAsync(int categoryId);
    Task<IEnumerable<CategoryAttribute>> SearchByKeyAsync(string key);
}