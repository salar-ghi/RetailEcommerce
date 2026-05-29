namespace Domain.IRepositories;

public interface IStorageSpaceRepository : IRepository<StorageSpace, int>
{
    Task<IEnumerable<StorageSpace>> SearchAsync(string searchTerm);
}
