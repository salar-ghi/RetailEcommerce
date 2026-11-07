namespace Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ActivityController : ControllerBase
{
    private readonly IUserActivityService _activityService;

    public ActivityController(IUserActivityService activityService)
    {
        _activityService = activityService;
    }

    /// <summary>
    /// Record a single user action or interaction from bazaar-azad or store-panel.
    /// Captures actions like product views, searches, cart actions, category visits for recommendations.
    /// </summary>
    [HttpPost("track")]
    public async Task<ActionResult<UserActivityDto>> Track([FromBody] TrackUserActivityDto dto, CancellationToken cancellationToken)
    {
        if (dto == null || string.IsNullOrWhiteSpace(dto.Action))
            return BadRequest("Action is required.");

        dto.IpAddress ??= HttpContext.Connection.RemoteIpAddress?.ToString();
        dto.UserAgent ??= HttpContext.Request.Headers.UserAgent.ToString();
        dto.Path ??= HttpContext.Request.Path;

        var result = await _activityService.TrackActivityAsync(dto, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Record a batch of activities queued from frontend (bazaar-azad or store-panel) to reduce network requests.
    /// </summary>
    [HttpPost("batch")]
    public async Task<ActionResult<int>> TrackBatch([FromBody] BatchTrackUserActivityDto dto, CancellationToken cancellationToken)
    {
        if (dto?.Activities == null || dto.Activities.Count == 0)
            return BadRequest("No activities provided.");

        var clientIp = HttpContext.Connection.RemoteIpAddress?.ToString();
        var clientUserAgent = HttpContext.Request.Headers.UserAgent.ToString();

        foreach (var act in dto.Activities)
        {
            act.IpAddress ??= clientIp;
            act.UserAgent ??= clientUserAgent;
        }

        var count = await _activityService.TrackBatchActivitiesAsync(dto, cancellationToken);
        return Ok(new { Count = count });
    }

    /// <summary>
    /// Query tracked activities with filtering by platform, user, session, action, and date range.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<PagedResultDto<UserActivityDto>>> GetActivities([FromQuery] UserActivityFilterDto filter, CancellationToken cancellationToken)
    {
        var result = await _activityService.GetActivitiesAsync(filter, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Retrieve activity summary and behavioral aggregations (top viewed products, search keywords, action breakdown).
    /// </summary>
    [HttpGet("summary")]
    public async Task<ActionResult<UserActivitySummaryDto>> GetSummary(
        [FromQuery] string? platform,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        CancellationToken cancellationToken)
    {
        var result = await _activityService.GetActivitySummaryAsync(platform, fromDate, toDate, cancellationToken);
        return Ok(result);
    }
}
