using System.ComponentModel.DataAnnotations;

namespace Sportradar.Calendar.Presentation.Web.Requests;

// this request record catches data from form post so we can validate it fast
public sealed record CreateEventRequest
{
    [Required]
    [Range(1, int.MaxValue)]
    // Sport id connects to sport table in db.
    public int SportId { get; init; }

    [Required]
    [DataType(DataType.DateTime)]
    // Start time uses offset so we remember timezone user picked.
    public DateTimeOffset StartsAt { get; init; }

    [Required]
    [StringLength(255)]
    // Title defaults to empty, framework will fill it from request body.
    public string Title { get; init; } = string.Empty;
}