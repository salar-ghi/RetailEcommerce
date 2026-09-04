namespace Application.Interfaces;

public interface IAnalyticsService
{
    Task<AnalyticsOverviewDto> GetOverviewAsync(AnalyticsQueryDto query, CancellationToken cancellationToken = default);
}
