namespace Application.Services;

public class ProductDimensionsService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ProductDimensionsService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ProductDimensionsDto> GetDimensionsByIdAsync(int id)
    {
        var dimensions = await _unitOfWork.ProductDimensions.GetByIdAsync(id);
        return dimensions != null ? _mapper.Map<ProductDimensionsDto>(dimensions) : throw new KeyNotFoundException($"Dimensions with ID {id} not found.");
    }

    public async Task AddDimensionsAsync(ProductDimensionsDto dimensionsDto)
    {
        var dimensions = _mapper.Map<ProductDimensions>(dimensionsDto);
        await _unitOfWork.ProductDimensions.AddAsync(dimensions);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task UpdateDimensionsAsync(ProductDimensionsDto dimensionsDto)
    {
        var dimensions = await _unitOfWork.ProductDimensions.GetByIdAsync(dimensionsDto.Id);
        if (dimensions != null)
        {
            _mapper.Map(dimensionsDto, dimensions);
            await _unitOfWork.ProductDimensions.UpdateAsync(dimensions);
            await _unitOfWork.SaveChangesAsync();
        }
        else
        {
            throw new KeyNotFoundException($"Dimensions with ID {dimensionsDto.Id} not found.");
        }
    }

    public async Task DeleteDimensionsAsync(int id)
    {
        var dimensions = await _unitOfWork.ProductDimensions.GetByIdAsync(id);
        if (dimensions != null)
        {
            await _unitOfWork.ProductDimensions.DeleteAsync(dimensions);
            await _unitOfWork.SaveChangesAsync();
        }
    }

    public async Task<ProductDimensionsDto> SearchDimensionsByProductIdAsync(int productId)
    {
        var dimensions = await _unitOfWork.ProductDimensions.GetByProductIdAsync(productId);
        return dimensions != null ? _mapper.Map<ProductDimensionsDto>(dimensions) : throw new KeyNotFoundException($"Dimensions for Product ID {productId} not found.");
    }
}