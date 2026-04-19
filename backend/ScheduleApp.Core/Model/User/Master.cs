using CSharpFunctionalExtensions;

namespace ScheduleApp.Core.Model.User;

public class Master : Entity<Guid>
{
    public long TelegramId { get; private set; }

    public string? AvatarUrl { get; private set; }
    public string? Description { get; private set; }

    public Location? Location { get; private set; }

    private Master() { }

    public Master(long telegramId)
    {
        Id = Guid.NewGuid();
        TelegramId = telegramId;
    }

    public void UpdateProfile(string? avatarUrl, string? description)
    {
        AvatarUrl = avatarUrl;
        Description = description;
    }

    public void UpdateLocation(string name, string url)
    {
        Location = new Location(name, url);
    }
}
