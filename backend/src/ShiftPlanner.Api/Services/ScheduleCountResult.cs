namespace ShiftPlanner.Api.Services;

public sealed record ScheduleCountResult(
    bool Succeeded,
    string Code,
    string Message,
    int ScheduledDays)
{
    public static ScheduleCountResult Success(int scheduledDays) =>
        new(true, "Success", "本月排班天數查詢成功。", scheduledDays);

    public static ScheduleCountResult Failure(string code, string message) =>
        new(false, code, message, 0);
}