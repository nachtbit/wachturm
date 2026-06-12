using wachturm.Domain.Entities;

namespace wachturm.Application.Endpoints;

public sealed class EndpointResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string Method { get; set; } = string.Empty;
    public int IntervalSeconds { get; set; }
    public int TimeoutThresholdMs { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastCheckedAtUtc { get; set; }

    public static EndpointResponse FromEntity(MonitoredEndpoint endpoint)
    {
        return new EndpointResponse
        {
            Id = endpoint.Id,
            Name = endpoint.Name,
            Url = endpoint.Url,
            Method = endpoint.Method,
            IntervalSeconds = endpoint.IntervalSeconds,
            TimeoutThresholdMs = endpoint.TimeoutThresholdMs,
            IsActive = endpoint.IsActive,
            CreatedAt = endpoint.CreatedAt,
            LastCheckedAtUtc = endpoint.LastCheckedAtUtc
        };
    }
}
