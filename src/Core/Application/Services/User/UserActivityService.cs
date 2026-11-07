namespace Application.Services;

public class UserActivityService : IUserActivityService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UserActivityService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<UserActivityDto> TrackActivityAsync(TrackUserActivityDto dto, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dto);

        if (string.IsNullOrWhiteSpace(dto.Action))
            throw new ArgumentException("Action is required.", nameof(dto));

        var activity = new UserActivity
        {
            UserId = dto.UserId,
            SessionId = dto.SessionId,
            Platform = NormalizePlatform(dto.Platform),
            Action = dto.Action.Trim().ToLowerInvariant(),
            EntityType = dto.EntityType,
            EntityId = dto.EntityId,
            Path = dto.Path,
            Method = dto.Method,
            StatusCode = dto.StatusCode,
            IpAddress = dto.IpAddress,
            UserAgent = dto.UserAgent,
            MetadataJson = dto.MetadataJson,
            CreatedTime = DateTime.UtcNow
        };

        await _unitOfWork.UserActivities.AddAsync(activity);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<UserActivityDto>(activity);
    }

    public async Task<int> TrackBatchActivitiesAsync(BatchTrackUserActivityDto dto, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dto);

        if (dto.Activities == null || dto.Activities.Count == 0)
            return 0;

        var now = DateTime.UtcNow;
        var entities = dto.Activities
            .Where(a => !string.IsNullOrWhiteSpace(a.Action))
            .Select(a => new UserActivity
            {
                UserId = a.UserId,
                SessionId = a.SessionId,
                Platform = NormalizePlatform(a.Platform),
                Action = a.Action.Trim().ToLowerInvariant(),
                EntityType = a.EntityType,
                EntityId = a.EntityId,
                Path = a.Path,
                Method = a.Method,
                StatusCode = a.StatusCode,
                IpAddress = a.IpAddress,
                UserAgent = a.UserAgent,
                MetadataJson = a.MetadataJson,
                CreatedTime = now
            })
            .ToList();

        if (entities.Count == 0)
            return 0;

        foreach (var entity in entities)
        {
            await _unitOfWork.UserActivities.AddAsync(entity);
        }

        await _unitOfWork.SaveChangesAsync();
        return entities.Count;
    }

    public async Task<PagedResultDto<UserActivityDto>> GetActivitiesAsync(UserActivityFilterDto filter, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(filter);

        var query = new UserActivityQuery
        {
            Platform = string.IsNullOrWhiteSpace(filter.Platform) ? null : NormalizePlatform(filter.Platform),
            UserId = filter.UserId,
            SessionId = filter.SessionId,
            Action = string.IsNullOrWhiteSpace(filter.Action) ? null : filter.Action.Trim().ToLowerInvariant(),
            EntityType = filter.EntityType,
            EntityId = filter.EntityId,
            FromDate = filter.FromDate,
            ToDate = filter.ToDate,
            Page = filter.Page > 0 ? filter.Page : 1,
            PageSize = filter.PageSize > 0 && filter.PageSize <= 200 ? filter.PageSize : 50
        };

        var totalCount = await _unitOfWork.UserActivities.GetActivitiesCountAsync(query, cancellationToken);
        var items = await _unitOfWork.UserActivities.GetActivitiesAsync(query, cancellationToken);

        return new PagedResultDto<UserActivityDto>
        {
            Items = _mapper.Map<IReadOnlyList<UserActivityDto>>(items),
            TotalCount = totalCount,
            Page = query.Page,
            PageSize = query.PageSize
        };
    }

    public async Task<UserActivitySummaryDto> GetActivitySummaryAsync(string? platform = null, DateTime? fromDate = null, DateTime? toDate = null, CancellationToken cancellationToken = default)
    {
        var normalizedPlatform = string.IsNullOrWhiteSpace(platform) ? null : NormalizePlatform(platform);

        var allQuery = new UserActivityQuery
        {
            Platform = normalizedPlatform,
            FromDate = fromDate,
            ToDate = toDate,
            Page = 1,
            PageSize = 5000 // Sample size for aggregations
        };

        var activities = (await _unitOfWork.UserActivities.GetActivitiesAsync(allQuery, cancellationToken)).ToList();
        var totalCount = await _unitOfWork.UserActivities.GetActivitiesCountAsync(allQuery, cancellationToken);

        var actionCounts = activities
            .GroupBy(a => a.Action)
            .ToDictionary(g => g.Key, g => g.Count());

        var platformCounts = activities
            .GroupBy(a => a.Platform)
            .ToDictionary(g => g.Key, g => g.Count());

        var topProducts = activities
            .Where(a => a.Action == "product_view" && !string.IsNullOrWhiteSpace(a.EntityId))
            .GroupBy(a => a.EntityId!)
            .OrderByDescending(g => g.Count())
            .Take(10)
            .Select(g => new TopTrackedEntityDto
            {
                EntityId = g.Key,
                Count = g.Count()
            })
            .ToList();

        var topSearches = activities
            .Where(a => a.Action == "search" && !string.IsNullOrWhiteSpace(a.EntityId))
            .GroupBy(a => a.EntityId!)
            .OrderByDescending(g => g.Count())
            .Take(10)
            .Select(g => new TopTrackedEntityDto
            {
                EntityId = g.Key,
                Count = g.Count()
            })
            .ToList();

        return new UserActivitySummaryDto
        {
            TotalEvents = totalCount,
            UniqueUsers = activities.Where(a => !string.IsNullOrWhiteSpace(a.UserId)).Select(a => a.UserId!).Distinct().Count(),
            UniqueSessions = activities.Where(a => !string.IsNullOrWhiteSpace(a.SessionId)).Select(a => a.SessionId!).Distinct().Count(),
            ActionBreakdown = actionCounts,
            PlatformBreakdown = platformCounts,
            TopViewedProducts = topProducts,
            TopSearchedKeywords = topSearches
        };
    }

    private static string NormalizePlatform(string? platform)
    {
        if (string.IsNullOrWhiteSpace(platform))
            return "bazaar-azad";

        var trimmed = platform.Trim().ToLowerInvariant();
        if (trimmed.Contains("store") || trimmed.Contains("panel") || trimmed.Contains("admin"))
            return "store-panel";

        return "bazaar-azad";
    }
}
