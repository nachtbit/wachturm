using wachturm.Domain.Entities;

namespace wachturm.Application.CheckResults;

public sealed class CheckResultResponse
{
    public Guid Id { get; set; }
    public Guid EndpointId { get; set; }
    public int StatusCode { get; set; }
    public long ResponseTimeMs { get; set; }
    public bool IsSuccess { get; set; }
    public DateTime CheckedAtUtc { get; set; }

    public static CheckResultResponse FromEntity(CheckResult result)
    {
        return new CheckResultResponse
        {
            Id = result.Id,
            EndpointId = result.EndpointId,
            StatusCode = result.StatusCode,
            ResponseTimeMs = result.ResponseTimeMs,
            IsSuccess = result.IsSuccess,
            CheckedAtUtc = result.CheckedAtUtc
        };
    }
}
