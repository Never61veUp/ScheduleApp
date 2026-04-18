using System.Collections.Generic;
using System.Linq;
using CSharpFunctionalExtensions;

namespace ScheduleApp.Core.Model.Scheduling;

public class WeeklySchedule
{
    public DateOnly Start { get; private set; }
    public DateOnly End { get; private set; }
    public IReadOnlyCollection<DaySchedule> DaySchedules { get; private set; } = [];

    private WeeklySchedule(DateOnly start, int days)
    {
        Start = start;
        End = start.AddDays(days);
    }
    
    public static Result<WeeklySchedule> Create(
        DateOnly start,
        int days,
        Func<DateOnly, (TimeOnly start, TimeOnly end, TimeSpan slotDuration)> dayConfig)
    {
        var schedules = new WeeklySchedule(start, days);
        var result = schedules.GenerateDays(start, days, dayConfig);
        
        if(result.IsFailure)
            return Result.Failure<WeeklySchedule>(result.Error);
        
        return Result.Success(schedules);
    }

    private Result GenerateDays(DateOnly start,
        int days, Func<DateOnly, (TimeOnly start, TimeOnly end, TimeSpan slotDuration)> dayConfig)
    {
        var schedules = new List<DaySchedule>();

        for (var date = start; date < start.AddDays(days); date = date.AddDays(1))
        {
            var (dayStart, dayEnd, slotDuration) = dayConfig(date);

            var result = DaySchedule.Create(date, dayStart, dayEnd, slotDuration);

            if (result.IsFailure)
                return Result.Failure(result.Error);

            schedules.Add(result.Value);
        }

        DaySchedules = schedules;
        return Result.Success();
    }
}