namespace Application.Services;

public class ProductAttributeService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ProductAttributeService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ProductAttributeDto>> GetAllAttributesAsync()
    {
        var attributes = await _unitOfWork.ProductAttributes.GetAllAsync();
        return _mapper.Map<IEnumerable<ProductAttributeDto>>(attributes);
    }

    public async Task<ProductAttributeDto> GetAttributeByIdAsync(int id)
    {
        var attribute = await _unitOfWork.ProductAttributes.GetByIdAsync(id);
        return attribute != null ? _mapper.Map<ProductAttributeDto>(attribute) : throw new KeyNotFoundException($"Attribute with ID {id} not found.");
    }

    public async Task AddAttributeAsync(ProductAttributeDto attributeDto)
    {
        var attribute = _mapper.Map<ProductAttribute>(attributeDto);
        await _unitOfWork.ProductAttributes.AddAsync(attribute);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task UpdateAttributeAsync(ProductAttributeDto attributeDto)
    {
        var attribute = await _unitOfWork.ProductAttributes.GetByIdAsync(attributeDto.Id);
        if (attribute != null)
        {
            _mapper.Map(attributeDto, attribute);
            await _unitOfWork.ProductAttributes.UpdateAsync(attribute);
            await _unitOfWork.SaveChangesAsync();
        }
        else
        {
            throw new KeyNotFoundException($"Attribute with ID {attributeDto.Id} not found.");
        }
    }

    public async Task DeleteAttributeAsync(int id)
    {
        var attribute = await _unitOfWork.ProductAttributes.GetByIdAsync(id);
        if (attribute != null)
        {
            await _unitOfWork.ProductAttributes.DeleteAsync(attribute);
            await _unitOfWork.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<ProductAttributeDto>> SearchAttributesByProductIdAsync(int productId)
    {
        var attributes = await _unitOfWork.ProductAttributes.GetByProductIdAsync(productId);
        return _mapper.Map<IEnumerable<ProductAttributeDto>>(attributes);
    }

    public async Task<IEnumerable<ProductAttributeDto>> SearchAttributesByKeyAsync(string key)
    {
        var attributes = await _unitOfWork.ProductAttributes.SearchByKeyAsync(key);
        return _mapper.Map<IEnumerable<ProductAttributeDto>>(attributes);
    }
}