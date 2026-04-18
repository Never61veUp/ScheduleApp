namespace ScheduleApp.API.Contracts;

public record ScheduleResponse(
    DateOnly Start,
    DateOnly End,
    IReadOnlyCollection<DayScheduleResponse> Days
);

public record DayScheduleResponse(
    DateOnly Date,
    TimeOnly Start,
    TimeOnly End,
    IReadOnlyCollection<SlotResponse> Slots
);

public record SlotResponse(
    TimeOnly Start,
    TimeOnly End,
    string Status
);