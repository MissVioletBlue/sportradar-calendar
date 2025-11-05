using System.ComponentModel.DataAnnotations;
using System.Xml;
using Sportradar.Calendar.Domain.Enums;

namespace Sportradar.Calendar.Domain.Entities;

public sealed class EntityEvent
{
    [Required, Key]
    public int Id { get; private set; }
    [Required]
    public int SportId { get; private set; }
    [Required, DataType(DataType.DateTime)]
    public DateTimeOffset StartsAt { get; private set; }
    [Required, StringLength(255)]
    public string Title { get; private set; } = "Placeholder Title";
    public EventStatus Status { get; private set; } = EventStatus.Scheduled;
    
    public EntityEvent(int sportId, DateTimeOffset startsAt, string title)
    {
        SportId = sportId;
        StartsAt = startsAt;
        Title = title.Trim();
    }
    
    public EntityEvent(int id, int sportId, DateTimeOffset startsAt, string title)
    {
        Id = id;
        SportId = sportId;
        StartsAt = startsAt;
        Title = title;
        Title = title.Trim();
    }
    
    private EntityEvent()
    {
        Title = string.Empty;
    }
}