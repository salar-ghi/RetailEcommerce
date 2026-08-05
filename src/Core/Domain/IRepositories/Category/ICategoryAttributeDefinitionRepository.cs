namespace Domain.IRepositories;

public interface ICategoryAttributeDefinitionRepository : IRepository<CategoryAttributeDefinition, int>
{
    Task<IReadOnlyList<CategoryAttributeDefinition>> GetActiveByCategoryIdAsync(int categoryId);
    Task<CategoryAttributeDefinition?> GetActiveByIdAsync(int categoryId, int id, bool trackChanges = false);
    Task<bool> ActiveAssignmentExistsAsync(int categoryId, int attributeDefinitionId);
    Task<bool> ActiveAttributeDefinitionExistsAsync(int attributeDefinitionId);
    Task<CategoryAttributeDefinition?> GetActiveWithAttributeDefinitionAsync(int categoryId, int id, bool trackChanges = false);
    Task<CategoryAttributeDefinition?> GetActiveWithAttributeDefinitionAsync(int id);
}
