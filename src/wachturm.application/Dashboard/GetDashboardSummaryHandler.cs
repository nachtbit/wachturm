using wachturm.Application.Abstractions;
using wachturm.Application.Endpoints;

namespace wachturm.Application.Dashboard;

public sealed class GetDashboardSummaryHandler
{
    private readonly IMonitoredEndpointRepository _endpointRepository;
    private readonly ICheckResultRepository _checkResultRepository;
    private readonly EndpointStatisticsCalculator _calculator;

    public GetDashboardSummaryHandler(
        IMonitoredEndpointRepository endpointRepository,
        ICheckResultRepository checkResultRepository,
        EndpointStatisticsCalculator calculator)
    {
        _endpointRepository = endpointRepository;
        _checkResultRepository = checkResultRepository;
        _calculator = calculator;
    }

    public async Task<DashboardSummaryResponse> HandleAsync(CancellationToken cancellationToken = default)
    {
        var endpoints = await _endpointRepository.GetAllAsync(cancellationToken);
        var results = await _checkResultRepository.GetAllAsync(cancellationToken);

        return _calculator.CalculateDashboardSummary(endpoints, results);
    }
}
