using wachturm.Domain.Common;

namespace wachturm.Domain.Entities;

public class MonitoredEndpoint : BaseEntity
{
    public string Name { get; set; } =  string.Empty;
    public string Url { get; set; } = string.Empty;
    public string Method { get; set; } = "GET";
    public int IntervalSeconds { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime? LastCheckedAtUtc { get; set; }
    public int TimeoutThresholdMs { get; set; } = 3000;
}