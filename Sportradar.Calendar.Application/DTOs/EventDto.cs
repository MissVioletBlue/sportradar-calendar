namespace Sportradar.Calendar.Application.DTOs;

public record EventDto(int Id, string Sport, DateTimeOffset StartsAt, string Title);