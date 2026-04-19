using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using ScheduleApp.Core.Model.Scheduling;

namespace ScheduleApp.PostgreSql.Repositories;

public class ScheduleRepository : IScheduleRepository
{
    private readonly AppDbContext _context;

    public ScheduleRepository(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<WeeklySchedule?> GetAsync(Guid id)
    {
        return await _context.Schedules
            .Include(x => x.DaySchedules)
            .ThenInclude(d => d.DaySlots)
            .Include(x => x.Overrides)
            .FirstOrDefaultAsync(x => x.Id == id);
    }
    
    public async Task<Result> AddAsync(WeeklySchedule schedule)
    {
        await _context.Schedules.AddAsync(schedule);
            
        if(await _context.SaveChangesAsync() < 1)
            return Result.Failure("Failed to add schedule.");
        return Result.Success();
    }
}

public interface IScheduleRepository
{
    Task<WeeklySchedule?> GetAsync(Guid id);
    Task<Result> AddAsync(WeeklySchedule schedule);
}