using Microsoft.AspNetCore.Mvc;
using ShiftPlanner.Api.Services;
using ShiftPlanner.Api.ViewModels;

namespace ShiftPlanner.Api.Controllers;

[ApiController]
[Route("api/employees/{employeeId:int}/schedules")]
public class EmployeeSchedulesController(IScheduleService scheduleService) : ControllerBase
{
    [HttpGet("calendar/next-month")]
    [ProducesResponseType(typeof(IReadOnlyList<ScheduleCalendarDayDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<ScheduleCalendarDayDto>>> GetNextMonthCalendarAsync(
        int employeeId,
        CancellationToken cancellationToken)
    {
        var result = await scheduleService.GetNextMonthCalendarAsync(employeeId, cancellationToken);

        if (!result.Succeeded)
        {
            return NotFound(new ApiErrorResponse(result.Code, result.Message));
        }

        return Ok(result.Days);
    }

    [HttpGet("next-month")]
    [ProducesResponseType(typeof(IReadOnlyList<ScheduleResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<ScheduleResponse>>> GetNextMonthAsync(
        int employeeId,
        CancellationToken cancellationToken)
    {
        var result = await scheduleService.GetNextMonthAsync(employeeId, cancellationToken);

        if (!result.Succeeded)
        {
            return NotFound(new ApiErrorResponse(result.Code, result.Message));
        }

        return Ok(result.Schedules.Select(ToResponse).ToList());
    }

    [HttpPost]
    [ProducesResponseType(typeof(ScheduleResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateAsync(
        int employeeId,
        CreateScheduleRequest request,
        CancellationToken cancellationToken)
    {
        if (request.WorkDate is null)
        {
            return BadRequest(new ApiErrorResponse("InvalidWorkDate", "必須提供有效的排班日期。"));
        }

        var result = await scheduleService.CreateAsync(employeeId, request.WorkDate.Value, cancellationToken);

        if (!result.Succeeded)
        {
            return ToErrorResult(result.Code, result.Message);
        }

        var response = ToResponse(result.Schedule!);
        return CreatedAtAction(nameof(GetNextMonthAsync), new { employeeId }, response);
    }

    [HttpDelete("{scheduleId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CancelAsync(
        int employeeId,
        int scheduleId,
        CancellationToken cancellationToken)
    {
        var result = await scheduleService.CancelAsync(employeeId, scheduleId, cancellationToken);

        if (!result.Succeeded)
        {
            return ToErrorResult(result.Code, result.Message);
        }

        return NoContent();
    }

    [HttpGet("current-month/count")]
    [ProducesResponseType(typeof(ScheduleCountResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ScheduleCountResponse>> GetCurrentMonthCountAsync(
        int employeeId,
        CancellationToken cancellationToken)
    {
        var result = await scheduleService.GetCurrentMonthCountAsync(employeeId, cancellationToken);

        if (!result.Succeeded)
        {
            return NotFound(new ApiErrorResponse(result.Code, result.Message));
        }

        return Ok(new ScheduleCountResponse(result.ScheduledDays));
    }

    private static ScheduleResponse ToResponse(Models.Schedule schedule)
    {
        return new ScheduleResponse(schedule.Id, schedule.WorkDate, schedule.CreatedAt);
    }

    private IActionResult ToErrorResult(string code, string message)
    {
        var error = new ApiErrorResponse(code, message);

        return code switch
        {
            "EmployeeNotFound" => NotFound(error),
            "InvalidMonth" or "Saturday" or "Sunday" or "Holiday" => BadRequest(error),
            _ => Conflict(error)
        };
    }
}