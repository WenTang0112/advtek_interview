using Microsoft.AspNetCore.Mvc;
using ShiftPlanner.Api.Services;
using ShiftPlanner.Api.ViewModels;

namespace ShiftPlanner.Api.Controllers;

[ApiController]
[Route("api/boss/dashboard")]
public class BossDashboardController(IStatisticsService statisticsService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(StatisticsDashboardDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<StatisticsDashboardDto>> GetAsync(
        [FromQuery] DashboardQueryRequest request,
        CancellationToken cancellationToken)
    {
        var dashboard = await statisticsService.GetDashboardAsync(
            request.Year,
            request.Month,
            cancellationToken);

        return Ok(dashboard);
    }
}