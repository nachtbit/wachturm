namespace wachturm.Application.Endpoints.CreateEndpoint;

public sealed class CreateEndpointRequest
{
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string Method { get; set; } = "GET";
    public int IntervalSeconds { get; set; } = 30;
    public int TimeoutThresholdMs { get; set; } = 3000;
}