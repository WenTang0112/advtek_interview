using Microsoft.EntityFrameworkCore;
using ShiftPlanner.Api.Data;
using ShiftPlanner.Api.Models;

namespace ShiftPlanner.Api.Services;

public class ScheduleService(
    ApplicationDbContext dbContext,
    ICalendarService calendarService,
    TimeProvider timeProvider) : IScheduleService
{
    private const int MaximumEmployeesPerDay = 2;
    private const int MaximumWorkdaysPerEmployeePerMonth = 15;

    public async Task<ScheduleQueryResult> GetNextMonthAsync(
        int employeeId,
        CancellationToken cancellationToken = default)
    {
        if (!await EmployeeExistsAsync(employeeId, cancellationToken))
        {
            return ScheduleQueryResult.Failure("EmployeeNotFound", "找不到指定員工。");
        }

        var today = DateOnly.FromDateTime(timeProvider.GetLocalNow().DateTime);
        var firstDayOfNextMonth = new DateOnly(today.Year, today.Month, 1).AddMonths(1);
        var firstDayOfFollowingMonth = firstDayOfNextMonth.AddMonths(1);
        var schedules = await dbContext.Schedules
            .AsNoTracking()
            .Where(schedule => schedule.EmployeeId == employeeId
                && schedule.WorkDate >= firstDayOfNextMonth
                && schedule.WorkDate < firstDayOfFollowingMonth)
            .OrderBy(schedule => schedule.WorkDate)
            .ToListAsync(cancellationToken);

        return ScheduleQueryResult.Success(schedules);
    }

    public async Task<ScheduleOperationResult> CreateAsync(
        int employeeId,
        DateOnly workDate,
        CancellationToken cancellationToken = default)
    {
        if (!calendarService.IsSchedulableMonth(workDate))
        {
            return ScheduleOperationResult.Failure("InvalidMonth", "僅能排定下個月的班表。");
        }

        if (workDate.DayOfWeek == DayOfWeek.Saturday)
        {
            return ScheduleOperationResult.Failure("Saturday", "星期六不可排班。");
        }

        if (workDate.DayOfWeek == DayOfWeek.Sunday)
        {
            return ScheduleOperationResult.Failure("Sunday", "星期日不可排班。");
        }

        if (calendarService.IsHoliday(workDate))
        {
            return ScheduleOperationResult.Failure("Holiday", "國定假日不可排班。");
        }

        if (!await EmployeeExistsAsync(employeeId, cancellationToken))
        {
            return ScheduleOperationResult.Failure("EmployeeNotFound", "找不到指定員工。");
        }

        if (await dbContext.Schedules.AnyAsync(
                schedule => schedule.EmployeeId == employeeId && schedule.WorkDate == workDate,
                cancellationToken))
        {
            return ScheduleOperationResult.Failure("DuplicateSchedule", "該員工當日已有排班。 ");
        }

        var employeesScheduledThatDay = await dbContext.Schedules.CountAsync(
            schedule => schedule.WorkDate == workDate,
            cancellationToken);

        if (employeesScheduledThatDay >= MaximumEmployeesPerDay)
        {
            return ScheduleOperationResult.Failure("DailyCapacityReached", "當日最多可安排兩名員工。 ");
        }

        var firstDayOfMonth = new DateOnly(workDate.Year, workDate.Month, 1);
        var firstDayOfNextMonth = firstDayOfMonth.AddMonths(1);
        var employeeSchedulesThisMonth = await dbContext.Schedules.CountAsync(
            schedule => schedule.EmployeeId == employeeId
                && schedule.WorkDate >= firstDayOfMonth
                && schedule.WorkDate < firstDayOfNextMonth,
            cancellationToken);

        if (employeeSchedulesThisMonth >= MaximumWorkdaysPerEmployeePerMonth)
        {
            return ScheduleOperationResult.Failure("MonthlyLimitReached", "每名員工每月最多可排 15 天。 ");
        }

        var schedule = new Schedule
        {
            EmployeeId = employeeId,
            WorkDate = workDate,
            CreatedAt = timeProvider.GetUtcNow().UtcDateTime
        };

        dbContext.Schedules.Add(schedule);

        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException)
        {
            return ScheduleOperationResult.Failure("ScheduleConflict", "排班資料已變更，請重新確認後再試。 ");
        }

        return ScheduleOperationResult.Success(schedule);
    }

    public async Task<ScheduleOperationResult> CancelAsync(
        int employeeId,
        int scheduleId,
        CancellationToken cancellationToken = default)
    {
        if (!await EmployeeExistsAsync(employeeId, cancellationToken))
        {
            return ScheduleOperationResult.Failure("EmployeeNotFound", "找不到指定員工。");
        }

        var schedule = await dbContext.Schedules.SingleOrDefaultAsync(
            item => item.Id == scheduleId && item.EmployeeId == employeeId,
            cancellationToken);

        if (schedule is null)
        {
            return ScheduleOperationResult.Failure("ScheduleNotFound", "找不到指定的個人排班。");
        }

        dbContext.Schedules.Remove(schedule);

        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException)
        {
            return ScheduleOperationResult.Failure("ScheduleConflict", "排班資料已變更，請重新確認後再試。 ");
        }

        return ScheduleOperationResult.Success(schedule, "排班已取消。");
    }

    public async Task<ScheduleCountResult> GetCurrentMonthCountAsync(
        int employeeId,
        CancellationToken cancellationToken = default)
    {
        if (!await EmployeeExistsAsync(employeeId, cancellationToken))
        {
            return ScheduleCountResult.Failure("EmployeeNotFound", "找不到指定員工。");
        }

        var today = DateOnly.FromDateTime(timeProvider.GetLocalNow().DateTime);
        var firstDayOfCurrentMonth = new DateOnly(today.Year, today.Month, 1);
        var firstDayOfNextMonth = firstDayOfCurrentMonth.AddMonths(1);
        var scheduledDays = await dbContext.Schedules.CountAsync(
            schedule => schedule.EmployeeId == employeeId
                && schedule.WorkDate >= firstDayOfCurrentMonth
                && schedule.WorkDate < firstDayOfNextMonth,
            cancellationToken);

        return ScheduleCountResult.Success(scheduledDays);
    }

    private Task<bool> EmployeeExistsAsync(int employeeId, CancellationToken cancellationToken)
    {
        return dbContext.Employees.AnyAsync(employee => employee.Id == employeeId, cancellationToken);
    }
}