namespace Application.Services;

public class ProductVariantService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ProductVariantService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ProductVariantDto>> GetAllVariantsAsync()
    {
        var variants = await _unitOfWork.ProductVariants.GetAllAsync();
        return _mapper.Map<IEnumerable<ProductVariantDto>>(variants);
    }

    public async Task<ProductVariantDto> GetVariantByIdAsync(int id)
    {
        var variant = await _unitOfWork.ProductVariants.GetByIdAsync(id);
        return variant != null ? _mapper.Map<ProductVariantDto>(variant) : throw new KeyNotFoundException($"Variant with ID {id} not found.");
    }

    public async Task AddVariantAsync(ProductVariantDto variantDto)
    {
        var variant = _mapper.Map<ProductVariant>(variantDto);
        await _unitOfWork.ProductVariants.AddAsync(variant);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task UpdateVariantAsync(ProductVariantDto variantDto)
    {
        var variant = await _unitOfWork.ProductVariants.GetByIdAsync(variantDto.Id);
        if (variant != null)
        {
            _mapper.Map(variantDto, variant);
            await _unitOfWork.ProductVariants.UpdateAsync(variant);
            await _unitOfWork.SaveChangesAsync();
        }
        else
        {
            throw new KeyNotFoundException($"Variant with ID {variantDto.Id} not found.");
        }
    }

    public async Task DeleteVariantAsync(int id)
    {
        var variant = await _unitOfWork.ProductVariants.GetByIdAsync(id);
        if (variant != null)
        {
            await _unitOfWork.ProductVariants.DeleteAsync(variant);
            await _unitOfWork.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<ProductVariantDto>> SearchVariantsByProductIdAsync(int productId)
    {
        var variants = await _unitOfWork.ProductVariants.GetByProductIdAsync(productId);
        return _mapper.Map<IEnumerable<ProductVariantDto>>(variants);
    }

    public async Task<IEnumerable<ProductVariantDto>> SearchVariantsByNameAsync(string variantName)
    {
        var variants = await _unitOfWork.ProductVariants.SearchByVariantNameAsync(variantName);
        return _mapper.Map<IEnumerable<ProductVariantDto>>(variants);
    }
}