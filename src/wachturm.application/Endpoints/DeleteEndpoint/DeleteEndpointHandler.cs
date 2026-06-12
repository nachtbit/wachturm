using wachturm.Application.Abstractions;
using wachturm.Application.Common;

namespace wachturm.Application.Endpoints.DeleteEndpoint;

public sealed class DeleteEndpointHandler
{
    private readonly IMonitoredEndpointRepository _repository;

    public DeleteEndpointHandler(IMonitoredEndpointRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result> HandleAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var endpoint = await _repository.GetByIdAsync(id, cancellationToken);

        if (endpoint is null)
            return Result.Failure("Endpoint not found.");

        await _repository.DeleteAsync(endpoint, cancellationToken);

        return Result.Success();
    }
}
