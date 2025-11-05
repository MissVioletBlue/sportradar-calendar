using Microsoft.EntityFrameworkCore;
using Sportradar.Calendar.Application.Abstractions;
using Sportradar.Calendar.Application.DTOs;

namespace Sportradar.Calendar.Infrastructure.Persistence;

public sealed class EfSportRepository : ISportRepository
{
    private readonly AppDbContext _dbContext;

    public EfSportRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<SportDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.Sports
            .AsNoTracking()
            .OrderBy(s => s.Name)
            .Select(s => new SportDto(s.Id, s.Name))
            .ToListAsync(cancellationToken);
    }
}