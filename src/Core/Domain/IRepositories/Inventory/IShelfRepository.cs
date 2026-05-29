namespace Domain.IRepositories;

public interface IShelfRepository : IRepository<Shelf, int>
{
    Task<IEnumerable<Shelf>> GetBySpaceIdAsync(int spaceId);
    Task<IEnumerable<Shelf>> GetByZoneIdAsync(int zoneId);
    Task<Shelf> GetByCodeAsync(string code);
}
