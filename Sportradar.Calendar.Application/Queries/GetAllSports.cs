using Sportradar.Calendar.Application.Abstractions;
using Sportradar.Calendar.Application.DTOs;

namespace Sportradar.Calendar.Application.Queries;

// query object so controllers ask for sports without touching repo directly
public sealed class GetAllSports
{
    private readonly ISportRepository _repository;

    public GetAllSports(ISportRepository repository)
    {
        _repository = repository;
    }

    public Task<IReadOnlyList<SportDto>> ExecuteAsync(CancellationToken cancellationToken)
    {
        // repository already knows how to fetch sports, so i simply forward call
        return _repository.GetAllAsync(cancellationToken);
    }
}