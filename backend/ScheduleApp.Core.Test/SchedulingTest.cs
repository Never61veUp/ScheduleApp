using ScheduleApp.Core.Model;
using ScheduleApp.Core.Model.Scheduling;

namespace ScheduleApp.Core.Test;

public class SchedulingTest
{
    [Test]
    [TestCase()]
    public void DayScheduling()
    {
        var dateOnly = new DateOnly(2024, 6, 1);
        var start = new TimeOnly(9, 0);
        var end = new TimeOnly(17, 0);
        var slotDuration = TimeSpan.FromMinutes(30);
        
        var dayScheduleResult = DaySchedule.Create(dateOnly, start, end, slotDuration);
        Assert.That(dayScheduleResult.IsSuccess);
        Assert.That(dayScheduleResult.Value.DaySlots.Count, Is.EqualTo(16));
    }
    
    [Test]
    public void WeekScheduling_WithOverrides_NoRequest()
    {
        // Arrange
        var startDate = new DateOnly(2026, 4, 18);
        var days = 7;

        var overrides = new List<DayOverride>
        {
            new()
            {
                DayOfWeek = DayOfWeek.Saturday,
                Start = new TimeOnly(10, 0),
                End = new TimeOnly(14, 0),
                SlotDuration = TimeSpan.FromMinutes(60),
                IsDayOff = false
            },
            new()
            {
                DayOfWeek = DayOfWeek.Sunday,
                IsDayOff = true
            }
        };
        
        // Act
        var result = WeeklySchedule.Create(
            startDate,
            days,
            overrides,
            date =>
            {
                return (
                    new TimeOnly(9, 0),
                    new TimeOnly(18, 0),
                    TimeSpan.FromMinutes(30)
                );
            });

        // Assert result
        Assert.That(result.IsSuccess);

        var schedule = result.Value;

        Assert.That(schedule.DaySchedules.Count, Is.EqualTo(7));
        
        DaySchedule Get(DayOfWeek day) =>
            schedule.DaySchedules.First(d => d.Date.DayOfWeek == day);
        
        var saturday = schedule.DaySchedules.First(x => x.Date.DayOfWeek == DayOfWeek.Saturday);

        Assert.That(saturday.Start, Is.EqualTo(new TimeOnly(10, 0)));
        Assert.That(saturday.SlotDuration, Is.EqualTo(TimeSpan.FromMinutes(60)));

        var sunday = schedule.DaySchedules.First(x => x.Date.DayOfWeek == DayOfWeek.Sunday);

        Assert.That(sunday.DaySlots.Count, Is.EqualTo(0));
    }
}