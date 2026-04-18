using CSharpFunctionalExtensions;

namespace ScheduleApp.Core.Model.Scheduling;

public class DayOverride : Entity<Guid>
{
    public DayOfWeek DayOfWeek { get; set; }

    public TimeOnly? Start { get; set; }
    public TimeOnly? End { get; set; }
    public TimeSpan? SlotDuration { get; set; }

    public bool IsDayOff { get; set; }
}