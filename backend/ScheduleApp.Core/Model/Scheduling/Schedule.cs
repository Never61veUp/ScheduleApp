using System.Collections.Generic;
using System.Linq;
using CSharpFunctionalExtensions;

namespace ScheduleApp.Core.Model.Scheduling;

public class WeeklySchedule : Entity<Guid>
{
    public DateOnly Start { get; private set; }
    public DateOnly End { get; private set; }
    private List<DaySchedule> _daySchedules = new();
    public IReadOnlyCollection<DaySchedule> DaySchedules => _daySchedules;

    private List<DayOverride> _overrides = [];
    public IReadOnlyCollection<DayOverride> Overrides => _overrides;

    private WeeklySchedule(DateOnly start, int days)
    {
        Start = start;
        End = start.AddDays(days);
    }
    private WeeklySchedule() { }
    
    public static Result<WeeklySchedule> Create(
        DateOnly start,
        int days,
        List<DayOverride> overrides,
        Func<DateOnly, (TimeOnly start, TimeOnly end, TimeSpan slotDuration)> dayConfig)
    {
        var schedules = new WeeklySchedule(start, days);
        schedules._overrides = overrides;
        var result = schedules.GenerateDays(start, days, overrides, dayConfig);
        
        if(result.IsFailure)
            return Result.Failure<WeeklySchedule>(result.Error);
        
        return Result.Success(schedules);
    }

    private Result GenerateDays(
        DateOnly start,
        int days,
        List<DayOverride> overrides,
        Func<DateOnly, (TimeOnly start, TimeOnly end, TimeSpan slotDuration)> defaultConfig)
    {
        var schedules = new List<DaySchedule>();

        for (var date = start; date < start.AddDays(days); date = date.AddDays(1))
        {
            var overrideDay = overrides.FirstOrDefault(x => x.DayOfWeek == date.DayOfWeek);

            if (overrideDay?.IsDayOff == true)
            {
                schedules.Add(DaySchedule.Create(date, TimeOnly.MinValue, TimeOnly.MinValue, TimeSpan.Zero).Value);
                continue;
            }
                

            var config = overrideDay is null
                ? defaultConfig(date)
                : (
                    overrideDay.Start ?? defaultConfig(date).start,
                    overrideDay.End ?? defaultConfig(date).end,
                    overrideDay.SlotDuration ?? defaultConfig(date).slotDuration
                );

            var result = DaySchedule.Create(date, config.Item1, config.Item2, config.Item3);

            if (result.IsFailure)
                return Result.Failure(result.Error);

            schedules.Add(result.Value);
        }

        _daySchedules = schedules;
        return Result.Success();
    }}