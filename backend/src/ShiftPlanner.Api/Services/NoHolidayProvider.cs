namespace ShiftPlanner.Api.Services;

public class NoHolidayProvider : IHolidayProvider
{
    public HolidayInfo? GetHoliday(DateOnly date)
    {
        return null;
    }
}