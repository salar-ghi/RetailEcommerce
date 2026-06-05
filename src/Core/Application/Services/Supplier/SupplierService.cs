namespace Application.Services;

public class SupplierService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ICurrentUserService _currentUserService;

    public SupplierService(
        IUnitOfWork unitOfWork, IMapper mapper, 
        IPasswordHasher passwordHasher, ICurrentUserService currentUserService)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _currentUserService = currentUserService;
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

        //supplier.Status = request.IsApproved ? SupplierStatus.Approved : SupplierStatus.Rejected;
        var approverUserId = GetRequiredCurrentUserId();

        supplier.Status = SupplierStatus.Approved;
        supplier.ApprovedByUserId = approverUserId;
        supplier.ApprovalDate ??= DateTime.UtcNow;
        
        await _unitOfWork.Suppliers.UpdateAsync(supplier);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<SupplierDto>(supplier);
    }

    public async Task<SupplierDto> ToggleSupplierStatusAsync(int supplierId, bool isApproved)
    {
        var supplier = await _unitOfWork.Suppliers.GetByIdAsync(supplierId);
        if (supplier == null || supplier.IsDeleted) throw new KeyNotFoundException($"Supplier with ID {supplierId} not found.");

        supplier.Status = isApproved ? SupplierStatus.Approved : SupplierStatus.Inactive;
        supplier.ModifiedTime = DateTime.UtcNow;

        if (isApproved)
        {
            var approverUserId = _currentUserService.UserId;

            if (!string.IsNullOrWhiteSpace(approverUserId))
            {
                supplier.ApprovedByUserId = approverUserId;
            }

            supplier.ApprovalDate ??= DateTime.UtcNow;
        }

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

    public async Task<Result<SupplierDto>> CreateSupplierAsync(SupplierRegistrationDto supplierDto)
    {
        var existedSupplier = await _unitOfWork.Suppliers.GetSingleAsync(s => s.Info == supplierDto.ContactInfo && !s.IsDeleted);
        if (existedSupplier != null)
            return Result<SupplierDto>.Failure("تامین کننده با شماره تلفن مشابه موجود میباشد");

        //User user = null;
        var user = (await _unitOfWork.Users.GetByPhonenumberAsync(supplierDto.ContactInfo));
        if (user != null)
            goto CreateSuplier;
        else
        {
            //var password = _currentUserService.GenerateRandomPassword();
            var password = "12345678*";
            var passwordHash = _passwordHasher.HashPassword(password);
            user = new User
            {
                Id = Guid.NewGuid().ToString(),
                PhoneNumber = supplierDto.ContactInfo,
                PasswordHash = passwordHash,
                IsActive = true,
                IsEmailConfirmed = false,
                TwoFactorEnabled = false,
                Username = supplierDto.ContactInfo,
                Email = supplierDto.Email,
            };
            await _unitOfWork.Users.AddAsync(user);

            var customerRole = (await _unitOfWork.Roles.GetSingleAsync(r => r.Name == "Customer"));
            await _unitOfWork.UserRoles.AddAsync(new UserRole { UserId = user.Id, RoleId = customerRole.Id });
        }

        CreateSuplier:
        var supplier = _mapper.Map<Supplier>(supplierDto);
        NormalizeSupplierOptionalFields(supplier);
        supplier.UserId = user.Id;

        await _unitOfWork.Suppliers.AddAsync(supplier);

        var supplierRole = (await _unitOfWork.Roles.GetSingleAsync(r => r.Name == "Supplier"));
        await _unitOfWork.UserRoles.AddAsync(new UserRole { UserId = user.Id, RoleId = supplierRole.Id });
        await _unitOfWork.SaveChangesAsync();

        return Result<SupplierDto>.Success(_mapper.Map<SupplierDto>(supplier));
    }

    public async Task<Result<string>> RegisterSupplierAsync(SupplierRegistrationDto supplierDto)
    {
        var sessionId = _currentUserService.UserId;
        if (sessionId == null)
            return Result<string>.Failure("ابتدا وارد برنامه شوید");

        var user = await _unitOfWork.Users.GetByIdAsync(sessionId);
        if (user.PhoneNumber == supplierDto.ContactInfo)
            return Result<string>.Failure("شماره همراه تامین کننده با شماره همراه کاربری یکسان نمیباشد");

        var existedSupplier = await _unitOfWork.Suppliers.GetSingleAsync(s => s.Info == supplierDto.ContactInfo && !s.IsDeleted);
        if (existedSupplier != null)
            return Result<string>.Failure("تامین کننده با شماره تلفن مشابه موجود میباشد");
        
        var supplier = _mapper.Map<Supplier>(supplierDto);
        NormalizeSupplierOptionalFields(supplier);
        supplier.UserId = user.Id;
        await _unitOfWork.Suppliers.AddAsync(supplier);

        var supplierRole = (await _unitOfWork.Roles.GetSingleAsync(r => r.Name == "Supplier"));
        await _unitOfWork.UserRoles.AddAsync(new UserRole { UserId = user.Id, RoleId = supplierRole.Id });
        await _unitOfWork.SaveChangesAsync();

        return Result<string>.Success("تامین کننده مورد نظر با موفقیت ثبت گردید");
    }

    public async Task UpdateSupplierAsync(UpdateSupplierStatusDto supplierDto)
    {
        var supplier = await _unitOfWork.Suppliers.GetByIdAsync(supplierDto.Id);
        if (supplier == null) throw new KeyNotFoundException($"Supplier with ID {supplierDto.Id} not found.");
        if (supplierDto.Status.HasValue)
            supplier.Status = supplierDto.Status.Value;

        if (supplierDto.Name is not null)
            supplier.Name = supplierDto.Name;
        if (supplierDto.ContactInfo is not null)
            supplier.Info = supplierDto.ContactInfo;
        if (supplierDto.Email is not null)
            supplier.Email = supplierDto.Email;
        if (supplierDto.Phone is not null)
            supplier.Phone = supplierDto.Phone;
        if (supplierDto.Address is not null)
            supplier.Address = supplierDto.Address;
        if (supplierDto.Website is not null)
            supplier.Website = supplierDto.Website;
        if (supplierDto.Description is not null)
            supplier.Description = supplierDto.Description;

        NormalizeSupplierOptionalFields(supplier);
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
    private static void NormalizeSupplierOptionalFields(Supplier supplier)
    {
        supplier.Email ??= string.Empty;
        supplier.Phone ??= string.Empty;
        supplier.Address ??= string.Empty;
        supplier.Website ??= string.Empty;
        supplier.Description ??= string.Empty;
    }

    private string GetRequiredCurrentUserId()
    {
        if (!string.IsNullOrWhiteSpace(_currentUserService.UserId))
        {
            return _currentUserService.UserId;
        }

        throw new UnauthorizedAccessException("You must be logged in to approve a supplier.");
    }

}
