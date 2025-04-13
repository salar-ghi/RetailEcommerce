namespace Application.Services;
public class ProductUnitPriceService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ProductUnitPriceService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ProductUnitPriceDto>> GetAllUnitPricesAsync()
    {
        var unitPrices = await _unitOfWork.ProductUnitPrices.GetAllAsync();
        return _mapper.Map<IEnumerable<ProductUnitPriceDto>>(unitPrices);
    }

    public async Task<ProductUnitPriceDto> GetUnitPriceByIdAsync(int id)
    {
        var unitPrice = await _unitOfWork.ProductUnitPrices.GetByIdAsync(id);
        if (unitPrice == null) throw new KeyNotFoundException($"UnitPrice with ID {id} not found.");
        return _mapper.Map<ProductUnitPriceDto>(unitPrice);
    }

    public async Task AddUnitPriceAsync(ProductUnitPriceDto unitPriceDto)
    {
        var unitPrice = _mapper.Map<ProductUnitPrice>(unitPriceDto);
        await _unitOfWork.ProductUnitPrices.AddAsync(unitPrice);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task UpdateUnitPriceAsync(ProductUnitPriceDto unitPriceDto)
    {
        var unitPrice = await _unitOfWork.ProductUnitPrices.GetByIdAsync(unitPriceDto.Id);
        if (unitPrice == null) throw new KeyNotFoundException($"UnitPrice with ID {unitPriceDto.Id} not found.");
        _mapper.Map(unitPriceDto, unitPrice);
        await _unitOfWork.ProductUnitPrices.UpdateAsync(unitPrice);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteUnitPriceAsync(int id)
    {
        var unitPrice = await _unitOfWork.ProductUnitPrices.GetByIdAsync(id);
        if (unitPrice != null)
        {
            await _unitOfWork.ProductUnitPrices.DeleteAsync(unitPrice);
            await _unitOfWork.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<ProductUnitPriceDto>> SearchUnitPricesByProductIdAsync(int productId)
    {
        var unitPrices = await _unitOfWork.ProductUnitPrices.GetByProductIdAsync(productId);
        return _mapper.Map<IEnumerable<ProductUnitPriceDto>>(unitPrices);
    }

    public async Task<IEnumerable<ProductUnitPriceDto>> SearchUnitPricesByPriceRangeAsync(decimal minPrice, decimal maxPrice)
    {
        var unitPrices = await _unitOfWork.ProductUnitPrices.SearchByPriceRangeAsync(minPrice, maxPrice);
        return _mapper.Map<IEnumerable<ProductUnitPriceDto>>(unitPrices);
    }
}