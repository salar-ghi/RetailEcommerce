namespace Domain.IRepositories;

public interface ITagRepository : IRepository<Tag, int> 
{
    Task<IEnumerable<Tag>> SearchByNameAsync(string name);
}
