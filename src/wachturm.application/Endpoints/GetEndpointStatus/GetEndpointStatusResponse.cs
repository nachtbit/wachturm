namespace wachturm.Application.Endpoints.GetEndpointStatus;

public sealed class GetEndpointStatusResponse
{
    public Guid EndpointId { get; set; }
    public bool IsHealthy { get; set; }
    public int? LastStatusCode { get; set; }
    public long? LastResponseTimeMs { get; set; }
    public double UptimePercentage { get; set; }
    public double AverageResponseTimeMs { get; set; }
    public int TotalChecks { get; set; }
    public int FailedChecks { get; set; }
}