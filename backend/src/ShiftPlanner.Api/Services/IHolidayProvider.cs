namespace ShiftPlanner.Api.Services;

public interface IHolidayProvider
{
    bool IsHoliday(DateOnly date);
}