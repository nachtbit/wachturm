using wachturm.Application.Abstractions;
using wachturm.Application.Common;

namespace wachturm.Application.Endpoints.GetEndpointStatus;

public sealed class GetEndpointStatusHandler
{
    private readonly IMonitoredEndpointRepository _endpointRepository;
    private readonly ICheckResultRepository _checkResultRepository;
    private readonly EndpointStatisticsCalculator _calculator;

    public GetEndpointStatusHandler(
        IMonitoredEndpointRepository endpointRepository,
        ICheckResultRepository checkResultRepository,
        EndpointStatisticsCalculator calculator)
    {
        _endpointRepository = endpointRepository;
        _checkResultRepository = checkResultRepository;
        _calculator = calculator;
    }

    public async Task<Result<GetEndpointStatusResponse>> HandleAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var endpoint = await _endpointRepository.GetByIdAsync(id, cancellationToken);

        if (endpoint is null)
            return Result<GetEndpointStatusResponse>.Failure("Endpoint not found.");

        var results = await _checkResultRepository.GetByEndpointIdAsync(id, cancellationToken);

        return Result<GetEndpointStatusResponse>.Success(_calculator.CalculateStatus(id, results));
    }
}
