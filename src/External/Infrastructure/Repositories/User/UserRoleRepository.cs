namespace Infrastructure.Repositories;

public class UserRoleRepository : Repository<UserRole, int> , IUserRoleRepository
{
    public UserRoleRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<UserRole>> GetByUserIdAsync(int userId)
    {
        return await _context.UserRoles
            .Where(ur => ur.UserId == userId)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<UserRole>> GetByRoleIdAsync(int roleId)
    {
        return await _context.UserRoles
            .Where(ur => ur.RoleId == roleId)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task DeleteAsync(int userId, int roleId)
    {
        var entity = await _context.UserRoles
            .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleId);
        if (entity != null)
        {
            _context.UserRoles.Remove(entity);
        }
    }


}