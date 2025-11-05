namespace Sportradar.Calendar.Application.DTOs;

// input dto i use when user creates event, mapping is straight from request
public record CreateEventDto(int SportId, DateTimeOffset StartsAt, string Title);