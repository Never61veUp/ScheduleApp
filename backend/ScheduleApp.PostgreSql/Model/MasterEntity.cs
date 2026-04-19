namespace ScheduleApp.PostgreSql.Model;

public class MasterEntity
{
    public Guid Id { get; set; }
    public long TelegramId { get; set; }

    public string? AvatarUrl { get; set; }
    public string? Description { get; set; }

    public string? LocationName { get; set; }
    public string? LocationUrl { get; set; }
}