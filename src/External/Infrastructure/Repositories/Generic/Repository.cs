namespace Infrastructure.Repositories;

public class Repository<T, TId> : IRepository<T, TId> where T : BaseModel<TId>
{
    protected readonly AppDbContext _context;

    public Repository(AppDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<T> GetByIdAsync(TId id) => await _context.Set<T>().FindAsync(id);

    public async Task<IEnumerable<T>> GetAllAsync() => await _context.Set<T>().AsNoTracking().ToListAsync();
    public IQueryable<T> GetAll() => _context.Set<T>().AsNoTracking();


    public async Task AddAsync(T entity) => await _context.Set<T>().AddAsync(entity);

    public async Task UpdateAsync(T entity) => _context.Set<T>().Update(entity);

    public async Task DeleteAsync(T entity) => _context.Set<T>().Remove(entity);

    public async Task<T> GetSingleAsync(Expression<Func<T, bool>> predicate) => await _context.Set<T>().FirstOrDefaultAsync(predicate);
    public async Task<T> GetByAsync(Func<T, bool> predicate) => await Task.FromResult(_context.Set<T>().FirstOrDefault(predicate));
    //{
    //    return await Task.FromResult(_context.Set<T>().FirstOrDefault(predicate));
    //}
}