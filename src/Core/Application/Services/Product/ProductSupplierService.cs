namespace Application.Services;
public class ProductSupplierService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ProductSupplierService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ProductSupplierDto>> GetAllProductSuppliersAsync()
    {
        var productSuppliers = await _unitOfWork.ProductSuppliers.GetAllAsync();
        return _mapper.Map<IEnumerable<ProductSupplierDto>>(productSuppliers);
    }

    public async Task<ProductSupplierDto> GetProductSupplierByIdAsync(int productId, int supplierId)
    {
        var productSupplier = await _unitOfWork.ProductSuppliers.GetAllAsync();
        var result = productSupplier.FirstOrDefault(ps => ps.ProductId == productId && ps.SupplierId == supplierId);
        if (result == null) throw new KeyNotFoundException($"ProductSupplier with Product ID {productId} and Supplier ID {supplierId} not found.");
        return _mapper.Map<ProductSupplierDto>(result);
    }

    public async Task AddProductSupplierAsync(ProductSupplierDto productSupplierDto)
    {
        var productSupplier = _mapper.Map<ProductSupplier>(productSupplierDto);
        await _unitOfWork.ProductSuppliers.AddAsync(productSupplier);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task UpdateProductSupplierAsync(ProductSupplierDto productSupplierDto)
    {
        var productSupplier = await _unitOfWork.ProductSuppliers.GetAllAsync();
        var existing = productSupplier.FirstOrDefault(ps => ps.ProductId == productSupplierDto.ProductId && ps.SupplierId == productSupplierDto.SupplierId);
        if (existing == null) throw new KeyNotFoundException($"ProductSupplier with Product ID {productSupplierDto.ProductId} and Supplier ID {productSupplierDto.SupplierId} not found.");
        _mapper.Map(productSupplierDto, existing);
        await _unitOfWork.ProductSuppliers.UpdateAsync(existing);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteProductSupplierAsync(int productId, int supplierId)
    {
        await _unitOfWork.ProductSuppliers.DeleteAsync(productId, supplierId);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<IEnumerable<ProductSupplierDto>> SearchProductSuppliersByProductIdAsync(int productId)
    {
        var productSuppliers = await _unitOfWork.ProductSuppliers.GetByProductIdAsync(productId);
        return _mapper.Map<IEnumerable<ProductSupplierDto>>(productSuppliers);
    }

    public async Task<IEnumerable<ProductSupplierDto>> SearchProductSuppliersBySupplierIdAsync(int supplierId)
    {
        var productSuppliers = await _unitOfWork.ProductSuppliers.GetBySupplierIdAsync(supplierId);
        return _mapper.Map<IEnumerable<ProductSupplierDto>>(productSuppliers);
    }
}