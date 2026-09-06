namespace ShiftPlanner.Api.Services;

public interface IScheduleService
{
    Task<ScheduleOperationResult> CreateAsync(
        int employeeId,
        DateOnly workDate,
        CancellationToken cancellationToken = default);
}