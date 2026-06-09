using wachturm.Domain.Entities;

namespace wachturm.Application.Abstractions;

public interface IMonitoredEndpointRepository
{
    Task AddAsync(MonitoredEndpoint endpoint, CancellationToken cancellationToken = default);
    Task<List<MonitoredEndpoint>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<MonitoredEndpoint?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}