namespace Infrastructure.Repositories;

public class UserRepository : Repository<User, string>, IUserRepository
{
    public UserRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<User>> SearchByUsernameAsync(string username)
    {
        return await _context.Users
            .Where(u => u.Username.Contains(username))
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<User>> SearchByEmailAsync(string email)
    {
        return await _context.Users
            .Where(u => u.Email.Contains(email))
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<User> GetByUsernameAsync(string username)
    {
        return await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<User> GetByPhonenumberAsync(string phonenum)
    {
        return await _context.Users
            .AsNoTracking()
            .Where(z => z.PhoneNumber == phonenum)
            .FirstOrDefaultAsync();
    }

    public async Task<User> GetByRefreshTokenAsync(string refreshToken)
    {
        return await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
    }

    public async Task<IEnumerable<User>> GetAllWithRolesAsync()
    {
        return await _context.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .AsNoTracking()
            .ToListAsync();
    }
}