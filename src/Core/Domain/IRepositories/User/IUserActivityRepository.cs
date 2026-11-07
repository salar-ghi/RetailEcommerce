namespace Domain.IRepositories;

public class UserActivityQuery
{
    public string? Platform { get; set; }
    public string? UserId { get; set; }
    public string? SessionId { get; set; }
    public string? Action { get; set; }
    public string? EntityType { get; set; }
    public string? EntityId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 50;
}

public interface IUserActivityRepository : IRepository<UserActivity, long>
{
    Task<IEnumerable<UserActivity>> GetActivitiesAsync(UserActivityQuery query, CancellationToken cancellationToken = default);
    Task<int> GetActivitiesCountAsync(UserActivityQuery query, CancellationToken cancellationToken = default);
    Task<IReadOnlyDictionary<string, int>> GetActionCountsByPlatformAsync(string? platform, DateTime? fromDate, DateTime? toDate, CancellationToken cancellationToken = default);
}
