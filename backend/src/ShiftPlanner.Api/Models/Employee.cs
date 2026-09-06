namespace ShiftPlanner.Api.Models;

public class Employee
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public EmployeeRole Role { get; set; }

    public ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
}