using wachturm.Application.Dashboard;
using wachturm.Application.Endpoints.GetEndpointStatus;
using wachturm.Domain.Entities;

namespace wachturm.Application.Endpoints;

public sealed class EndpointStatisticsCalculator
{
    public GetEndpointStatusResponse CalculateStatus(Guid endpointId, IReadOnlyCollection<CheckResult> results)
    {
        if (results.Count == 0)
        {
            return new GetEndpointStatusResponse
            {
                EndpointId = endpointId,
                IsHealthy = false,
                UptimePercentage = 0,
                AverageResponseTimeMs = 0,
                TotalChecks = 0,
                FailedChecks = 0
            };
        }

        var orderedResults = results
            .OrderByDescending(x => x.CheckedAtUtc)
            .ToList();

        var lastResult = orderedResults[0];
        var successfulChecks = orderedResults.Count(x => x.IsSuccess);
        var failedChecks = orderedResults.Count - successfulChecks;

        return new GetEndpointStatusResponse
        {
            EndpointId = endpointId,
            IsHealthy = lastResult.IsSuccess,
            LastStatusCode = lastResult.StatusCode,
            LastResponseTimeMs = lastResult.ResponseTimeMs,
            UptimePercentage = Math.Round((double)successfulChecks / orderedResults.Count * 100, 2),
            AverageResponseTimeMs = Math.Round(orderedResults.Average(x => x.ResponseTimeMs), 2),
            TotalChecks = orderedResults.Count,
            FailedChecks = failedChecks
        };
    }

    public DashboardSummaryResponse CalculateDashboardSummary(
        IReadOnlyCollection<MonitoredEndpoint> endpoints,
        IReadOnlyCollection<CheckResult> results)
    {
        var latestResultsByEndpoint = results
            .GroupBy(x => x.EndpointId)
            .Select(x => x.OrderByDescending(result => result.CheckedAtUtc).First())
            .ToList();

        return new DashboardSummaryResponse
        {
            TotalEndpoints = endpoints.Count,
            ActiveEndpoints = endpoints.Count(x => x.IsActive),
            InactiveEndpoints = endpoints.Count(x => !x.IsActive),
            HealthyEndpoints = latestResultsByEndpoint.Count(x => x.IsSuccess),
            UnhealthyEndpoints = latestResultsByEndpoint.Count(x => !x.IsSuccess),
            TotalChecks = results.Count,
            FailedChecks = results.Count(x => !x.IsSuccess),
            AverageResponseTimeMs = results.Count == 0
                ? 0
                : Math.Round(results.Average(x => x.ResponseTimeMs), 2)
        };
    }
}
