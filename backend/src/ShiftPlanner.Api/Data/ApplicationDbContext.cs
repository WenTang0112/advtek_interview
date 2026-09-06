using Microsoft.EntityFrameworkCore;
using ShiftPlanner.Api.Models;

namespace ShiftPlanner.Api.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Employee> Employees => Set<Employee>();

    public DbSet<Schedule> Schedules => Set<Schedule>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.Property(employee => employee.Name).HasMaxLength(100).IsRequired();
            entity.Property(employee => employee.Email).HasMaxLength(256).IsRequired();

            entity.HasMany(employee => employee.Schedules)
                .WithOne(schedule => schedule.Employee)
                .HasForeignKey(schedule => schedule.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Schedule>(entity =>
        {
            entity.Property(schedule => schedule.WorkDate).IsRequired();
            entity.Property(schedule => schedule.CreatedAt).IsRequired();

            entity.HasIndex(schedule => new { schedule.EmployeeId, schedule.WorkDate })
                .IsUnique();
        });
    }
}