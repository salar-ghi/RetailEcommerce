namespace Application.Services;

public class ProductStockService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ProductStockService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ProductStockDto> GetStockByIdAsync(int id)
    {
        var stock = await _unitOfWork.ProductStocks.GetByIdAsync(id);
        return stock != null ? _mapper.Map<ProductStockDto>(stock) : throw new KeyNotFoundException($"Stock with ID {id} not found.");
    }

    public async Task AddStockAsync(ProductStockDto stockDto)
    {
        var stock = _mapper.Map<ProductStock>(stockDto);
        await _unitOfWork.ProductStocks.AddAsync(stock);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task UpdateStockAsync(ProductStockDto stockDto)
    {
        var stock = await _unitOfWork.ProductStocks.GetByIdAsync(stockDto.Id);
        if (stock != null)
        {
            _mapper.Map(stockDto, stock);
            await _unitOfWork.ProductStocks.UpdateAsync(stock);
            await _unitOfWork.SaveChangesAsync();
        }
        else
        {
            throw new KeyNotFoundException($"Stock with ID {stockDto.Id} not found.");
        }
    }

    public async Task DeleteStockAsync(int id)
    {
        var stock = await _unitOfWork.ProductStocks.GetByIdAsync(id);
        if (stock != null)
        {
            await _unitOfWork.ProductStocks.DeleteAsync(stock);
            await _unitOfWork.SaveChangesAsync();
        }
    }

    public async Task<ProductStockDto> SearchStockByProductIdAsync(int productId)
    {
        var stock = await _unitOfWork.ProductStocks.GetByProductIdAsync(productId);
        return stock != null ? _mapper.Map<ProductStockDto>(stock) : throw new KeyNotFoundException($"Stock for Product ID {productId} not found.");
    }

    public async Task<IEnumerable<ProductStockDto>> SearchLowStockAsync(int threshold)
    {
        var stocks = await _unitOfWork.ProductStocks.SearchByLowStockAsync(threshold);
        return _mapper.Map<IEnumerable<ProductStockDto>>(stocks);
    }
}