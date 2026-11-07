namespace Application.Interfaces;

public interface IUserActivityService
{
    Task<UserActivityDto> TrackActivityAsync(TrackUserActivityDto dto, CancellationToken cancellationToken = default);
    Task<int> TrackBatchActivitiesAsync(BatchTrackUserActivityDto dto, CancellationToken cancellationToken = default);
    Task<PagedResultDto<UserActivityDto>> GetActivitiesAsync(UserActivityFilterDto filter, CancellationToken cancellationToken = default);
    Task<UserActivitySummaryDto> GetActivitySummaryAsync(string? platform = null, DateTime? fromDate = null, DateTime? toDate = null, CancellationToken cancellationToken = default);
}
