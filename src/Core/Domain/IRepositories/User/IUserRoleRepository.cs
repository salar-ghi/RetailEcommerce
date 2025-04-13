namespace Domain.IRepositories;

public interface IUserRoleRepository : IRepository<UserRole, int>
{
    Task<IEnumerable<UserRole>> GetByUserIdAsync(int userId);
    Task<IEnumerable<UserRole>> GetByRoleIdAsync(int roleId);
    Task DeleteAsync(int userId, int roleId);
}