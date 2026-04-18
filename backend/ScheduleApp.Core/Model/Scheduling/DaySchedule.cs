using CSharpFunctionalExtensions;

namespace ScheduleApp.Core.Model.Scheduling;

public class DaySchedule
{
    public DateOnly Date { get; private set; }
    public TimeOnly Start { get; private set; }
    public TimeOnly End { get; private set; }
    public TimeSpan SlotDuration { get; private set; }

    public IReadOnlyCollection<Slot> DaySlots { get; private set; } = [];

    private DaySchedule(DateOnly date, TimeOnly start, TimeOnly end, TimeSpan slotDuration)
    {
        Date = date;
        Start = start;
        End = end;
        SlotDuration = slotDuration;
    }

    public static Result<DaySchedule> Create(DateOnly date, TimeOnly start, TimeOnly end, TimeSpan slotDuration)
    {
        var daySchedule = new DaySchedule(date, start, end, slotDuration);
        var schedulingResult = daySchedule.GenerateSlots();
        if (schedulingResult.IsFailure)
            return Result.Failure<DaySchedule>(schedulingResult.Error);
        return Result.Success(daySchedule);
    }

    private Result GenerateSlots()
    {
        if (End <= Start)
        {
            DaySlots = [];
            return Result.Success();
        }
            
        var slots = new List<Slot>();
        var currentTime = Start;
            
        while (currentTime.Add(SlotDuration) <= End)
        {
            slots.Add(new Slot(currentTime, currentTime.Add(SlotDuration), Status.Available));
                
            currentTime = currentTime.Add(SlotDuration);
        }
    
        DaySlots = slots;
            
        return Result.Success();
    }
}