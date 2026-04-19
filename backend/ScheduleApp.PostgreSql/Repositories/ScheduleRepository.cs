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

    public async Task<Result> UpdateAsync(WeeklySchedule schedule)
    {
        _context.Update(schedule);
        
        if(await _context.SaveChangesAsync() < 1)
            return Result.Failure("Failed to update schedule.");
        return Result.Success();
    }
    
    public async Task<Result<DaySchedule>> GetDayScheduleAsync(Guid id)
    {
        var daySchedule = await _context.DaySchedules
            .Include(x => x.DaySlots)
            .FirstOrDefaultAsync(x => x.Id == id);
        
        if (daySchedule is null)
            return Result.Failure<DaySchedule>("Day schedule not found.");
        
        return Result.Success(daySchedule);
    }
    
    public async Task<Result<Slot>> GetSlotAsync(Guid id)
    {
        var slot = await _context.Slots
            .FirstOrDefaultAsync(x => x.Id == id);
        return slot ?? Result.Failure<Slot>("Slot not found.");
    }
    
    public async Task<Result> UpdateAsync(Slot slot)
    {
        _context.Update(slot);
        
        if(await _context.SaveChangesAsync() < 1)
            return Result.Failure("Failed to update slot.");
        return Result.Success();
    }
}

public interface IScheduleRepository
{
    Task<WeeklySchedule?> GetAsync(Guid id);
    Task<Result> AddAsync(WeeklySchedule schedule);
    Task<Result> UpdateAsync(WeeklySchedule schedule);
    Task<Result<DaySchedule>> GetDayScheduleAsync(Guid id);
    Task<Result<Slot>> GetSlotAsync(Guid id);
    Task<Result> UpdateAsync(Slot slot);
}