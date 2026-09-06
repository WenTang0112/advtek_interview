namespace ShiftPlanner.Api.Services;

public class NoHolidayProvider : IHolidayProvider
{
    public bool IsHoliday(DateOnly date)
    {
        return false;
    }
}