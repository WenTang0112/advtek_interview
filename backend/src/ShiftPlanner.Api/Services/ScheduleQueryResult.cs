using ShiftPlanner.Api.Models;

namespace ShiftPlanner.Api.Services;

public sealed record ScheduleQueryResult(
    bool Succeeded,
    string Code,
    string Message,
    IReadOnlyList<Schedule> Schedules)
{
    public static ScheduleQueryResult Success(IReadOnlyList<Schedule> schedules) =>
        new(true, "Success", "班表查詢成功。", schedules);

    public static ScheduleQueryResult Failure(string code, string message) =>
        new(false, code, message, []);
}