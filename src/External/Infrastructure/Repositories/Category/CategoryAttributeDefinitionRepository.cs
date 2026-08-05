namespace Infrastructure.Repositories;

public class CategoryAttributeDefinitionRepository : Repository<CategoryAttributeDefinition, int>, ICategoryAttributeDefinitionRepository
{
    public CategoryAttributeDefinitionRepository(AppDbContext context) : base(context) { }

    public async Task<IReadOnlyList<CategoryAttributeDefinition>> GetActiveByCategoryIdAsync(int categoryId)
    {
        return await IncludeActiveAttributeDefinition(_context.CategoryAttributeDefinitions.AsNoTracking())
            .Where(ca => ca.CategoryId == categoryId && !ca.IsDeleted && !ca.AttributeDefinition.IsDeleted)
            .OrderBy(ca => ca.SortOrder)
            .ThenBy(ca => ca.AttributeDefinition.SortOrder)
            .ThenBy(ca => ca.AttributeDefinition.Name)
            .ToListAsync();
    }

    public async Task<CategoryAttributeDefinition?> GetActiveByIdAsync(int categoryId, int id, bool trackChanges = false)
    {
        var query = trackChanges
            ? _context.CategoryAttributeDefinitions.AsTracking()
            : _context.CategoryAttributeDefinitions.AsNoTracking();

        return await query.FirstOrDefaultAsync(ca => ca.Id == id && ca.CategoryId == categoryId && !ca.IsDeleted);
    }

    public async Task<bool> ActiveAssignmentExistsAsync(int categoryId, int attributeDefinitionId)
    {
        return await _context.CategoryAttributeDefinitions.AnyAsync(ca =>
            ca.CategoryId == categoryId && ca.AttributeDefinitionId == attributeDefinitionId && !ca.IsDeleted);
    }

    public async Task<bool> ActiveAttributeDefinitionExistsAsync(int attributeDefinitionId)
    {
        return await _context.AttributeDefinitions.AnyAsync(a => a.Id == attributeDefinitionId && !a.IsDeleted);
    }

    public async Task<CategoryAttributeDefinition?> GetActiveWithAttributeDefinitionAsync(int categoryId, int id, bool trackChanges = false)
    {
        var query = trackChanges
            ? _context.CategoryAttributeDefinitions.AsTracking()
            : _context.CategoryAttributeDefinitions.AsNoTracking();

        return await IncludeActiveAttributeDefinition(query)
            .FirstOrDefaultAsync(ca => ca.Id == id && ca.CategoryId == categoryId && !ca.IsDeleted && !ca.AttributeDefinition.IsDeleted);
    }

    public async Task<CategoryAttributeDefinition?> GetActiveWithAttributeDefinitionAsync(int id)
    {
        return await IncludeActiveAttributeDefinition(_context.CategoryAttributeDefinitions.AsNoTracking())
            .FirstOrDefaultAsync(ca => ca.Id == id && !ca.IsDeleted && !ca.AttributeDefinition.IsDeleted);
    }

    private static IQueryable<CategoryAttributeDefinition> IncludeActiveAttributeDefinition(IQueryable<CategoryAttributeDefinition> query)
    {
        return query
            .Include(ca => ca.AttributeDefinition)
            .ThenInclude(a => a.Options.Where(o => !o.IsDeleted));
    }
}
