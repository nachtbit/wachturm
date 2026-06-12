using wachturm.Application.Endpoints;
using wachturm.Domain.Entities;
using Xunit;

namespace wachturm.application.tests;

public sealed class EndpointStatisticsCalculatorTests
{
    [Fact]
    public void CalculateStatus_ReturnsUnhealthyEmptyStatus_WhenNoChecksExist()
    {
        var endpointId = Guid.NewGuid();
        var calculator = new EndpointStatisticsCalculator();

        var status = calculator.CalculateStatus(endpointId, []);

        Assert.Equal(endpointId, status.EndpointId);
        Assert.False(status.IsHealthy);
        Assert.Equal(0, status.TotalChecks);
        Assert.Equal(0, status.AverageResponseTimeMs);
    }

    [Fact]
    public void CalculateStatus_ReturnsAggregateStatistics()
    {
        var endpointId = Guid.NewGuid();
        var calculator = new EndpointStatisticsCalculator();
        var now = DateTime.UtcNow;
        var results = new List<CheckResult>
        {
            new()
            {
                EndpointId = endpointId,
                StatusCode = 200,
                ResponseTimeMs = 100,
                IsSuccess = true,
                CheckedAtUtc = now
            },
            new()
            {
                EndpointId = endpointId,
                StatusCode = 500,
                ResponseTimeMs = 300,
                IsSuccess = false,
                CheckedAtUtc = now.AddMinutes(-1)
            }
        };

        var status = calculator.CalculateStatus(endpointId, results);

        Assert.True(status.IsHealthy);
        Assert.Equal(2, status.TotalChecks);
        Assert.Equal(1, status.FailedChecks);
        Assert.Equal(50, status.UptimePercentage);
        Assert.Equal(200, status.AverageResponseTimeMs);
        Assert.Equal(200, status.LastStatusCode);
        Assert.Equal(100, status.LastResponseTimeMs);
    }
}
