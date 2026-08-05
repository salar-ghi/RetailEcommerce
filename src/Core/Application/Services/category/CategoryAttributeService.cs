namespace Application.Services;

public class CategoryAttributeService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CategoryAttributeService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<CategoryAttributeDefinitionDto>> GetCategoryAttributeDefinitionsAsync(int categoryId)
    {
        var attributes = await _unitOfWork.CategoryAttributeDefinitions.GetActiveByCategoryIdAsync(categoryId);
        return attributes.Select(ToCategoryAttributeDefinitionDto);
    }

    public async Task<CategoryAttributeDefinitionDto> AssignCategoryAttributeDefinitionAsync(int categoryId, AssignCategoryAttributeDefinitionDto request)
    {
        var attributeExists = await _unitOfWork.CategoryAttributeDefinitions.ActiveAttributeDefinitionExistsAsync(request.AttributeDefinitionId);
        if (!attributeExists) throw new ArgumentException("Attribute definition does not exist.");

        var exists = await _unitOfWork.CategoryAttributeDefinitions.ActiveAssignmentExistsAsync(categoryId, request.AttributeDefinitionId);
        if (exists) throw new InvalidOperationException("Attribute is already assigned to this category.");

        var categoryAttribute = new CategoryAttributeDefinition
        {
            CategoryId = categoryId,
            AttributeDefinitionId = request.AttributeDefinitionId,
            IsRequired = request.IsRequired,
            SortOrder = request.SortOrder,
            IsVisibleOnProductPage = request.IsVisibleOnProductPage,
            IsFilterable = request.IsFilterable
        };

        await _unitOfWork.CategoryAttributeDefinitions.AddAsync(categoryAttribute);
        await _unitOfWork.SaveChangesAsync();

        var created = await _unitOfWork.CategoryAttributeDefinitions.GetActiveWithAttributeDefinitionAsync(categoryAttribute.Id);
        return ToCategoryAttributeDefinitionDto(created ?? categoryAttribute);
    }

    public async Task<CategoryAttributeDefinitionDto> UpdateCategoryAttributeDefinitionAsync(int categoryId, int id, UpdateCategoryAttributeDefinitionDto request)
    {
        var row = await _unitOfWork.CategoryAttributeDefinitions.GetActiveWithAttributeDefinitionAsync(categoryId, id, trackChanges: true);
        if (row is null) throw new KeyNotFoundException($"Category attribute with ID {id} not found.");

        row.IsRequired = request.IsRequired;
        row.SortOrder = request.SortOrder;
        row.IsVisibleOnProductPage = request.IsVisibleOnProductPage;
        row.IsFilterable = request.IsFilterable;
        await _unitOfWork.SaveChangesAsync();

        return ToCategoryAttributeDefinitionDto(row);
    }

    public async Task RemoveCategoryAttributeDefinitionAsync(int categoryId, int id)
    {
        var row = await _unitOfWork.CategoryAttributeDefinitions.GetActiveByIdAsync(categoryId, id, trackChanges: true);
        if (row is null) throw new KeyNotFoundException($"Category attribute with ID {id} not found.");

        row.IsDeleted = true;
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<IEnumerable<CategoryAttributeDto>> GetAllAttributesAsync()
    {
        var attributes = await _unitOfWork.CategoryAttributes.GetAllAsync(a => !a.IsDeleted);
        return _mapper.Map<IEnumerable<CategoryAttributeDto>>(attributes);
    }

    public async Task<CategoryAttributeDto> GetAttributeByIdAsync(int id)
    {
        var attribute = await _unitOfWork.CategoryAttributes.GetSingleAsync(a => a.Id == id && !a.IsDeleted);
        if (attribute == null) throw new KeyNotFoundException($"Attribute with ID {id} not found.");
        return _mapper.Map<CategoryAttributeDto>(attribute);
    }

    public async Task<CategoryAttributeDto> AddAttributeAsync(CategoryAttributeDto attributeDto)
    {
        var attribute = _mapper.Map<CategoryAttribute>(attributeDto);
        await _unitOfWork.CategoryAttributes.AddAsync(attribute);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<CategoryAttributeDto>(attribute);
    }

    public async Task UpdateAttributeAsync(CategoryAttributeDto attributeDto)
    {
        var attribute = await _unitOfWork.CategoryAttributes.GetSingleAsync(a => a.Id == attributeDto.Id && !a.IsDeleted);
        if (attribute == null) throw new KeyNotFoundException($"Attribute with ID {attributeDto.Id} not found.");
        _mapper.Map(attributeDto, attribute);
        await _unitOfWork.CategoryAttributes.UpdateAsync(attribute);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAttributeAsync(int id)
    {
        var attribute = await _unitOfWork.CategoryAttributes.GetSingleAsync(a => a.Id == id && !a.IsDeleted);
        if (attribute is null) throw new KeyNotFoundException($"Attribute with ID {id} not found.");

        attribute.IsDeleted = true;
        await _unitOfWork.CategoryAttributes.UpdateAsync(attribute);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<IEnumerable<CategoryAttributeDto>> SearchAttributesByCategoryIdAsync(int categoryId)
    {
        var attributes = await _unitOfWork.CategoryAttributes.GetByCategoryIdAsync(categoryId);
        return _mapper.Map<IEnumerable<CategoryAttributeDto>>(attributes);
    }

    public async Task<IEnumerable<CategoryAttributeDto>> SearchAttributesByKeyAsync(string key)
    {
        var attributes = await _unitOfWork.CategoryAttributes.SearchByKeyAsync(key);
        return _mapper.Map<IEnumerable<CategoryAttributeDto>>(attributes);
    }

    private static CategoryAttributeDefinitionDto ToCategoryAttributeDefinitionDto(CategoryAttributeDefinition categoryAttribute) => new()
    {
        Id = categoryAttribute.Id,
        CategoryId = categoryAttribute.CategoryId,
        AttributeDefinitionId = categoryAttribute.AttributeDefinitionId,
        IsRequired = categoryAttribute.IsRequired,
        SortOrder = categoryAttribute.SortOrder,
        IsVisibleOnProductPage = categoryAttribute.IsVisibleOnProductPage,
        IsFilterable = categoryAttribute.IsFilterable,
        AttributeDefinition = categoryAttribute.AttributeDefinition is null ? null : new AttributeDefinitionDto
        {
            Id = categoryAttribute.AttributeDefinition.Id,
            Code = categoryAttribute.AttributeDefinition.Code,
            Name = categoryAttribute.AttributeDefinition.Name,
            DataType = categoryAttribute.AttributeDefinition.DataType,
            Unit = categoryAttribute.AttributeDefinition.Unit,
            IsFilterable = categoryAttribute.AttributeDefinition.IsFilterable,
            IsSearchable = categoryAttribute.AttributeDefinition.IsSearchable,
            IsComparable = categoryAttribute.AttributeDefinition.IsComparable,
            IsRequired = categoryAttribute.AttributeDefinition.IsRequired,
            IsVariantAttribute = categoryAttribute.AttributeDefinition.IsVariantAttribute,
            SortOrder = categoryAttribute.AttributeDefinition.SortOrder,
            ValidationRegex = categoryAttribute.AttributeDefinition.ValidationRegex,
            MinValue = categoryAttribute.AttributeDefinition.MinValue,
            MaxValue = categoryAttribute.AttributeDefinition.MaxValue,
            Options = categoryAttribute.AttributeDefinition.Options?
                .Where(o => !o.IsDeleted)
                .OrderBy(o => o.SortOrder)
                .Select(o => new AttributeOptionDto
                {
                    Id = o.Id,
                    Value = o.Value,
                    Label = o.Label,
                    SortOrder = o.SortOrder,
                    IsActive = o.IsActive
                }).ToList() ?? new List<AttributeOptionDto>()
        }
    };
}
