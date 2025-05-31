using Application.Helper;

namespace Application.Services;
public class SupplierService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;
    private readonly IPasswordHasher _passwordHasher;

    public SupplierService(
        IUnitOfWork unitOfWork,
        IMapper mapper, IPasswordHasher passwordHasher,
        ICurrentUserService currentUserService)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
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
            User user = null;
            var userId = _currentUserService.UserId;
            if (userId != null)
            {
                user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null)
                    throw new Exception("User not found");
            }
            else
            {
                var existingUser = (await _unitOfWork.Users.GetByPhonenumberAsync(supplierDto.PhoneNumber));
                if (existingUser != null)
                    throw new Exception("User with this phone number or email already exists.");

                var password = GenerateRandomPassword();
                Console.Clear();
                Console.WriteLine($"password ==> {password}");
                var passwordHash = _passwordHasher.HashPassword(password);
                user = new User
                {
                    Id = Guid.NewGuid().ToString(),
                    PhoneNumber = supplierDto.PhoneNumber,
                    PasswordHash = passwordHash,
                    IsActive = true,
                    IsEmailConfirmed = false,
                    TwoFactorEnabled = false,
                    Username = supplierDto.PhoneNumber,
                };
            }

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