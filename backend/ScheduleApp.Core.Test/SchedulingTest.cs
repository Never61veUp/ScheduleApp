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
        var startDate = new DateOnly(2026, 4, 18);
        var days = 7;

        var result = WeeklySchedule.Create(
            startDate,
            days,
            date =>
            {
                if (date.DayOfWeek == DayOfWeek.Sunday)
                    return (new TimeOnly(0, 0), new TimeOnly(0, 0), TimeSpan.FromMinutes(30));
                
                if (date.DayOfWeek == DayOfWeek.Saturday)
                    return (new TimeOnly(10, 0), new TimeOnly(14, 0), TimeSpan.FromMinutes(60));
                
                return (new TimeOnly(9, 0), new TimeOnly(18, 0), TimeSpan.FromMinutes(30));
            });

        Assert.That(result.IsSuccess);

        var schedule = result.Value;

        Assert.That(schedule.DaySchedules, Has.Count.EqualTo(7));
        
        var saturday = schedule.DaySchedules
            .First(d => d.Date.DayOfWeek == DayOfWeek.Saturday);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(saturday.Start, Is.EqualTo(new TimeOnly(10, 0)));
            Assert.That(saturday.End, Is.EqualTo(new TimeOnly(14, 0)));
            Assert.That(saturday.SlotDuration, Is.EqualTo(TimeSpan.FromMinutes(60)));
        }
        
        var sunday = schedule.DaySchedules
            .First(d => d.Date.DayOfWeek == DayOfWeek.Sunday);

        Assert.That(sunday.DaySlots.Count, Is.EqualTo(0));

        // 🔍 Будний день
        var monday = schedule.DaySchedules
            .First(d => d.Date.DayOfWeek == DayOfWeek.Monday);

        Assert.That(monday.Start, Is.EqualTo(new TimeOnly(9, 0)));
        Assert.That(monday.End, Is.EqualTo(new TimeOnly(18, 0)));
    }
}