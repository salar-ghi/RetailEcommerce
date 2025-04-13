namespace Infrastructure.Repositories;

public class RoleRepository : Repository<Role, int>, IRoleRepository
{
    public RoleRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Role>> SearchByNameAsync(string name)
    {
        return await _context.Roles
            .Where(r => r.Name.Contains(name))
            .AsNoTracking()
            .ToListAsync();
    }
}