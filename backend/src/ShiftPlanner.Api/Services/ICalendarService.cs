namespace ShiftPlanner.Api.Services;

public interface ICalendarService
{
    bool IsWeekend(DateOnly date);

    bool IsHoliday(DateOnly date);

    string? GetHolidayName(DateOnly date);

    bool IsSchedulableMonth(DateOnly date);

    bool IsWorkday(DateOnly date);
}