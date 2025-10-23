namespace Sportradar.Calendar.Application.DTOs;

public record CreateEventDto(int SportId, DateTimeOffset StartsAt, string Title);