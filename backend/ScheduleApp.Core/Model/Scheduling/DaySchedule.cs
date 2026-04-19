using CSharpFunctionalExtensions;

namespace ScheduleApp.Core.Model.Scheduling;

public class DaySchedule : Entity<Guid>
{
    public DateOnly Date { get; private set; }
    public TimeOnly Start { get; private set; }
    public TimeOnly End { get; private set; }
    public TimeSpan SlotDuration { get; private set; }

    public bool IsDayOff
    {
        get => field;
        private set;
    }

    private List<Slot> _daySlots = [];

    public IReadOnlyCollection<Slot> DaySlots => _daySlots;

    private DaySchedule(DateOnly date, TimeOnly start, TimeOnly end, TimeSpan slotDuration)
    {
        Date = date;
        Start = start;
        End = end;
        SlotDuration = slotDuration;
        IsDayOff = DaySlots.Count == 0;
    }

    private DaySchedule() { }
    
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
            _daySlots = [];
            return Result.Success();
        }
            
        var slots = new List<Slot>();
        var currentTime = Start;
            
        while (currentTime.Add(SlotDuration) <= End)
        {
            slots.Add(new Slot(currentTime, currentTime.Add(SlotDuration), Status.Available));
                
            currentTime = currentTime.Add(SlotDuration);
        }
    
        _daySlots = slots;
            
        return Result.Success();
    }
}