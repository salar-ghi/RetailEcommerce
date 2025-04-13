namespace Domain.IRepositories;

public interface IRoleRepository : IRepository<Role, int> 
{
    Task<IEnumerable<Role>> SearchByNameAsync(string name);
}
