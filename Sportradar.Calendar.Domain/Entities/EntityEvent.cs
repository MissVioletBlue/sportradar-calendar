using System.ComponentModel.DataAnnotations;
using System.Xml;
using Sportradar.Calendar.Domain.Enums;

namespace Sportradar.Calendar.Domain.Entities;

// this class is like my row for events table, so i pack all simple info here
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
    
    // this constructor is for new events before they hit database
    public EntityEvent(int sportId, DateTimeOffset startsAt, string title)
    {
        SportId = sportId;
        StartsAt = startsAt;
        Title = title.Trim();
    }
    
    // this one is for materializing event with already known id (like from repo)
    public EntityEvent(int id, int sportId, DateTimeOffset startsAt, string title)
    {
        Id = id;
        SportId = sportId;
        StartsAt = startsAt;
        Title = title;
        Title = title.Trim();
    }
    
    // ef core needs empty ctor but we never use it directly
    private EntityEvent()
    {
        Title = string.Empty;
    }
}