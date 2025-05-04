namespace Application.Interfaces;

public interface IUserService
{
    Task<string> GenerateRefreshTokenAsync(User user);
    Task<User> ValidateRefreshTokenAsync(string refreshToken);

    Task<User> RegisterAsync(SignupDto dto);
    Task<(string jwtToken, string refreshToken)> AuthenticateAsync(LoginDto dto);
    Task<User> AddUserAsync(AddUserDto dto);


    //***********
    Task<IEnumerable<UserDto>> GetAllUsersAsync();
    Task<UserDto> GetUserByIdAsync(string id);
    Task UpdateUserAsync(UserDto userDto);
    Task DeleteUserAsync(string id);
    Task<IEnumerable<UserDto>> SearchUsersByUsernameAsync(string username);
    Task<IEnumerable<UserDto>> SearchUsersByEmailAsync(string email);
}
