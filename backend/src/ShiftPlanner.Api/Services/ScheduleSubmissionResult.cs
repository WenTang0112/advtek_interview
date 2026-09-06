namespace ShiftPlanner.Api.Services;

public sealed record ScheduleSubmissionResult(
    bool Succeeded,
    string Code,
    string Message,
    int ScheduledDays)
{
    public static ScheduleSubmissionResult Success(int scheduledDays) =>
        new(true, "Success", "班表已通過提交驗證。", scheduledDays);

    public static ScheduleSubmissionResult Failure(string code, string message, int scheduledDays) =>
        new(false, code, message, scheduledDays);
}