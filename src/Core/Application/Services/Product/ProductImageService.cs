namespace Application.Services;

public class ProductImageService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ProductImageService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ProductImageDto>> GetAllImagesAsync()
    {
        var images = await _unitOfWork.ProductImages.GetAllAsync();
        return _mapper.Map<IEnumerable<ProductImageDto>>(images);
    }

    public async Task<ProductImageDto> GetImageByIdAsync(int id)
    {
        var image = await _unitOfWork.ProductImages.GetByIdAsync(id);
        if (image == null) throw new KeyNotFoundException($"Image with ID {id} not found.");
        return _mapper.Map<ProductImageDto>(image);
    }

    public async Task AddImageAsync(ProductImageDto imageDto)
    {
        var image = _mapper.Map<ProductImage>(imageDto);
        await _unitOfWork.ProductImages.AddAsync(image);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task UpdateImageAsync(ProductImageDto imageDto)
    {
        var image = await _unitOfWork.ProductImages.GetByIdAsync(imageDto.Id);
        if (image == null) throw new KeyNotFoundException($"Image with ID {imageDto.Id} not found.");
        _mapper.Map(imageDto, image);
        await _unitOfWork.ProductImages.UpdateAsync(image);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteImageAsync(int id)
    {
        var image = await _unitOfWork.ProductImages.GetByIdAsync(id);
        if (image != null)
        {
            await _unitOfWork.ProductImages.DeleteAsync(image);
            await _unitOfWork.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<ProductImageDto>> SearchImagesByProductIdAsync(int productId)
    {
        var images = await _unitOfWork.ProductImages.GetByProductIdAsync(productId);
        return _mapper.Map<IEnumerable<ProductImageDto>>(images);
    }

    public async Task<IEnumerable<ProductImageDto>> SearchImagesByPrimaryAsync(bool isPrimary)
    {
        var images = await _unitOfWork.ProductImages.SearchByPrimaryAsync(isPrimary);
        return _mapper.Map<IEnumerable<ProductImageDto>>(images);
    }
}