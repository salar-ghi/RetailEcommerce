namespace Domain.IRepositories;

public interface IAttributeDefinitionRepository : IRepository<AttributeDefinition, int>
{
    Task<IReadOnlyList<AttributeDefinition>> GetActiveWithOptionsAsync();
    Task<AttributeDefinition?> GetActiveWithOptionsAsync(int id, bool trackChanges = false);
    Task<AttributeDefinition?> GetActiveByCodeAndDataTypeAsync(string code, AttributeDataType dataType, bool trackChanges = false);
    Task<bool> ActiveCodeAndDataTypeExistsAsync(string code, AttributeDataType dataType, int? excludedId = null);
}
