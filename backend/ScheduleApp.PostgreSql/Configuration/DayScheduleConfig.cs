using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScheduleApp.Core.Model.Scheduling;

namespace ScheduleApp.PostgreSql.Configuration;

public class DayScheduleConfig : IEntityTypeConfiguration<DaySchedule>
{
    public void Configure(EntityTypeBuilder<DaySchedule> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Date);
        builder.Property(x => x.Start);
        builder.Property(x => x.End);
        builder.Property(x => x.SlotDuration);
        builder.Property(x => x.IsDayOff);

        builder.HasMany(x => x.DaySlots)
            .WithOne()
            .OnDelete(DeleteBehavior.Cascade);
    }
}