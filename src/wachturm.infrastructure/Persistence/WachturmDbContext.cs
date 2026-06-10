using Microsoft.EntityFrameworkCore;
using wachturm.Domain.Entities;

namespace wachturm.Infrastructure.Persistence;

public class WachturmDbContext : DbContext
{
    public WachturmDbContext(DbContextOptions<WachturmDbContext> options)
        : base(options)
    {
    }
    
    public DbSet<MonitoredEndpoint> MonitoredEndpoints => Set<MonitoredEndpoint>();
    public DbSet<CheckResult> CheckResults => Set<CheckResult>();
}