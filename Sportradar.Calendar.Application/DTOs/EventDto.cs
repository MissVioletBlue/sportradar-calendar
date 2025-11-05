namespace Sportradar.Calendar.Application.DTOs;

// output dto for calendars so front end gets nice ready data
public record EventDto(int Id, string Sport, DateTimeOffset StartsAt, string Title);