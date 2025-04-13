namespace Infrastructure.Repositories;

// Infrastructure/Repositories/UserAddressRepository.cs
public class UserAddressRepository : Repository<UserAddress, int>, IUserAddressRepository
{
    public UserAddressRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<UserAddress>> GetByUserIdAsync(int userId)
    {
        return await _context.UserAddresses
            .Where(ua => ua.UserId == userId)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<UserAddress>> SearchByCityAsync(string city)
    {
        return await _context.UserAddresses
            .Where(ua => ua.City.Contains(city))
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<UserAddress>> SearchByCountryAsync(string country)
    {
        return await _context.UserAddresses
            .Where(ua => ua.Country.Contains(country))
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<UserAddress>> SearchByPrimaryAsync(bool isPrimary)
    {
        return await _context.UserAddresses
            .Where(ua => ua.IsPrimary == isPrimary)
            .AsNoTracking()
            .ToListAsync();
    }
}