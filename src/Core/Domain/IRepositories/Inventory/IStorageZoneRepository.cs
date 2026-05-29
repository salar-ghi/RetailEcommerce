namespace Domain.IRepositories;

public interface IStorageZoneRepository : IRepository<StorageZone, int>
{
    Task<IEnumerable<StorageZone>> GetBySpaceIdAsync(int spaceId);
}
