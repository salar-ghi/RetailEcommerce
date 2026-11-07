namespace Infrastructure.Repositories;

public class UserActivityRepository : Repository<UserActivity, long>, IUserActivityRepository
{
    public UserActivityRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<UserActivity>> GetActivitiesAsync(UserActivityQuery query, CancellationToken cancellationToken = default)
    {
        var dbQuery = BuildFilterQuery(query);

        return await dbQuery
            .OrderByDescending(x => x.CreatedTime)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetActivitiesCountAsync(UserActivityQuery query, CancellationToken cancellationToken = default)
    {
        return await BuildFilterQuery(query).CountAsync(cancellationToken);
    }

    public async Task<IReadOnlyDictionary<string, int>> GetActionCountsByPlatformAsync(string? platform, DateTime? fromDate, DateTime? toDate, CancellationToken cancellationToken = default)
    {
        var dbQuery = _context.Set<UserActivity>().AsQueryable();

        if (!string.IsNullOrWhiteSpace(platform))
            dbQuery = dbQuery.Where(x => x.Platform == platform);

        if (fromDate.HasValue)
            dbQuery = dbQuery.Where(x => x.CreatedTime >= fromDate.Value);

        if (toDate.HasValue)
            dbQuery = dbQuery.Where(x => x.CreatedTime <= toDate.Value);

        return await dbQuery
            .GroupBy(x => x.Action)
            .Select(g => new { Action = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Action, x => x.Count, cancellationToken);
    }

    private IQueryable<UserActivity> BuildFilterQuery(UserActivityQuery query)
    {
        var dbQuery = _context.Set<UserActivity>().AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Platform))
            dbQuery = dbQuery.Where(x => x.Platform == query.Platform);

        if (!string.IsNullOrWhiteSpace(query.UserId))
            dbQuery = dbQuery.Where(x => x.UserId == query.UserId);

        if (!string.IsNullOrWhiteSpace(query.SessionId))
            dbQuery = dbQuery.Where(x => x.SessionId == query.SessionId);

        if (!string.IsNullOrWhiteSpace(query.Action))
            dbQuery = dbQuery.Where(x => x.Action == query.Action);

        if (!string.IsNullOrWhiteSpace(query.EntityType))
            dbQuery = dbQuery.Where(x => x.EntityType == query.EntityType);

        if (!string.IsNullOrWhiteSpace(query.EntityId))
            dbQuery = dbQuery.Where(x => x.EntityId == query.EntityId);

        if (query.FromDate.HasValue)
            dbQuery = dbQuery.Where(x => x.CreatedTime >= query.FromDate.Value);

        if (query.ToDate.HasValue)
            dbQuery = dbQuery.Where(x => x.CreatedTime <= query.ToDate.Value);

        return dbQuery;
    }
}
