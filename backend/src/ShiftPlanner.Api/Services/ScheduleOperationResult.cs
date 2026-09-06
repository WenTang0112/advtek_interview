using ShiftPlanner.Api.Models;

namespace ShiftPlanner.Api.Services;

public sealed record ScheduleOperationResult(
    bool Succeeded,
    string Code,
    string Message,
    Schedule? Schedule = null)
{
    public static ScheduleOperationResult Success(Schedule schedule, string message = "排班已建立。") =>
        new(true, "Success", message, schedule);

    public static ScheduleOperationResult Failure(string code, string message) =>
        new(false, code, message);
}