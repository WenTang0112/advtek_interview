namespace ShiftPlanner.Api.ViewModels;

public sealed record StatisticsDashboardDto(
    int Year,
    int Month,
    IReadOnlyList<EmployeeWorkdaySummaryDto> EmployeeWorkdays,
    IReadOnlyList<MonthlyWorkdayRankingDto> MonthlyRanking,
    IReadOnlyList<DailyScheduleDto> DailySchedules);

public sealed record EmployeeWorkdaySummaryDto(
    int EmployeeId,
    string EmployeeName,
    int MonthlyWorkdays,
    int YearlyWorkdays);

public sealed record MonthlyWorkdayRankingDto(
    int Rank,
    int EmployeeId,
    string EmployeeName,
    int Workdays);

public sealed record DailyScheduleDto(
    DateOnly WorkDate,
    IReadOnlyList<ScheduledEmployeeDto> Employees);

public sealed record ScheduledEmployeeDto(int EmployeeId, string EmployeeName);