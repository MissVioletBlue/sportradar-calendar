using Sportradar.Calendar.Application.Abstractions;
using Sportradar.Calendar.Application.DTOs;

namespace Sportradar.Calendar.Infrastructure.InMemory;

// quick in-memory repo mostly for tests or demos without database (no longer used)
public sealed class InMemoryEventRepository : IEntityEventRepository
{
    private static readonly IReadOnlyDictionary<int, string> SportNames = new Dictionary<int, string>
    {
        { 1, "Football" },
        { 2, "Ice Hockey" },
    };
    
    private readonly List<EventDto> _events = new()
    {
        new(1, "Football", new DateTimeOffset(2023, 01, 01, 18, 00, 00, TimeSpan.Zero), "Salzburg VS Sturm"),
        new(2, "Ice Hockey", new DateTimeOffset(2023, 01, 02, 19, 30, 00, TimeSpan.Zero), "KAC VS Capitals"),
    };
    
    private readonly object _syncRoot = new();
    
    public Task<IReadOnlyList<EventDto>> GetUpcomingAsync(DateTimeOffset from, DateTimeOffset to, int? sportId, CancellationToken cancellationToken)
    {
        lock (_syncRoot)
        {
            var events = _events
                .Where(entityEvent => entityEvent.StartsAt >= from && entityEvent.StartsAt <= to)
                .Where(entityEvent => !sportId.HasValue || MatchesSport(entityEvent, sportId.Value))
                .OrderBy(entityEvent => entityEvent.StartsAt)
                .ToList();

            return Task.FromResult<IReadOnlyList<EventDto>>(events);
        }
    }

    public Task<int> AddAsync(CreateEventDto input, CancellationToken cancellationToken)
    {
        lock (_syncRoot)
        {
            var id = _events.Count == 0 ? 1 : _events.Max(entityEvent => entityEvent.Id) + 1;
            var sportName = ResolveSportName(input.SportId);

            _events.Add(new(id, sportName, input.StartsAt, input.Title));

            return Task.FromResult(id);
        }
    }

    public Task DeleteAsync(int id, CancellationToken cancellationToken)
    {
        lock (_syncRoot)
        {
            var index = _events.FindIndex(entityEvent => entityEvent.Id == id);
            if (index >= 0)
            {
                _events.RemoveAt(index);
            }
        }

        return Task.CompletedTask;
    }

    public Task<EventDto?> GetAsync(int id, CancellationToken cancellationToken)
    {
        lock (_syncRoot)
        {
            var entity = _events.FirstOrDefault(entityEvent => entityEvent.Id == id);
            return Task.FromResult(entity);
        }
    }
    
    private static string ResolveSportName(int sportId)
    {
        if (SportNames.TryGetValue(sportId, out var sportName))
        {
            return sportName;
        }

        return "Unknown Sport";
    }
    
    private static bool MatchesSport(EventDto entityEvent, int sportId)
    {
        var sportName = ResolveSportName(sportId);
        return string.Equals(entityEvent.Sport, sportName, StringComparison.OrdinalIgnoreCase);
    }
}