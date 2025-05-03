namespace Application.Services;
using BCrypt.Net;
public class UserService //: IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UserService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
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

    public async Task AddUserAsync(UserDto userDto)
    {
        var user = _mapper.Map<User>(userDto);
        await _unitOfWork.Users.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();
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
    public async Task<User> RegisterAsync(string username, string password, string role)
    {
        var existingUser = await _unitOfWork.Users.GetByUsernameAsync(username);
        if (existingUser != null)
        {
            throw new Exception("Username already exists");
        }

        string passwordHash = BCrypt.HashPassword(password);
        var user = new User
        {
            Username = username,
            PasswordHash = passwordHash,
            //Role = role
        };

        await _unitOfWork.Users.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();
        return user;
    }

    public async Task<User> AuthenticateAsync(string username, string password)
    {
        var user = await _unitOfWork.Users.GetByUsernameAsync(username);
        if (user == null || !BCrypt.Verify(password, user.PasswordHash))
        {
            return null;
        }
        return user;
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