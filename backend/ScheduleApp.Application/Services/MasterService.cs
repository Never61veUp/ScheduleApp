using ScheduleApp.Core.Model.User;
using ScheduleApp.PostgreSql.Repositories;

namespace ScheduleApp.Application.Services;

public interface IMasterService
{
    Task UpdateGeo(long telegramId, string name, string url);
    Task UpdateProfile(long telegramId, string? avatar, string? desc);
    Task Login(long telegramId);
    Task<Master?> GetByTelegramId(long telegramId);
}

public class MasterService : IMasterService
{
    private readonly IMasterRepository _repo;

    public MasterService(IMasterRepository repo)
    {
        _repo = repo;
    }
    
    public async Task UpdateGeo(long telegramId, string name, string url)
    {
        var master = await _repo.GetByTelegramId(telegramId)
                     ?? throw new Exception("Master not found");

        master.UpdateLocation(name, url);
        await _repo.Update(master);
    }
    
    public async Task UpdateProfile(long telegramId, string? avatar, string? desc)
    {
        var master = await _repo.GetByTelegramId(telegramId)
                     ?? throw new Exception("Master not found");

        master.UpdateProfile(avatar, desc);
        await _repo.Update(master);
    }
    
    public async Task Login(long telegramId)
    {
        var master = await _repo.GetByTelegramId(telegramId);

        if (master == null)
        {
            master = new Master(telegramId);
            await _repo.Add(master);
        }
    }
    
    public async Task<Master?> GetByTelegramId(long telegramId)
    {
        return await _repo.GetByTelegramId(telegramId);
    }
}