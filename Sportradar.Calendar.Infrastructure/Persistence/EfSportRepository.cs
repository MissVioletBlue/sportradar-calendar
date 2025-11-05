using Microsoft.EntityFrameworkCore;
using Sportradar.Calendar.Application.Abstractions;
using Sportradar.Calendar.Application.DTOs;

namespace Sportradar.Calendar.Infrastructure.Persistence;

// ef core version of sport repository, very small wrapper around dbset
public sealed class EfSportRepository : ISportRepository
{
    // storing db context for building the query later
    private readonly AppDbContext _dbContext;

    public EfSportRepository(AppDbContext dbContext)
    {
        // dependency injection hands me context, i just keep reference
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<SportDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        // read sports without tracking, sort alphabetically, map to dto
        return await _dbContext.Sports
            .AsNoTracking()
            .OrderBy(s => s.Name)
            .Select(s => new SportDto(s.Id, s.Name))
            .ToListAsync(cancellationToken);
    }
}