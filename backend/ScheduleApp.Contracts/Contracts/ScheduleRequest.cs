namespace ScheduleApp.Contracts.Contracts;

public record CreateScheduleRequest(
    DateOnly StartDate,
    int Days,
    DayScheduleConfig Default,
    List<DayOverrideConfig> Overrides
);

public record DayScheduleConfig(
    TimeOnly Start,
    TimeOnly End,
    int SlotDurationMinutes
);

public record DayOverrideConfig(
    DayOfWeek DayOfWeek,
    TimeOnly? Start,
    TimeOnly? End,
    int? SlotDurationMinutes,
    bool IsDayOff
);
