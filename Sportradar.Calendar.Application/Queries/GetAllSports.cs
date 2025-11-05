using Sportradar.Calendar.Application.Abstractions;
using Sportradar.Calendar.Application.DTOs;

namespace Sportradar.Calendar.Application.Queries;

public sealed class GetAllSports
{
    private readonly ISportRepository _repository;

    public GetAllSports(ISportRepository repository)
    {
        _repository = repository;
    }

    public Task<IReadOnlyList<SportDto>> ExecuteAsync(CancellationToken cancellationToken)
    {
        return _repository.GetAllAsync(cancellationToken);
    }
}