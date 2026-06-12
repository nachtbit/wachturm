using wachturm.Application.Abstractions;
using wachturm.Application.Common;

namespace wachturm.Application.Endpoints.GetEndpointById;

public sealed class GetEndpointByIdHandler
{
    private readonly IMonitoredEndpointRepository _repository;

    public GetEndpointByIdHandler(IMonitoredEndpointRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<EndpointResponse>> HandleAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var endpoint = await _repository.GetByIdAsync(id, cancellationToken);

        return endpoint is null
            ? Result<EndpointResponse>.Failure("Endpoint not found.")
            : Result<EndpointResponse>.Success(EndpointResponse.FromEntity(endpoint));
    }
}
