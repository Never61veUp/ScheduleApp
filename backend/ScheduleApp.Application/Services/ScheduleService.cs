using CSharpFunctionalExtensions;
using ScheduleApp.Contracts.Contracts;
using ScheduleApp.Core.Model.Scheduling;

namespace ScheduleApp.Application.Services;

public interface IScheduleService
{
    Result<WeeklySchedule> CreateSchedule(CreateScheduleRequest request);
    Result<WeeklySchedule> GetSchedule();
}

public class ScheduleService : IScheduleService
{
    private Dictionary<int, WeeklySchedule> _storage = new Dictionary<int, WeeklySchedule>();
    public Result<WeeklySchedule> CreateSchedule(CreateScheduleRequest request)
    {
        var result = WeeklySchedule.Create(
            request.StartDate,
            request.Days,
            date =>
            {
                var overrideDay = request.Overrides
                    .FirstOrDefault(o => o.DayOfWeek == date.DayOfWeek);
                
                if (overrideDay?.IsDayOff == true)
                    return (TimeOnly.MinValue, TimeOnly.MinValue, TimeSpan.Zero);

                var config = overrideDay is null
                    ? request.Default
                    : new DayScheduleConfig(
                        overrideDay.Start ?? request.Default.Start,
                        overrideDay.End ?? request.Default.End,
                        overrideDay.SlotDurationMinutes ?? request.Default.SlotDurationMinutes
                    );

                return (
                    config.Start,
                    config.End,
                    TimeSpan.FromMinutes(config.SlotDurationMinutes)
                );
            });
        _storage.Add(1, result.Value);
        return result;
    }

    public Result<WeeklySchedule> GetSchedule()
    {
        if (!_storage.TryGetValue(1, out var schedule))
            return Result.Failure<WeeklySchedule>("Schedule not created");

        return Result.Success(schedule);
    }

    public void Clear()
    {
        _storage.Clear();
    }
}