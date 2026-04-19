using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScheduleApp.Core.Model.Scheduling;

namespace ScheduleApp.PostgreSql.Configuration;

public class ScheduleConfig : IEntityTypeConfiguration<WeeklySchedule>
{
    public void Configure(EntityTypeBuilder<WeeklySchedule> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Start);
        builder.Property(x => x.End);

        builder.HasMany(x => x.DaySchedules)
            .WithOne()
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Overrides)
            .WithOne()
            .OnDelete(DeleteBehavior.Cascade);
    }
}