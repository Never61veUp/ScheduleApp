using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScheduleApp.Core.Model.Scheduling;

namespace ScheduleApp.PostgreSql.Configuration;

public class OverrideConfig : IEntityTypeConfiguration<DayOverride>
{
    public void Configure(EntityTypeBuilder<DayOverride> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.DayOfWeek);
        builder.Property(x => x.Start);
        builder.Property(x => x.End);
        builder.Property(x => x.SlotDuration);
        builder.Property(x => x.IsDayOff);
    }
}