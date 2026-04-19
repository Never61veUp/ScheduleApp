using ScheduleApp.API.Contracts;
using ScheduleApp.Core.Model.Scheduling;

namespace ScheduleApp.Application.Extension;

public static class ScheduleMapper
{
    public static ScheduleResponse ToResponse(this WeeklySchedule schedule)
    {
        return new ScheduleResponse(
            schedule.Start,
            schedule.End,
            schedule.DaySchedules.Select(d => new DayScheduleResponse(
                d.Date,
                d.Start,
                d.End,
                d.DaySlots.Select(s => new SlotResponse(
                    s.Start,
                    s.End,
                    s.Status.ToString()
                )).ToList()
            )).ToList()
        );
    }
}