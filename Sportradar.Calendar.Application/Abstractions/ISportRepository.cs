using Sportradar.Calendar.Application.DTOs;

namespace Sportradar.Calendar.Application.Abstractions;

// separate repo so sports list can be cached later if needed
public interface ISportRepository
{
    Task<IReadOnlyList<SportDto>> GetAllAsync(CancellationToken cancellationToken);
}