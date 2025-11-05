using Sportradar.Calendar.Application.DTOs;

namespace Sportradar.Calendar.Application.Abstractions;

public interface ISportRepository
{
    Task<IReadOnlyList<SportDto>> GetAllAsync(CancellationToken cancellationToken);
}