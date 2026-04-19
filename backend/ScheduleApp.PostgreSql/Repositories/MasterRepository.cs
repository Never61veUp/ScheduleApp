using Microsoft.EntityFrameworkCore;
using ScheduleApp.Core.Model.User;
using ScheduleApp.PostgreSql.Model;

namespace ScheduleApp.PostgreSql.Repositories;

public class MasterRepository : IMasterRepository
{
    private readonly AppDbContext _db;

    public MasterRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<Master?> GetByTelegramId(long telegramId)
    {
        var entity = await _db.MasterEntities
            .FirstOrDefaultAsync(x => x.TelegramId == telegramId);

        if (entity == null) return null;

        return MapToDomain(entity);
    }

    public async Task Add(Master master)
    {
        var entity = MapToEntity(master);
        await _db.MasterEntities.AddAsync(entity);
        await _db.SaveChangesAsync();
    }
    
    public async Task Update(Master master)
    {
        var entity = MapToEntity(master);
        _db.Update(entity);
        await _db.SaveChangesAsync();
    }

    private Master MapToDomain(MasterEntity e)
    {
        var m = new Master(e.TelegramId);

        if (!string.IsNullOrWhiteSpace(e.LocationName))
            m.UpdateLocation(e.LocationName, e.LocationUrl);

        m.UpdateProfile(e.AvatarUrl, e.Description);

        return m;
    }

    private MasterEntity MapToEntity(Master m)
    {
        return new MasterEntity
        {
            Id = m.Id,
            TelegramId = m.TelegramId,
            AvatarUrl = m.AvatarUrl,
            Description = m.Description,
            LocationName = m.Location?.Name,
            LocationUrl = m.Location?.MapUrl
        };
    }
}

public interface IMasterRepository
{
    Task Add(Master master);
    Task<Master?> GetByTelegramId(long telegramId);
    Task Update(Master master);
}