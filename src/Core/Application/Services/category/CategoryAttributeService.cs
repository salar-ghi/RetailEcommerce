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

    public async Task<IEnumerable<CategoryAttributeDto>> GetAllAttributesAsync()
    {
        var attributes = await _unitOfWork.CategoryAttributes.GetAllAsync();
        return _mapper.Map<IEnumerable<CategoryAttributeDto>>(attributes);
    }

    public async Task<CategoryAttributeDto> GetAttributeByIdAsync(int id)
    {
        var attribute = await _unitOfWork.CategoryAttributes.GetByIdAsync(id);
        if (attribute == null) throw new KeyNotFoundException($"Attribute with ID {id} not found.");
        return _mapper.Map<CategoryAttributeDto>(attribute);
    }

    public async Task AddAttributeAsync(CategoryAttributeDto attributeDto)
    {
        var attribute = _mapper.Map<CategoryAttribute>(attributeDto);
        await _unitOfWork.CategoryAttributes.AddAsync(attribute);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task UpdateAttributeAsync(CategoryAttributeDto attributeDto)
    {
        var attribute = await _unitOfWork.CategoryAttributes.GetByIdAsync(attributeDto.Id);
        if (attribute == null) throw new KeyNotFoundException($"Attribute with ID {attributeDto.Id} not found.");
        _mapper.Map(attributeDto, attribute);
        await _unitOfWork.CategoryAttributes.UpdateAsync(attribute);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAttributeAsync(int id)
    {
        var attribute = await _unitOfWork.CategoryAttributes.GetByIdAsync(id);
        if (attribute is not null)
        {
            await _unitOfWork.CategoryAttributes.DeleteAsync(attribute);
            await _unitOfWork.SaveChangesAsync();
        }
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
}