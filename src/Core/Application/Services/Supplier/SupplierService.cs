namespace Application.Services;
public class SupplierService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public SupplierService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
    }

    public async Task<IEnumerable<SupplierDto>> GetAllSuppliersAsync()
    {
        var suppliers = await _unitOfWork.Suppliers.GetAllAsync();
        return _mapper.Map<IEnumerable<SupplierDto>>(suppliers);
    }

    public async Task<SupplierDto> GetSupplierByIdAsync(int id)
    {
        var supplier = await _unitOfWork.Suppliers.GetByIdAsync(id);
        if (supplier == null) throw new KeyNotFoundException($"Supplier with ID {id} not found.");
        return _mapper.Map<SupplierDto>(supplier);
    }

    public async Task AddSupplierAsync(SupplierRegistrationDto supplierDto)
    {
        try
        {
            var userId = _currentUserService.UserId;

            var supplier = _mapper.Map<Supplier>(supplierDto);
            supplier.CreatedTime = DateTime.Now;
            supplier.ModifiedTime = DateTime.Now;
            supplier.UserId = userId;
            supplier.CreatedBy = userId;
            supplier.ModifiedBy = userId;

            await _unitOfWork.Suppliers.AddAsync(supplier);
            await _unitOfWork.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task UpdateSupplierAsync(SupplierDto supplierDto)
    {
        var supplier = await _unitOfWork.Suppliers.GetByIdAsync(supplierDto.Id);
        if (supplier == null) throw new KeyNotFoundException($"Supplier with ID {supplierDto.Id} not found.");
        _mapper.Map(supplierDto, supplier);
        await _unitOfWork.Suppliers.UpdateAsync(supplier);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteSupplierAsync(int id)
    {
        var supplier = await _unitOfWork.Suppliers.GetByIdAsync(id);
        if (supplier != null)
        {
            await _unitOfWork.Suppliers.DeleteAsync(supplier);
            await _unitOfWork.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<SupplierDto>> SearchSuppliersByNameAsync(string name)
    {
        var suppliers = await _unitOfWork.Suppliers.SearchByNameAsync(name);
        return _mapper.Map<IEnumerable<SupplierDto>>(suppliers);
    }

    public async Task<IEnumerable<SupplierDto>> SearchSuppliersByContactInfoAsync(string contactInfo)
    {
        var suppliers = await _unitOfWork.Suppliers.SearchByContactInfoAsync(contactInfo);
        return _mapper.Map<IEnumerable<SupplierDto>>(suppliers);
    }
}