namespace Infrastructure.Repositories;

public class AttributeDefinitionRepository : Repository<AttributeDefinition, int>, IAttributeDefinitionRepository
{
    public AttributeDefinitionRepository(AppDbContext context) : base(context) { }

    public async Task<IReadOnlyList<AttributeDefinition>> GetActiveWithOptionsAsync()
    {
        return await IncludeActiveOptions(_context.AttributeDefinitions.AsNoTracking())
            .Where(attribute => !attribute.IsDeleted)
            .OrderBy(attribute => attribute.SortOrder)
            .ThenBy(attribute => attribute.Name)
            .ToListAsync();
    }

    public async Task<AttributeDefinition?> GetActiveWithOptionsAsync(int id, bool trackChanges = false)
    {
        var query = trackChanges
            ? _context.AttributeDefinitions.AsTracking()
            : _context.AttributeDefinitions.AsNoTracking();

        return await IncludeActiveOptions(query)
            .FirstOrDefaultAsync(attribute => attribute.Id == id && !attribute.IsDeleted);
    }

    public async Task<AttributeDefinition?> GetActiveByCodeAndDataTypeAsync(string code, AttributeDataType dataType, bool trackChanges = false)
    {
        var query = trackChanges
            ? _context.AttributeDefinitions.AsTracking()
            : _context.AttributeDefinitions.AsNoTracking();

        return await IncludeActiveOptions(query)
            .FirstOrDefaultAsync(attribute =>
                !attribute.IsDeleted &&
                attribute.Code == code &&
                attribute.DataType == dataType);
    }

    public async Task<bool> ActiveCodeAndDataTypeExistsAsync(string code, AttributeDataType dataType, int? excludedId = null)
    {
        return await _context.AttributeDefinitions
            .AsNoTracking()
            .AnyAsync(attribute =>
                !attribute.IsDeleted &&
                attribute.Code == code &&
                attribute.DataType == dataType &&
                (excludedId == null || attribute.Id != excludedId.Value));
    }

    private static IQueryable<AttributeDefinition> IncludeActiveOptions(IQueryable<AttributeDefinition> query)
    {
        return query.Include(attribute => attribute.Options.Where(option => !option.IsDeleted));
    }
}
