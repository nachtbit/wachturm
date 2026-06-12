using wachturm.Application.Abstractions;

namespace wachturm.Application.Endpoints.GetEndpoints;

public sealed class GetEndpointsHandler
{
    private readonly IMonitoredEndpointRepository _repository;

    public GetEndpointsHandler(IMonitoredEndpointRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<EndpointResponse>> HandleAsync(CancellationToken cancellationToken = default)
    {
        var endpoints = await _repository.GetAllAsync(cancellationToken);

        return endpoints
            .Select(EndpointResponse.FromEntity)
            .ToList();
    }
}
