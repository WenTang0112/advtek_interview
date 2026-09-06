using ShiftPlanner.Api.ViewModels;

namespace ShiftPlanner.Api.Services;

public sealed record ScheduleCalendarResult(
    bool Succeeded,
    string Code,
    string Message,
    IReadOnlyList<ScheduleCalendarDayDto> Days)
{
    public static ScheduleCalendarResult Success(IReadOnlyList<ScheduleCalendarDayDto> days) =>
        new(true, "Success", "下個月月曆查詢成功。", days);

    public static ScheduleCalendarResult Failure(string code, string message) =>
        new(false, code, message, []);
}