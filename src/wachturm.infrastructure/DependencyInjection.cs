using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using wachturm.Application.Abstractions;
using wachturm.Infrastructure.Persistence;
using wachturm.Infrastructure.Repositories;

namespace wachturm.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<WachturmDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IMonitoredEndpointRepository, MonitoredEndpointRepository>();
        services.AddScoped<ICheckResultRepository, CheckResultRepository>();
        
        return services;
    }
}