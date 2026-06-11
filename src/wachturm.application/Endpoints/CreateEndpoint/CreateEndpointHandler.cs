using wachturm.Application.Abstractions;
using wachturm.Application.Common;
using wachturm.Domain.Entities;

namespace wachturm.Application.Endpoints.CreateEndpoint;

public sealed class CreateEndpointHandler
{
    private readonly IMonitoredEndpointRepository _repository;

    public CreateEndpointHandler(IMonitoredEndpointRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<MonitoredEndpoint>> HandleAsync(
        CreateEndpointRequest request,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            return Result<MonitoredEndpoint>.Failure("Endpoint name is required.");

        if (string.IsNullOrWhiteSpace(request.Url))
            return Result<MonitoredEndpoint>.Failure("Endpoint URL is required.");

        if (!Uri.TryCreate(request.Url, UriKind.Absolute, out var uri) ||
            (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
            return Result<MonitoredEndpoint>.Failure("Endpoint URL must be a valid HTTP or HTTPS URL.");

        if (request.IntervalSeconds < 5)
            return Result<MonitoredEndpoint>.Failure("Interval must be at least 5 seconds.");
        
        if (request.TimeoutThresholdMs < 100)
            return Result<MonitoredEndpoint>.Failure("Timeout threshold must be at least 100 ms.");

        var endpoint = new MonitoredEndpoint
        {
            Name = request.Name.Trim(),
            Url = request.Url.Trim(),
            Method = request.Method.Trim().ToUpperInvariant(),
            IntervalSeconds = request.IntervalSeconds,
            TimeoutThresholdMs = request.TimeoutThresholdMs,
            IsActive = true
        };

        await _repository.AddAsync(endpoint, cancellationToken);

        return Result<MonitoredEndpoint>.Success(endpoint);
    }
}