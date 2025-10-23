using Sportradar.Calendar.Application.Abstractions;
using Sportradar.Calendar.Application.DTOs;

namespace Sportradar.Calendar.Application.Queries;

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
        return _repository.GetUpcomingAsync(from, to, sportId, cancellationToken);
    }
}