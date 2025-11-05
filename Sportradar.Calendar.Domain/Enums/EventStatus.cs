namespace Sportradar.Calendar.Domain.Enums;

// status values i map to UI badges, order kinda matches event lifecycle
public enum EventStatus
{
    Scheduled = 0,
    Postponed = 1,
    Live = 2,
    Finished = 3,
    Cancelled = 4,
}