using ShiftPlanner.Api.ViewModels;

namespace ShiftPlanner.Api.Services;

public interface IStatisticsService
{
    Task<StatisticsDashboardDto> GetDashboardAsync(
        int year,
        int month,
        CancellationToken cancellationToken = default);
}