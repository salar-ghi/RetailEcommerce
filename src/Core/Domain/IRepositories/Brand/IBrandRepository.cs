namespace Domain.IRepositories;

public interface IBrandRepository : IRepository<Brand, int> 
{
    Task<IEnumerable<Brand>> SearchByNameAsync(string name);
    Task<IEnumerable<Brand>> SearchByDescriptionAsync(string description);
    Task<IEnumerable<Brand>> GetAllWithCategoryAsync();
    Task<Brand?> GetByIdWithCategoryAsync(int id, bool asNoTracking = false);
    Task<bool> ExistsByNameAsync(string normalizedName, int? excludedId = null);
}
