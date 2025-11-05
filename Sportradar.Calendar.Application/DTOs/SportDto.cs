namespace Sportradar.Calendar.Application.DTOs;

// super tiny dto that goes out to controllers, keeps data clean for json
public sealed record SportDto(int Id, string Name);