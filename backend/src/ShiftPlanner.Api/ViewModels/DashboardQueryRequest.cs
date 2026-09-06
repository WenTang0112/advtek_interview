using System.ComponentModel.DataAnnotations;

namespace ShiftPlanner.Api.ViewModels;

public sealed class DashboardQueryRequest
{
    [Range(1, 9999, ErrorMessage = "年份必須介於 1 至 9999。")]
    public int Year { get; init; }

    [Range(1, 12, ErrorMessage = "月份必須介於 1 至 12。")]
    public int Month { get; init; }
}