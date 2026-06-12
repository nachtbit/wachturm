using Microsoft.EntityFrameworkCore;
using wachturm.Application.Abstractions;
using wachturm.Domain.Entities;
using wachturm.Infrastructure.Persistence;

namespace wachturm.Infrastructure.Repositories;

public class CheckResultRepository : ICheckResultRepository
{
    private readonly WachturmDbContext _dbContext;

    public CheckResultRepository(WachturmDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task AddAsync(CheckResult result, CancellationToken cancellationToken = default)
    {
        _dbContext.CheckResults.Add(result);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<CheckResult>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.CheckResults
            .AsNoTracking()
            .OrderByDescending(x => x.CheckedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<CheckResult>> GetByEndpointIdAsync(Guid endpointId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.CheckResults
            .AsNoTracking()
            .Where(x => x.EndpointId == endpointId)
            .OrderByDescending(x => x.CheckedAtUtc)
            .ToListAsync(cancellationToken);
    }
    
    public async Task<List<CheckResult>> GetLatestByEndpointIdAsync(
        Guid endpointId,
        int count,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.CheckResults
            .AsNoTracking()
            .Where(x => x.EndpointId == endpointId)
            .OrderByDescending(x => x.CheckedAtUtc)
            .Take(count)
            .ToListAsync(cancellationToken);
    }
    
    public async Task<CheckResult?> GetLatestByEndpointIdAsync(
        Guid endpointId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.CheckResults
            .AsNoTracking()
            .Where(x => x.EndpointId == endpointId)
            .OrderByDescending(x => x.CheckedAtUtc)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<List<CheckResult>> GetHistoryByEndpointIdAsync(
        Guid endpointId,
        int take,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.CheckResults
            .AsNoTracking()
            .Where(x => x.EndpointId == endpointId)
            .OrderByDescending(x => x.CheckedAtUtc)
            .Take(take)
            .ToListAsync(cancellationToken);
    }
}
