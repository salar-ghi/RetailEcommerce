using AutoMapper.Configuration.Annotations;
using System.Collections.Immutable;

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
        var suppliers = await _unitOfWork.Suppliers.GetAllAsync(z => !z.IsDeleted);
        return _mapper.Map<IEnumerable<SupplierDto>>(suppliers);
    
    }

    public async Task<SupplierDto> ApproveSupplierAsync(ApproveSupplierDto request)
    {
        var supplier = await _unitOfWork.Suppliers.GetByIdAsync(request.Id);
        if (supplier == null || supplier.IsDeleted) throw new KeyNotFoundException($"Supplier with ID {request.Id} not found.");

        supplier.Status = request.IsApproved ? SupplierStatus.Approved : SupplierStatus.Rejected;
        supplier.ApprovalDate = DateTime.Now;
        supplier.ModifiedTime = DateTime.Now;
        supplier.ModifiedBy = _currentUserService.UserId;
        supplier.ApprovedByUserId = request.ApprovedByUserId;
        await _unitOfWork.Suppliers.UpdateAsync(supplier);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<SupplierDto>(supplier);
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

                var password = _currentUserService.GenerateRandomPassword();
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
                await _unitOfWork.Users.AddAsync(user);

                // Assign Customer role
                var customerRole = (await _unitOfWork.Roles.GetSingleAsync(r => r.Name == "Customer"));
                if (customerRole == null)
                {
                    customerRole = new Role { Name = "Customer" };
                    await _unitOfWork.Roles.AddAsync(customerRole);
                    await _unitOfWork.SaveChangesAsync();
                }
                await _unitOfWork.UserRoles.AddAsync(new UserRole { UserId = user.Id, RoleId = customerRole.Id });
            }
            var exisitingSupplier = await _unitOfWork.Suppliers.GetSingleAsync(s => s.UserId == user.Id && !s.IsDeleted);
            if (exisitingSupplier != null)
                throw new Exception("USer is already registered as a supplier");

            var supplier = _mapper.Map<Supplier>(supplierDto);
            supplier.CreatedTime = DateTime.Now;
            supplier.ModifiedTime = DateTime.Now;
            supplier.UserId = userId;
            supplier.CreatedBy = userId;
            supplier.ModifiedBy = userId;

            await _unitOfWork.Suppliers.AddAsync(supplier);

            var supplierRole = (await _unitOfWork.Roles.GetSingleAsync(r => r.Name == "Supplier"));
            if (supplierRole == null)
            {
                supplierRole = new Role { Name = "Supplier" };
                await _unitOfWork.Roles.AddAsync(supplierRole);
                await _unitOfWork.SaveChangesAsync();
            }
            await _unitOfWork.UserRoles.AddAsync(new UserRole { UserId = user.Id, RoleId = supplierRole.Id });

            await _unitOfWork.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task UpdateSupplierAsync(UpdateSupplierStatusDto supplierDto)
    {
        var supplier = await _unitOfWork.Suppliers.GetByIdAsync(supplierDto.Id);
        if (supplier == null) throw new KeyNotFoundException($"Supplier with ID {supplierDto.Id} not found.");
        _mapper.Map(supplierDto, supplier);
        supplier.Status = supplierDto.Status;
        supplier.ModifiedTime = DateTime.UtcNow;

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