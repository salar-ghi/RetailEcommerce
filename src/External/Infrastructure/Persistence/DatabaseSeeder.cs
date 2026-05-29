namespace Infrastructure.Persistence;

using Application.Helper;
using Domain.Enums;

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
        // Ensure the database is created and migrations are applied once before seeding.
        await _context.Database.MigrateAsync();

        // Seed roles
        await SeedRolesAsync();

        // Seed user with username 09108592503
        await SeedUserAsync();

        // Seed banner placements that the application depends on.
        await SeedBannerPlacementsAsync();
    }

    private async Task SeedRolesAsync()
    {
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
                IsEmailConfirmed = true,
                Email = "salar.ghi1993@gmail.com"
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
    private async Task SeedBannerPlacementsAsync()
    {
        var placements = new[]
        {
            new BannerPlacement
            {
                Id = 3,
                Name = "HOME TOP",
                Code = BannerPageCode.HOME_TOP,
                CreatedTime = new DateTime(2026, 2, 20, 9, 43, 36).AddTicks(2395852),
                ModifiedTime = new DateTime(2026, 2, 20, 13, 13, 36).AddTicks(2392581)
            },
            new BannerPlacement
            {
                Id = 4,
                Name = "HOME BOTTOM",
                Code = BannerPageCode.HOME_BOTTOM,
                CreatedTime = new DateTime(2026, 2, 20, 9, 43, 36).AddTicks(3233995),
                ModifiedTime = new DateTime(2026, 2, 20, 13, 13, 36).AddTicks(3233956)
            },
            new BannerPlacement
            {
                Id = 5,
                Name = "PRODUCT MID",
                Code = BannerPageCode.PRODUCT_MID,
                CreatedTime = new DateTime(2026, 2, 20, 9, 43, 36).AddTicks(3242640),
                ModifiedTime = new DateTime(2026, 2, 20, 13, 13, 36).AddTicks(3242623)
            },
            new BannerPlacement
            {
                Id = 6,
                Name = "PRODUCT TOP",
                Code = BannerPageCode.PRODUCT_TOP,
                CreatedTime = new DateTime(2026, 2, 20, 9, 43, 36).AddTicks(3243037),
                ModifiedTime = new DateTime(2026, 2, 20, 13, 13, 36).AddTicks(3243029)
            },
            new BannerPlacement
            {
                Id = 7,
                Name = "PRODUCT BOTTOM",
                Code = BannerPageCode.PRODUCT_BOTTOM,
                CreatedTime = new DateTime(2026, 2, 20, 9, 43, 36).AddTicks(3243338),
                ModifiedTime = new DateTime(2026, 2, 20, 13, 13, 36).AddTicks(3243331)
            }
        };

        foreach (var placement in placements)
        {
            await _context.Database.ExecuteSqlInterpolatedAsync($@"
SET IDENTITY_INSERT [dbo].[BannerPlacement] ON;

MERGE [dbo].[BannerPlacement] AS target
USING (VALUES ({placement.Id}, {placement.Name}, {placement.Code.ToString()}, {placement.RecommendedSize}, {placement.CreatedBy}, {placement.CreatedTime}, {placement.ModifiedBy}, {placement.ModifiedTime}, {placement.IsDeleted}))
    AS source ([Id], [Name], [Code], [RecommendedSize], [CreatedBy], [CreatedTime], [ModifiedBy], [ModifiedTime], [IsDeleted])
ON target.[Code] = source.[Code]
WHEN MATCHED THEN
    UPDATE SET
        [Name] = source.[Name],
        [RecommendedSize] = source.[RecommendedSize],
        [CreatedBy] = source.[CreatedBy],
        [CreatedTime] = source.[CreatedTime],
        [ModifiedBy] = source.[ModifiedBy],
        [ModifiedTime] = source.[ModifiedTime],
        [IsDeleted] = source.[IsDeleted]
WHEN NOT MATCHED BY TARGET THEN
    INSERT ([Id], [Name], [Code], [RecommendedSize], [CreatedBy], [CreatedTime], [ModifiedBy], [ModifiedTime], [IsDeleted])
    VALUES (source.[Id], source.[Name], source.[Code], source.[RecommendedSize], source.[CreatedBy], source.[CreatedTime], source.[ModifiedBy], source.[ModifiedTime], source.[IsDeleted]);

SET IDENTITY_INSERT [dbo].[BannerPlacement] OFF;");
        }
    }
}
