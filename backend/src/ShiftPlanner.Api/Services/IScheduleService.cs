namespace ShiftPlanner.Api.Services;

public interface IScheduleService
{
    Task<ScheduleCalendarResult> GetNextMonthCalendarAsync(
        int employeeId,
        CancellationToken cancellationToken = default);

    Task<ScheduleQueryResult> GetNextMonthAsync(
        int employeeId,
        CancellationToken cancellationToken = default);

    Task<ScheduleOperationResult> CreateAsync(
        int employeeId,
        DateOnly workDate,
        CancellationToken cancellationToken = default);

    Task<ScheduleOperationResult> CancelAsync(
        int employeeId,
        int scheduleId,
        CancellationToken cancellationToken = default);

    Task<ScheduleSubmissionResult> SubmitScheduleAsync(
        int employeeId,
        DateOnly scheduleMonth,
        CancellationToken cancellationToken = default);

    Task<ScheduleCountResult> GetCurrentMonthCountAsync(
        int employeeId,
        CancellationToken cancellationToken = default);
}