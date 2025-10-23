using Sportradar.Calendar.Domain.Enums;

namespace Sportradar.Calendar.Domain.Entities;

public sealed class EntityEvent
{
    public int Id { get; private set; }
    public int SportId { get; private set; }
    public DateTimeOffset StartsAt { get; private set; }
    public string Title { get; private set; } = "Placeholder Title";
    public EventStatus Status { get; private set; } = EventStatus.Scheduled;
    
    public EntityEvent(int id, int sportId, DateTimeOffset startsAt, string title)
    {
        Id = id;
        SportId = sportId;
        StartsAt = startsAt;
        Title = title; ;
    }
    
    private EntityEvent() {} // TODO: IMPLEMENT EF LATER.
}