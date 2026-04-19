using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScheduleApp.Core.Model.Scheduling;

namespace ScheduleApp.PostgreSql.Configuration;

public class SlotConfig : IEntityTypeConfiguration<Slot>
{
    public void Configure(EntityTypeBuilder<Slot> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Start);
        builder.Property(x => x.End);
        builder.Property(x => x.Status);
    }
}