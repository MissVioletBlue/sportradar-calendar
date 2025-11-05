using Sportradar.Calendar.Application.DTOs;

namespace Sportradar.Calendar.Application.Abstractions;

// repository contract so app layer does not care about actual storage
public interface IEntityEventRepository
{
    Task<IReadOnlyList<EventDto>> GetUpcomingAsync(DateTimeOffset from, DateTimeOffset to, int? sportId, CancellationToken cancellationToken);
    Task<int> AddAsync(CreateEventDto input, CancellationToken cancellationToken);
    Task DeleteAsync(int id, CancellationToken cancellationToken);
    Task<EventDto?> GetAsync(int id, CancellationToken cancellationToken);
}