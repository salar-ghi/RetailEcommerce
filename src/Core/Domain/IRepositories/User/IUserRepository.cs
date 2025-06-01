namespace Domain.IRepositories;

public interface IUserRepository : IRepository<User, string> 
{
    Task<IEnumerable<User>> SearchByUsernameAsync(string username);
    Task<User> GetByRefreshTokenAsync(string refreshToken);
    Task<User> GetByUsernameAsync(string username);
    Task<User> GetByPhonenumberAsync(string phonenum);
    Task<IEnumerable<User>> SearchByEmailAsync(string email);
}