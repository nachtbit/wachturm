using wachturm.Domain.Common;

namespace wachturm.Domain.Entities;

public class CheckResult : BaseEntity
{
    public Guid EndpointId { get; set; }
    public int StatusCode { get; set; }
    public long ResponseTimeMs { get; set; }
    public bool IsSuccess { get; set; }
    public DateTime CheckedAtUtc { get; set; } = DateTime.UtcNow;
}