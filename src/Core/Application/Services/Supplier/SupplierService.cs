using Application.Common;

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

        //supplier.Status = request.IsApproved ? SupplierStatus.Approved : SupplierStatus.Rejected;
        supplier.Status = SupplierStatus.Approved;
        supplier.ApprovalDate = DateTime.Now;
        supplier.ModifiedTime = DateTime.Now;
        //supplier.ModifiedBy = _currentUserService.UserId;
        //supplier.ApprovedByUserId = _currentUserService.UserId;
        supplier.ApprovedByUserId = "576b27d5-11b8-4c21-b211-6e4882f1a80e";
        supplier.ModifiedBy = "576b27d5-11b8-4c21-b211-6e4882f1a80e";
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

    public async Task<Result<string>> CreateSupplierAsync(SupplierRegistrationDto supplierDto)
    {
        var sessionId = _currentUserService.UserId;
        var existedSupplier = await _unitOfWork.Suppliers.GetSingleAsync(s => s.Info == supplierDto.ContactInfo && !s.IsDeleted);
        if (existedSupplier != null)
            return Result<string>.Failure("تامین کننده با شماره تلفن مشابه موجود میباشد");

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
                CreatedBy = sessionId,
                CreatedTime = DateTime.Now,
                ModifiedBy = sessionId,
                ModifiedTime = DateTime.Now,
            };
            await _unitOfWork.Users.AddAsync(user);

            var customerRole = (await _unitOfWork.Roles.GetSingleAsync(r => r.Name == "Customer"));
            await _unitOfWork.UserRoles.AddAsync(new UserRole { UserId = user.Id, RoleId = customerRole.Id });
        }

        CreateSuplier:
        var supplier = _mapper.Map<Supplier>(supplierDto);
        supplier.CreatedTime = DateTime.Now;
        supplier.ModifiedTime = DateTime.Now;
        supplier.UserId = user.Id;
        supplier.CreatedBy = sessionId;
        supplier.ModifiedBy = sessionId;

        await _unitOfWork.Suppliers.AddAsync(supplier);

        var supplierRole = (await _unitOfWork.Roles.GetSingleAsync(r => r.Name == "Supplier"));
        await _unitOfWork.UserRoles.AddAsync(new UserRole { UserId = user.Id, RoleId = supplierRole.Id });
        await _unitOfWork.SaveChangesAsync();

        return Result<string>.Success("تامین کننده مورد نظر با موفقیت ثبت گردید");
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
        supplier.CreatedTime = DateTime.Now;
        supplier.ModifiedTime = DateTime.Now;
        supplier.UserId = user.Id;
        supplier.CreatedBy = sessionId;
        supplier.ModifiedBy = sessionId;
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
        _mapper.Map(supplierDto, supplier);

        if (supplierDto.Status.HasValue)
            supplier.Status = supplierDto.Status.Value;

        if (!string.IsNullOrEmpty(supplierDto.Phone))
            supplier.Phone = supplierDto.Phone;
        if (!string.IsNullOrEmpty(supplierDto.ContactInfo))
            supplier.Info = supplierDto.ContactInfo;
        if (supplierDto.Email is not null)
            supplier.Email = supplierDto.Email;
        if (supplierDto.Name is not null)
            supplier.Name = supplierDto.Name;

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