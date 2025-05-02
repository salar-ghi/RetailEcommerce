namespace Domain.IRepositories;

public interface IUserRoleRepository : IRepository<UserRole, int>
{
    Task<IEnumerable<UserRole>> GetByUserIdAsync(string userId);
    Task<IEnumerable<UserRole>> GetByRoleIdAsync(int roleId);
    Task DeleteAsync(string userId, int roleId);
}