namespace Application.Services;

public class ProductTagService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ProductTagService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ProductTagDto>> GetAllProductTagsAsync()
    {
        var productTags = await _unitOfWork.ProductTags.GetAllAsync();
        return _mapper.Map<IEnumerable<ProductTagDto>>(productTags);
    }

    public async Task<ProductTagDto> GetProductTagByIdAsync(int productId, int tagId)
    {
        var productTag = await _unitOfWork.ProductTags.GetAllAsync();
        var result = productTag.FirstOrDefault(pt => pt.ProductId == productId && pt.TagId == tagId);
        if (result == null) throw new KeyNotFoundException($"ProductTag with Product ID {productId} and Tag ID {tagId} not found.");
        return _mapper.Map<ProductTagDto>(result);
    }

    public async Task AddProductTagAsync(ProductTagDto productTagDto)
    {
        var productTag = _mapper.Map<ProductTag>(productTagDto);
        await _unitOfWork.ProductTags.AddAsync(productTag);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task UpdateProductTagAsync(ProductTagDto productTagDto)
    {
        var productTag = await _unitOfWork.ProductTags.GetAllAsync();
        var existing = productTag.FirstOrDefault(pt => pt.ProductId == productTagDto.ProductId && pt.TagId == productTagDto.TagId);
        if (existing == null) throw new KeyNotFoundException($"ProductTag with Product ID {productTagDto.ProductId} and Tag ID {productTagDto.TagId} not found.");
        _mapper.Map(productTagDto, existing);
        await _unitOfWork.ProductTags.UpdateAsync(existing);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteProductTagAsync(int productId, int tagId)
    {
        await _unitOfWork.ProductTags.DeleteAsync(productId, tagId);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<IEnumerable<ProductTagDto>> SearchProductTagsByProductIdAsync(int productId)
    {
        var productTags = await _unitOfWork.ProductTags.GetByProductIdAsync(productId);
        return _mapper.Map<IEnumerable<ProductTagDto>>(productTags);
    }

    public async Task<IEnumerable<ProductTagDto>> SearchProductTagsByTagIdAsync(int tagId)
    {
        var productTags = await _unitOfWork.ProductTags.GetByTagIdAsync(tagId);
        return _mapper.Map<IEnumerable<ProductTagDto>>(productTags);
    }
}
