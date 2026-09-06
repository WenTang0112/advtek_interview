using System.ComponentModel.DataAnnotations;

namespace ShiftPlanner.Api.ViewModels;

public sealed class CreateScheduleRequest
{
    [Required]
    public DateOnly? WorkDate { get; init; }
}