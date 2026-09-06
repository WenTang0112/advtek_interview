using Microsoft.EntityFrameworkCore;
using ShiftPlanner.Api.Data;
using ShiftPlanner.Api.Models;
using ShiftPlanner.Api.ViewModels;

namespace ShiftPlanner.Api.Services;

public class StatisticsService(ApplicationDbContext dbContext) : IStatisticsService
{
    public async Task<StatisticsDashboardDto> GetDashboardAsync(
        int year,
        int month,
        CancellationToken cancellationToken = default)
    {
        if (year is < 1 or > 9999)
        {
            throw new ArgumentOutOfRangeException(nameof(year));
        }

        if (month is < 1 or > 12)
        {
            throw new ArgumentOutOfRangeException(nameof(month));
        }

        var firstDayOfMonth = new DateOnly(year, month, 1);
        var firstDayOfNextMonth = firstDayOfMonth.AddMonths(1);
        var firstDayOfYear = new DateOnly(year, 1, 1);
        var firstDayOfNextYear = firstDayOfYear.AddYears(1);

        var employeeTotals = await dbContext.Employees
            .AsNoTracking()
            .Where(employee => employee.Role == EmployeeRole.Employee)
            .Select(employee => new
            {
                employee.Id,
                employee.Name,
                MonthlyWorkdays = employee.Schedules.Count(schedule =>
                    schedule.WorkDate >= firstDayOfMonth && schedule.WorkDate < firstDayOfNextMonth),
                YearlyWorkdays = employee.Schedules.Count(schedule =>
                    schedule.WorkDate >= firstDayOfYear && schedule.WorkDate < firstDayOfNextYear)
            })
            .OrderBy(employee => employee.Name)
            .ToListAsync(cancellationToken);

        var employeeWorkdays = employeeTotals
            .Select(employee => new EmployeeWorkdaySummaryDto(
                employee.Id,
                employee.Name,
                employee.MonthlyWorkdays,
                employee.YearlyWorkdays))
            .ToList();

        var monthlyRanking = employeeWorkdays
            .OrderByDescending(employee => employee.MonthlyWorkdays)
            .ThenBy(employee => employee.EmployeeName)
            .Select((employee, index) => new MonthlyWorkdayRankingDto(
                index + 1,
                employee.EmployeeId,
                employee.EmployeeName,
                employee.MonthlyWorkdays))
            .ToList();

        var scheduledEmployees = await dbContext.Schedules
            .AsNoTracking()
            .Where(schedule => schedule.WorkDate >= firstDayOfMonth && schedule.WorkDate < firstDayOfNextMonth)
            .OrderBy(schedule => schedule.WorkDate)
            .ThenBy(schedule => schedule.Employee.Name)
            .Select(schedule => new
            {
                schedule.WorkDate,
                schedule.EmployeeId,
                EmployeeName = schedule.Employee.Name
            })
            .ToListAsync(cancellationToken);

        var employeesByDate = scheduledEmployees
            .GroupBy(schedule => schedule.WorkDate)
            .ToDictionary(
                group => group.Key,
                group => (IReadOnlyList<ScheduledEmployeeDto>)group
                    .Select(schedule => new ScheduledEmployeeDto(schedule.EmployeeId, schedule.EmployeeName))
                    .ToList());

        var dailySchedules = Enumerable.Range(0, DateTime.DaysInMonth(year, month))
            .Select(dayOffset => firstDayOfMonth.AddDays(dayOffset))
            .Select(workDate => new DailyScheduleDto(
                workDate,
                employeesByDate.GetValueOrDefault(workDate, [])))
            .ToList();

        return new StatisticsDashboardDto(year, month, employeeWorkdays, monthlyRanking, dailySchedules);
    }
}