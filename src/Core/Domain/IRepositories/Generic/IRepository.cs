namespace Domain.IRepositories;

public interface IRepository<T, TId> where T : class
{
    Task<T> GetByIdAsync(TId id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> predicate);
    IQueryable<T> GetAll();
    Task<T> GetByAsync(Func<T, bool> predicate);
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
    Task<T> GetSingleAsync(Expression<Func<T, bool>> predicate);
}