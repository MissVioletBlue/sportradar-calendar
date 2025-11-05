using Microsoft.EntityFrameworkCore;
using Sportradar.Calendar.Application.Abstractions;
using Sportradar.Calendar.Application.DTOs;
using Sportradar.Calendar.Domain.Entities;

namespace Sportradar.Calendar.Infrastructure.Persistence;

public sealed class EfEntityEventRepository : IEntityEventRepository
{
    private readonly AppDbContext _dbContext;

    public EfEntityEventRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<EventDto>> GetUpcomingAsync(
        DateTimeOffset from,
        DateTimeOffset to,
        int? sportId,
        CancellationToken cancellationToken)
    {
        var eventsQuery = _dbContext.Events
            .AsNoTracking()
            .Where(e => e.StartsAt >= from && e.StartsAt <= to);

        if (sportId.HasValue)
        {
            eventsQuery = eventsQuery.Where(e => e.SportId == sportId.Value);
        }

        var query = from entity in eventsQuery
                    join sport in _dbContext.Sports.AsNoTracking() on entity.SportId equals sport.Id
                    orderby entity.StartsAt
                    select new EventDto(entity.Id, sport.Name, entity.StartsAt, entity.Title);

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<int> AddAsync(CreateEventDto input, CancellationToken cancellationToken)
    {
        var sportExists = await _dbContext.Sports
            .AsNoTracking()
            .AnyAsync(s => s.Id == input.SportId, cancellationToken);

        if (!sportExists)
        {
            throw new ArgumentException($"Sport with id {input.SportId} was not found.", nameof(input));
        }

        var entity = new EntityEvent(input.SportId, input.StartsAt, input.Title.Trim());

        await _dbContext.Events.AddAsync(entity, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.Events.FindAsync(new object?[] { id }, cancellationToken);
        if (entity is null)
        {
            return;
        }

        _dbContext.Events.Remove(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<EventDto?> GetAsync(int id, CancellationToken cancellationToken)
    {
        var query = from entity in _dbContext.Events.AsNoTracking()
                    join sport in _dbContext.Sports.AsNoTracking() on entity.SportId equals sport.Id
                    where entity.Id == id
                    select new EventDto(entity.Id, sport.Name, entity.StartsAt, entity.Title);

        return await query.SingleOrDefaultAsync(cancellationToken);
    }
}