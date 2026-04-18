using CSharpFunctionalExtensions;

namespace ScheduleApp.Core.Model.Scheduling;

public class Slot
{
    public TimeOnly Start { get; set; }
    public TimeOnly End { get; set; }
    public Status Status { get; set; }
    
    public Slot(TimeOnly start, TimeOnly end, Status status)
    {
        Start = start;
        End = end;
        Status = status;
    }
    
    public Result Disable()
    {
        if (Status == Status.Booked)
            return Result.Failure("Cannot disable a booked slot");
        
        Status = Status.Unavailable;
        return Result.Success();
    }

    public Result Enable()
    {
        if (Status == Status.Booked)
            return Result.Failure("Cannot enable a booked slot");
        
        Status = Status.Available;
        return Result.Success();
    }
}