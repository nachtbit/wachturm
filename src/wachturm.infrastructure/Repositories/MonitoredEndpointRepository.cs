using Microsoft.EntityFrameworkCore;
using wachturm.Application.Abstractions;
using wachturm.Domain.Entities;
using wachturm.Infrastructure.Persistence;

namespace wachturm.Infrastructure.Repositories;

public class MonitoredEndpointRepository : IMonitoredEndpointRepository
{
    private readonly WachturmDbContext _dbContext;

    public MonitoredEndpointRepository(WachturmDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(MonitoredEndpoint endpoint, CancellationToken cancellationToken = default)
    {
        _dbContext.MonitoredEndpoints.Add(endpoint);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<MonitoredEndpoint>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.MonitoredEndpoints
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<MonitoredEndpoint?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.MonitoredEndpoints
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }
    
    public async Task UpdateAsync(MonitoredEndpoint endpoint, CancellationToken cancellationToken = default)
    {
        _dbContext.MonitoredEndpoints.Update(endpoint);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(MonitoredEndpoint endpoint, CancellationToken cancellationToken = default)
    {
        _dbContext.MonitoredEndpoints.Remove(endpoint);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}