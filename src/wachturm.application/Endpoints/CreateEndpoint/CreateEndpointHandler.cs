using wachturm.Application.Abstractions;
using wachturm.Application.Common;
using wachturm.Application.Endpoints;
using wachturm.Domain.Entities;

namespace wachturm.Application.Endpoints.CreateEndpoint;

public sealed class CreateEndpointHandler
{
    private readonly IMonitoredEndpointRepository _repository;

    public CreateEndpointHandler(IMonitoredEndpointRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<EndpointResponse>> HandleAsync(
        CreateEndpointRequest request,
        CancellationToken cancellationToken = default)
    {
        var validationResult = EndpointValidation.Validate(
            request.Name,
            request.Url,
            request.Method,
            request.IntervalSeconds,
            request.TimeoutThresholdMs);

        if (validationResult.IsFailure)
            return Result<EndpointResponse>.Failure(validationResult.Error!);

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

        return Result<EndpointResponse>.Success(EndpointResponse.FromEntity(endpoint));
    }
}
