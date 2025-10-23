using Sportradar.Calendar.Application.Abstractions;
using Sportradar.Calendar.Application.DTOs;

namespace Sportradar.Calendar.Infrastructure.InMemory;

public sealed class InMemoryEventRepository : IEntityEventRepository
{
    private readonly List<EventDto> _events = new()
    {
        new(1, "Football", new DateTime(2023, 01, 01), "Salzburg VS Sturm"),
        new(2, "Ice Hockey", new DateTime(2023, 01, 02), "KAC VS Capitals"),

    };
    public Task<IReadOnlyList<EventDto>> GetUpcomingAsync(DateTimeOffset from, DateTimeOffset to, int? sportId, CancellationToken cancellationToken)
    {
        return Task.FromResult<IReadOnlyList<EventDto>>(_events.Where(entityEvent => entityEvent.StartsAt >= from && entityEvent.StartsAt <= to).ToList());
    }

    public Task<int> AddAsync(CreateEventDto input, CancellationToken ct)
    {
        var id = _events.Count + 1;
        _events.Add(new(id, "Football", input.StartsAt, input.Title));
        return Task.FromResult(id);
    }
}