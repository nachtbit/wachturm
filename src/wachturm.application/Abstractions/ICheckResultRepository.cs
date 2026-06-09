using wachturm.Domain.Entities;

namespace wachturm.Application.Abstractions;

public interface ICheckResultRepository
{
    Task AddAsync(CheckResult result, CancellationToken cancellationToken = default);
    Task<List<CheckResult>> GetByEndpointIdAsync(Guid endpointId, CancellationToken cancellationToken = default);
}