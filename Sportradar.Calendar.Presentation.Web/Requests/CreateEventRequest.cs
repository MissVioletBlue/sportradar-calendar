using System.ComponentModel.DataAnnotations;

namespace Sportradar.Calendar.Presentation.Web.Requests;

public sealed record CreateEventRequest
{
    [Required]
    [Range(1, int.MaxValue)]
    public int SportId { get; init; }

    [Required]
    [DataType(DataType.DateTime)]
    public DateTimeOffset StartsAt { get; init; }

    [Required]
    [StringLength(255)]
    public string Title { get; init; } = string.Empty;
}