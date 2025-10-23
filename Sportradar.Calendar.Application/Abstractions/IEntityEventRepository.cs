using Sportradar.Calendar.Application.DTOs;

namespace Sportradar.Calendar.Application.Abstractions;

public interface IEntityEventRepository
{
    Task<IReadOnlyList<EventDto>> GetUpcomingAsync(DateTimeOffset from, DateTimeOffset to, int? sportId, CancellationToken cancellationToken);
    Task<int> AddAsync(CreateEventDto input, CancellationToken ct);
}