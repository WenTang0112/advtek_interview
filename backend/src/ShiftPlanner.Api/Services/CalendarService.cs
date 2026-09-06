namespace ShiftPlanner.Api.Services;

public class CalendarService(IHolidayProvider holidayProvider, TimeProvider timeProvider) : ICalendarService
{
    public bool IsWeekend(DateOnly date)
    {
        return date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday;
    }

    public bool IsSchedulableMonth(DateOnly date)
    {
        var today = DateOnly.FromDateTime(timeProvider.GetLocalNow().DateTime);
        var nextMonth = today.AddMonths(1);

        return date.Year == nextMonth.Year && date.Month == nextMonth.Month;
    }

    public bool IsWorkday(DateOnly date)
    {
        return !IsWeekend(date) && !holidayProvider.IsHoliday(date);
    }
}