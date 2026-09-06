namespace ShiftPlanner.Api.Services;

public interface IHolidayProvider
{
    HolidayInfo? GetHoliday(DateOnly date);
}