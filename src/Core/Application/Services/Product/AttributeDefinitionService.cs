namespace Application.Services;

public class AttributeDefinitionService
{
    private readonly IUnitOfWork _unitOfWork;

    public AttributeDefinitionService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<AttributeDefinitionDto>> GetAllAsync()
    {
        var attributes = await _unitOfWork.AttributeDefinitions.GetActiveWithOptionsAsync();
        return attributes.Select(ToDto);
    }

    public async Task<AttributeDefinitionDto> GetByIdAsync(int id)
    {
        var attribute = await _unitOfWork.AttributeDefinitions.GetActiveWithOptionsAsync(id);
        return attribute is null ? throw new KeyNotFoundException($"Attribute with ID {id} not found.") : ToDto(attribute);
    }

    public async Task<AttributeDefinitionDto> CreateAsync(UpsertAttributeDefinitionDto request)
    {
        var attribute = ToEntity(request);
        Normalize(attribute);

        var existingAttribute = await _unitOfWork.AttributeDefinitions
            .GetActiveByCodeAndDataTypeAsync(attribute.Code, attribute.DataType);

        if (existingAttribute is not null)
        {
            await AssignToCategoryIfRequestedAsync(request, existingAttribute.Id);
            return ToDto(existingAttribute);
        }

        await _unitOfWork.AttributeDefinitions.AddAsync(attribute);
        await _unitOfWork.SaveChangesAsync();

        await AssignToCategoryIfRequestedAsync(request, attribute.Id);
        return ToDto(attribute);
    }

    public async Task<AttributeDefinitionDto> UpdateAsync(int id, UpsertAttributeDefinitionDto request)
    {
        var attribute = await _unitOfWork.AttributeDefinitions.GetActiveWithOptionsAsync(id, trackChanges: true);
        if (attribute is null) throw new KeyNotFoundException($"Attribute with ID {id} not found.");

        attribute.Code = request.Code;
        attribute.Name = request.Name;
        attribute.DataType = request.DataType;
        attribute.Unit = request.Unit;
        attribute.IsFilterable = request.IsFilterable;
        attribute.IsSearchable = request.IsSearchable;
        attribute.IsComparable = request.IsComparable;
        attribute.IsRequired = request.IsRequired;
        attribute.IsVariantAttribute = request.IsVariantAttribute;
        attribute.SortOrder = request.SortOrder;
        attribute.ValidationRegex = request.ValidationRegex;
        attribute.MinValue = request.MinValue;
        attribute.MaxValue = request.MaxValue;

        attribute.Options.Clear();
        foreach (var option in request.Options ?? [])
        {
            attribute.Options.Add(new AttributeOption
            {
                Value = option.Value,
                Label = option.Label,
                SortOrder = option.SortOrder,
                IsActive = option.IsActive
            });
        }

        Normalize(attribute);
        if (await _unitOfWork.AttributeDefinitions.ActiveCodeAndDataTypeExistsAsync(attribute.Code, attribute.DataType, id))
        {
            throw new ArgumentException($"Attribute '{attribute.Name}' with data type '{attribute.DataType}' already exists.");
        }

        await _unitOfWork.SaveChangesAsync();
        return ToDto(attribute);
    }

    public async Task DeleteAsync(int id)
    {
        var attribute = await _unitOfWork.AttributeDefinitions.GetActiveWithOptionsAsync(id, trackChanges: true);
        if (attribute is null) throw new KeyNotFoundException($"Attribute with ID {id} not found.");
        attribute.IsDeleted = true;
        await _unitOfWork.SaveChangesAsync();
    }

    private async Task AssignToCategoryIfRequestedAsync(UpsertAttributeDefinitionDto request, int attributeDefinitionId)
    {
        if (request.CategoryId is null) return;

        var exists = await _unitOfWork.CategoryAttributeDefinitions
            .ActiveAssignmentExistsAsync(request.CategoryId.Value, attributeDefinitionId);
        if (exists) return;

        await _unitOfWork.CategoryAttributeDefinitions.AddAsync(new CategoryAttributeDefinition
        {
            CategoryId = request.CategoryId.Value,
            AttributeDefinitionId = attributeDefinitionId,
            IsRequired = request.IsRequired,
            SortOrder = request.SortOrder,
            IsFilterable = request.IsFilterable
        });
        await _unitOfWork.SaveChangesAsync();
    }

    private static AttributeDefinition ToEntity(UpsertAttributeDefinitionDto request) => new()
    {
        Code = request.Code,
        Name = request.Name,
        DataType = request.DataType,
        Unit = request.Unit,
        IsFilterable = request.IsFilterable,
        IsSearchable = request.IsSearchable,
        IsComparable = request.IsComparable,
        IsRequired = request.IsRequired,
        IsVariantAttribute = request.IsVariantAttribute,
        SortOrder = request.SortOrder,
        ValidationRegex = request.ValidationRegex,
        MinValue = request.MinValue,
        MaxValue = request.MaxValue,
        Options = request.Options.Select(o => new AttributeOption
        {
            Value = o.Value,
            Label = o.Label,
            SortOrder = o.SortOrder,
            IsActive = o.IsActive
        }).ToList()
    };

    private static AttributeDefinitionDto ToDto(AttributeDefinition attribute) => new()
    {
        Id = attribute.Id,
        Code = attribute.Code,
        Name = attribute.Name,
        DataType = attribute.DataType,
        Unit = attribute.Unit,
        IsFilterable = attribute.IsFilterable,
        IsSearchable = attribute.IsSearchable,
        IsComparable = attribute.IsComparable,
        IsRequired = attribute.IsRequired,
        IsVariantAttribute = attribute.IsVariantAttribute,
        SortOrder = attribute.SortOrder,
        ValidationRegex = attribute.ValidationRegex,
        MinValue = attribute.MinValue,
        MaxValue = attribute.MaxValue,
        Options = attribute.Options?.Where(o => !o.IsDeleted).Select(o => new AttributeOptionDto
        {
            Id = o.Id,
            Value = o.Value,
            Label = o.Label,
            SortOrder = o.SortOrder,
            IsActive = o.IsActive
        }).ToList() ?? []
    };

    private static void Normalize(AttributeDefinition attribute)
    {
        attribute.Code = attribute.Code?.Trim().ToLowerInvariant() ?? string.Empty;
        attribute.Name = attribute.Name?.Trim() ?? string.Empty;
        foreach (var option in attribute.Options ?? [])
        {
            option.Value = option.Value?.Trim() ?? string.Empty;
            option.Label = string.IsNullOrWhiteSpace(option.Label) ? option.Value : option.Label.Trim();
        }
    }
}
