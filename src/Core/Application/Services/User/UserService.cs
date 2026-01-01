using Application.Common;

namespace Application.Services;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;

    public UserService(IUnitOfWork unitOfWork, IMapper mapper,
        IPasswordHasher passwordHasher, ICurrentUserService currentUserService,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
        _currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
    }

    public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
    {
        var users = await _unitOfWork.Users.GetAllAsync();
        return _mapper.Map<IEnumerable<UserDto>>(users);
    }

    public async Task<UserDto> GetUserByIdAsync(string id)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(id);
        if (user == null) throw new KeyNotFoundException($"User with ID {id} not found.");
        return _mapper.Map<UserDto>(user);
    }

    //public async Task AddUserAsync(UserDto userDto)
    //{
    //    var user = _mapper.Map<User>(userDto);
    //    await _unitOfWork.Users.AddAsync(user);
    //    await _unitOfWork.SaveChangesAsync();
    //}

    public async Task<Result<string>> AddUserAsync(AddUserDto dto)
    {
        var existingUser = await _unitOfWork.Users.GetByAsync(u => u.PhoneNumber == dto.PhoneNumber);
        if (existingUser != null)
        {
            return Result<string>.Failure("شماره همراه تکراری می باشد");
        }

        //var password = _currentUserService.GenerateRandomPassword();
        var password = "12345678*";
        var passwordHash = _passwordHasher.HashPassword(password);

        var user = new User
        {
            Id = Guid.NewGuid().ToString(),
            PhoneNumber = dto.PhoneNumber,
            Username = dto.PhoneNumber,
            PasswordHash = passwordHash,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            IsActive = true,
            IsEmailConfirmed = false,
            TwoFactorEnabled = false,
            CreatedBy = _currentUserService.UserId,
            CreatedTime = DateTime.Now,
            ModifiedBy = _currentUserService.UserId,
            ModifiedTime = DateTime.Now,
        };

        var roles = await _unitOfWork.Roles.GetAllAsync(r => dto.Roles.Contains(r.Name));
        if (roles != null)
        {
            foreach (var role in roles)
            {
                user.UserRoles.Add(new UserRole
                {
                    //Id = 1,
                    UserId = user.Id,
                    RoleId = role.Id,
                    CreatedBy = _currentUserService.UserId,
                    ModifiedBy = _currentUserService.UserId,
                    CreatedTime = DateTime.Now,
                });
                //await _unitOfWork.UserRoles.AddAsync(userRole);
                //await _unitOfWork.SaveChangesAsync();
            }
        }
        await _unitOfWork.Users.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        //var userId = user.Id;
        return Result<string>.Success("کاربر مورد نظر با موفقیت ثبت گردید.");
    }

    public async Task UpdateUserAsync(UserDto userDto)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userDto.Id);
        if (user == null) throw new KeyNotFoundException($"User with ID {userDto.Id} not found.");
        _mapper.Map(userDto, user);
        await _unitOfWork.Users.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteUserAsync(string id)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(id);
        if (user != null)
        {
            await _unitOfWork.Users.DeleteAsync(user);
            await _unitOfWork.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<UserDto>> SearchUsersByUsernameAsync(string username)
    {
        var users = await _unitOfWork.Users.SearchByUsernameAsync(username);
        return _mapper.Map<IEnumerable<UserDto>>(users);
    }

    public async Task<IEnumerable<UserDto>> SearchUsersByEmailAsync(string email)
    {
        var users = await _unitOfWork.Users.SearchByEmailAsync(email);
        return _mapper.Map<IEnumerable<UserDto>>(users);
    }

    //public async Task<BasketDto> GetUserBasketAsync(string userId)
    //{
    //    var basket = await _basketRepository.GetSingleAsync(b => b.UserId == userId);
    //    if (basket == null) throw new NotFoundException("Basket not found");
    //    return _mapper.Map<BasketDto>(basket);
    //}
    public async Task<Result<string>> RegisterAsync(SignupDto dto)
    {
        var existingUser = await _unitOfWork.Users.GetByAsync(u => u.PhoneNumber == dto.PhoneNumber);
        if (existingUser != null)
        {
            return Result<string>.Failure("شماره همراه تکراری می باشد");
        }
        var passwordHash = _passwordHasher.HashPassword(dto.Password);
        var user = new User
        {
            Id = Guid.NewGuid().ToString(),
            PhoneNumber = dto.PhoneNumber,
            PasswordHash = passwordHash,
            IsActive = true,
            IsEmailConfirmed = false,
            TwoFactorEnabled = false,
            Username = dto.PhoneNumber,
            CreatedBy = "bdfb65f1-9024-4736-846d-df7de909f571",
            ModifiedBy = "bdfb65f1-9024-4736-846d-df7de909f571",
        };

        await _unitOfWork.Users.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();
        return Result<string>.Success("ثبت نام با موفقیت انجام شد");
    }

    public async Task<AuthResult> AuthenticateAsync(LoginDto dto)
    {
        var user = await _unitOfWork.Users.GetByAsync(u => u.Username == dto.Username);
        if (user == null || !_passwordHasher.VerifyPassword(dto.Password, user.PasswordHash))
        {
            return AuthResult.Failure("Invalid credentials");
        }
        var jwtToken = _jwtTokenGenerator.GenerateJwtToken(user);
        var refreshToken = GenerateRandomRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        user.LastLoginTime = DateTime.UtcNow;

        await _unitOfWork.Users.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync();

        return AuthResult.Success(jwtToken, refreshToken);
    }

    public async Task<User> ValidateRefreshTokenAsync(string refreshToken)
    {
        var user = await _unitOfWork.Users.GetByRefreshTokenAsync(refreshToken);
        if (user == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            return null;
        }
        return user;
    }
    public async Task<string> GenerateRefreshTokenAsync(User user)
    {
        user.RefreshToken = GenerateRandomRefreshToken();
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7); // Expires in 7 days
        await _unitOfWork.SaveChangesAsync();
        return user.RefreshToken;
    }

    private string GenerateRandomRefreshToken()
    {
        var randomNumber = new byte[32];
        using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}