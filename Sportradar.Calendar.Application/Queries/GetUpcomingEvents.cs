using Sportradar.Calendar.Application.Abstractions;
using Sportradar.Calendar.Application.DTOs;

namespace Sportradar.Calendar.Application.Queries;

// query service for fetching events that are happening soon
public sealed class GetUpcomingEvents
{
    private readonly IEntityEventRepository _repository;
    
    public GetUpcomingEvents(IEntityEventRepository repository)
    {
        _repository = repository;
    }

    public Task<IReadOnlyList<EventDto>> ExecuteAsync(DateTimeOffset from, DateTimeOffset to, int? sportId,
        CancellationToken cancellationToken)
    {
        // i just forward parameters because repository already implements filters
        return _repository.GetUpcomingAsync(from, to, sportId, cancellationToken);
    }
}