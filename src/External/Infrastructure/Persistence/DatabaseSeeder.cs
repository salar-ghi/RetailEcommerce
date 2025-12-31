namespace Infrastructure.Persistence;

using Application.Helper;

public class DatabaseSeeder
{
    private readonly AppDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    public DatabaseSeeder(AppDbContext context, IPasswordHasher passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    public async Task SeedAsync()
    {
        // Seed roles
        await SeedRolesAsync();

        // Seed user with username 09108592503
        await SeedUserAsync();
    }

    private async Task SeedRolesAsync()
    {
        // Ensure the database is created and migrations are applied
        await _context.Database.MigrateAsync();

        var rolesNames = new[] { "Manager", "Supplier", "Customer", "Admin", "employee", "Developer", "Accountant" };
        foreach (var roleName in rolesNames)
        {
            if (!await _context.Roles.AnyAsync(r => r.Name == roleName))
            {
                _context.Roles.Add(new Role { Name = roleName });
            }
        }
        await _context.SaveChangesAsync();
    }
    private async Task SeedUserAsync()
    {
        // Seed user if not exists
        const string username = "09108592503";
        if (await _context.Users.FirstOrDefaultAsync(u => u.PhoneNumber == username) == null)
        {
            var password = "12345678*";
            var passwordHash = _passwordHasher.HashPassword(password); // Hash the password

            var user = new User
            {
                Username = username,
                PhoneNumber = username, // Assuming PhoneNumber matches Username as per query
                FirstName = "Salar",
                LastName = "ghahremani",
                PasswordHash = passwordHash,
                IsActive = true,
                // Set other properties to defaults or null as needed (e.g., Email, etc.)
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync(); // Save to get the user's ID

            // Assign specific roles to the user
            var roles = await _context.Roles.Where(r => r.Name == "Manager" || r.Name == "Supplier" || r.Name == "Customer").ToListAsync();
            foreach (var role in roles)
            {
                _context.UserRoles.Add(new UserRole { UserId = user.Id, RoleId = role.Id });
            }
            await _context.SaveChangesAsync();
        }
    }
}
