namespace Application.Interfaces;

public interface IUserService
{
    //Task<BasketDto> GetUserBasketAsync(string userId);
    Task<User> RegisterAsync(string username, string password, string role);
    Task<User> AuthenticateAsync(string username, string password);
    Task<string> GenerateRefreshTokenAsync(User user);
    Task<User> ValidateRefreshTokenAsync(string refreshToken);

}
