namespace ShiftPlanner.Api.ViewModels;

public sealed record ScheduleResponse(int Id, DateOnly WorkDate, DateTime CreatedAt);