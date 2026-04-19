using Microsoft.EntityFrameworkCore;
using ScheduleApp.Core.Model.Scheduling;
using ScheduleApp.PostgreSql.Configuration;

namespace ScheduleApp.PostgreSql;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<WeeklySchedule> Schedules => Set<WeeklySchedule>();
    public DbSet<DaySchedule> DaySchedules => Set<DaySchedule>();
    public DbSet<Slot> Slots => Set<Slot>();
    public DbSet<DayOverride> Overrides => Set<DayOverride>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ScheduleConfig());
        modelBuilder.ApplyConfiguration(new DayScheduleConfig());
        modelBuilder.ApplyConfiguration(new SlotConfig());
        modelBuilder.ApplyConfiguration(new OverrideConfig());
    }
}