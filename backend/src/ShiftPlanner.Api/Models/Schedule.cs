namespace ShiftPlanner.Api.Models;

public class Schedule
{
    public int Id { get; set; }

    public int EmployeeId { get; set; }

    public DateOnly WorkDate { get; set; }

    public DateTime CreatedAt { get; set; }

    public Employee Employee { get; set; } = null!;
}