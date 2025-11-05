using Microsoft.EntityFrameworkCore;
using Sportradar.Calendar.Application.Abstractions;
using Sportradar.Calendar.Application.DTOs;
using Sportradar.Calendar.Domain.Entities;

namespace Sportradar.Calendar.Infrastructure.Persistence;

// entity framework implementation of event repository, all heavy lifting is sql
public sealed class EfEntityEventRepository : IEntityEventRepository
{
    // keep db context so we can build queries and save stuff
    private readonly AppDbContext _dbContext;
    
    public EfEntityEventRepository(AppDbContext dbContext)
    {
        // context injected from DI container, i store it for later use
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<EventDto>> GetUpcomingAsync(
        DateTimeOffset from,
        DateTimeOffset to,
        int? sportId,
        CancellationToken cancellationToken)
    {
        // start from events table and only pull ones inside requested window
        var eventsQuery = _dbContext.Events
            .AsNoTracking()
            .Where(e => e.StartsAt >= from && e.StartsAt <= to);

        // when caller passes sport filter we narrow down to that sport only
        if (sportId.HasValue)
        {
            eventsQuery = eventsQuery.Where(e => e.SportId == sportId.Value);
        }

        // join with sports so dto can show sport name without another lookup
        var query = from entity in eventsQuery
                    join sport in _dbContext.Sports.AsNoTracking() on entity.SportId equals sport.Id
                    orderby entity.StartsAt
                    select new EventDto(entity.Id, sport.Name, entity.StartsAt, entity.Title);

        // returning list so caller can enumerate, tolist executes sql
        return await query.ToListAsync(cancellationToken);
    }

    public async Task<int> AddAsync(CreateEventDto input, CancellationToken cancellationToken)
    {
        // safety check so we do not insert event that points to missing sport
        var sportExists = await _dbContext.Sports
            .AsNoTracking()
            .AnyAsync(s => s.Id == input.SportId, cancellationToken);

        if (!sportExists)
        {
            throw new ArgumentException($"Sport with id {input.SportId} was not found.", nameof(input));
        }

        // create entity using trimmed title so database stays clean
        var entity = new EntityEvent(input.SportId, input.StartsAt, input.Title.Trim());

        // add and save so EF tracks new identity value
        await _dbContext.Events.AddAsync(entity, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        // after save entity id is populated and we return it
        return entity.Id;
    }

    // not implemented, not needed for this demo
    public async Task DeleteAsync(int id, CancellationToken cancellationToken)
    {
        // find event by id, ef core returns null if missing
        var entity = await _dbContext.Events.FindAsync(new object?[] { id }, cancellationToken);
        if (entity is null)
        {
            // nothing to delete so we just exit quietly
            return;
        }

        // remove entity then persist change
        _dbContext.Events.Remove(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<EventDto?> GetAsync(int id, CancellationToken cancellationToken)
    {
        // join event with sport to provide nice dto for single record
        var query = from entity in _dbContext.Events.AsNoTracking()
                    join sport in _dbContext.Sports.AsNoTracking() on entity.SportId equals sport.Id
                    where entity.Id == id
                    select new EventDto(entity.Id, sport.Name, entity.StartsAt, entity.Title);

        // SingleOrDefaultAsync gives null when event not found which is fine
        return await query.SingleOrDefaultAsync(cancellationToken);
    }
}