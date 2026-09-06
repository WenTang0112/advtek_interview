namespace ShiftPlanner.Api.ViewModels;

public sealed record ScheduleCalendarDayDto(
    DateOnly WorkDate,
    bool CanSchedule,
    bool IsMine,
    int? MyScheduleId,
    bool IsSaturday,
    bool IsSunday,
    int ScheduledEmployeeCount,
    string? HolidayName);