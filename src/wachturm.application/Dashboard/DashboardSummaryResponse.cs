namespace wachturm.Application.Dashboard;

public sealed class DashboardSummaryResponse
{
    public int TotalEndpoints { get; set; }
    public int ActiveEndpoints { get; set; }
    public int InactiveEndpoints { get; set; }
    public int HealthyEndpoints { get; set; }
    public int UnhealthyEndpoints { get; set; }
    public int TotalChecks { get; set; }
    public int FailedChecks { get; set; }
    public double AverageResponseTimeMs { get; set; }
}
