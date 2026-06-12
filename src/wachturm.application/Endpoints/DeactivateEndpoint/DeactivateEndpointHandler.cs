using wachturm.Application.Abstractions;
using wachturm.Application.Common;

namespace wachturm.Application.Endpoints.DeactivateEndpoint;

public sealed class DeactivateEndpointHandler
{
    private readonly IMonitoredEndpointRepository _repository;

    public DeactivateEndpointHandler(IMonitoredEndpointRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<EndpointResponse>> HandleAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var endpoint = await _repository.GetByIdAsync(id, cancellationToken);

        if (endpoint is null)
            return Result<EndpointResponse>.Failure("Endpoint not found.");

        endpoint.IsActive = false;

        await _repository.UpdateAsync(endpoint, cancellationToken);

        return Result<EndpointResponse>.Success(EndpointResponse.FromEntity(endpoint));
    }
}
