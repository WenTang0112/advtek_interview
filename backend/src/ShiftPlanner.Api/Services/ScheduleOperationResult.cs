using ShiftPlanner.Api.Models;

namespace ShiftPlanner.Api.Services;

public sealed record ScheduleOperationResult(
    bool Succeeded,
    string Code,
    string Message,
    Schedule? Schedule = null)
{
    public static ScheduleOperationResult Success(Schedule schedule) =>
        new(true, "Success", "排班已建立。", schedule);

    public static ScheduleOperationResult Failure(string code, string message) =>
        new(false, code, message);
}