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

    public async Task<User> GetByRefreshTokenAsync(string refreshToken)
    {
        return await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
    }
}