using CSharpFunctionalExtensions;
using ScheduleApp.Contracts.Contracts;
using ScheduleApp.Core.Model.Scheduling;
using ScheduleApp.PostgreSql.Repositories;

namespace ScheduleApp.Application.Services;

public interface IScheduleService
{
    Task<Result> CreateSchedule(CreateScheduleRequest request);
    Task<Result<WeeklySchedule>> GetSchedule(Guid id);
}

public class ScheduleService : IScheduleService
{
    private readonly IScheduleRepository _repository;

    public ScheduleService(IScheduleRepository repository)
    {
        _repository = repository;
    }
    public async Task<Result> CreateSchedule(CreateScheduleRequest request)
    {
        var overrides = request.Overrides
            .Select(x => new DayOverride
            {
                DayOfWeek = x.DayOfWeek,
                Start = x.Start,
                End = x.End,
                SlotDuration = x.SlotDurationMinutes is null
                    ? null
                    : TimeSpan.FromMinutes(x.SlotDurationMinutes.Value),
                IsDayOff = x.IsDayOff
            })
            .ToList();

        var result = WeeklySchedule.Create(
            request.StartDate,
            request.Days,
            overrides,
            date => (
                request.Default.Start,
                request.Default.End,
                TimeSpan.FromMinutes(request.Default.SlotDurationMinutes)
            ));

        if (result.IsFailure)
            return Result.Failure<WeeklySchedule>(result.Error);

        var saveResult = await _repository.AddAsync(result.Value);
        if(saveResult.IsFailure)
            return Result.Failure(saveResult.Error);

        return Result.Success();
    }

    public async Task<Result<WeeklySchedule>> GetSchedule(Guid id)
    {
        var result = await _repository.GetAsync(id);
        if(result is null)
            return Result.Failure<WeeklySchedule>("Schedule not found");
        return Result.Success(result);
    }
}