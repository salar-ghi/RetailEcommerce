namespace Domain.IRepositories;

public interface ICategoryRepository : IRepository<Category, int>
{
    Task<IEnumerable<Category>> SearchByNameAsync(string name);
    Task<IEnumerable<Category>> SearchByDescriptionAsync(string description);
    Task<bool> ExistsByNameAsync(string normalizedName, int? excludedId = null);
}
