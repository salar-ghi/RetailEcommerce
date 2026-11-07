namespace Application.DTOs;

public class TrackUserActivityDto
{
    public string? UserId { get; set; }
    public string? SessionId { get; set; }
    public string Platform { get; set; } = "bazaar-azad"; // bazaar-azad or store-panel
    public string Action { get; set; } = string.Empty; // page_view, product_view, search, add_to_cart, remove_from_cart, checkout_step, admin_edit, filter_category, etc.
    public string? EntityType { get; set; } // Product, Category, Order, etc.
    public string? EntityId { get; set; }
    public string? Path { get; set; }
    public string? Method { get; set; }
    public int? StatusCode { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public string? MetadataJson { get; set; }
}

public class BatchTrackUserActivityDto
{
    public List<TrackUserActivityDto> Activities { get; set; } = new();
}

public class UserActivityDto
{
    public long Id { get; set; }
    public string? UserId { get; set; }
    public string? SessionId { get; set; }
    public string Platform { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string? EntityType { get; set; }
    public string? EntityId { get; set; }
    public string? Path { get; set; }
    public string? Method { get; set; }
    public int? StatusCode { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public string? MetadataJson { get; set; }
    public DateTime CreatedTime { get; set; }
}

public class UserActivityFilterDto
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

public class PagedResultDto<T>
{
    public IReadOnlyList<T> Items { get; set; } = Array.Empty<T>();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => PageSize > 0 ? (int)Math.Ceiling((double)TotalCount / PageSize) : 0;
}

public class UserActivitySummaryDto
{
    public int TotalEvents { get; set; }
    public int UniqueUsers { get; set; }
    public int UniqueSessions { get; set; }
    public IReadOnlyDictionary<string, int> ActionBreakdown { get; set; } = new Dictionary<string, int>();
    public IReadOnlyDictionary<string, int> PlatformBreakdown { get; set; } = new Dictionary<string, int>();
    public IReadOnlyList<TopTrackedEntityDto> TopViewedProducts { get; set; } = Array.Empty<TopTrackedEntityDto>();
    public IReadOnlyList<TopTrackedEntityDto> TopSearchedKeywords { get; set; } = Array.Empty<TopTrackedEntityDto>();
}

public class TopTrackedEntityDto
{
    public string EntityId { get; set; } = string.Empty;
    public string? Name { get; set; }
    public int Count { get; set; }
}
