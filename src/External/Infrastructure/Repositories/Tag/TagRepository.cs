namespace Infrastructure.Repositories;

public class TagRepository : Repository<Tag, int>, ITagRepository
{
    public TagRepository(AppDbContext context) : base(context) { }
    public async Task<IEnumerable<Tag>> SearchByNameAsync(string name)
    {
        return await _context.Tags
            .Where(t => t.Name.Contains(name))
            .AsNoTracking()
            .ToListAsync();
    }

}
