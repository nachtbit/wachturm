using Microsoft.Extensions.DependencyInjection;
using wachturm.Application.Dashboard;
using wachturm.Application.Endpoints;
using wachturm.Application.Endpoints.ActivateEndpoint;
using wachturm.Application.Endpoints.CreateEndpoint;
using wachturm.Application.Endpoints.DeactivateEndpoint;
using wachturm.Application.Endpoints.DeleteEndpoint;
using wachturm.Application.Endpoints.GetEndpointById;
using wachturm.Application.Endpoints.GetEndpointHistory;
using wachturm.Application.Endpoints.GetEndpoints;
using wachturm.Application.Endpoints.GetEndpointStatus;
using wachturm.Application.Endpoints.GetLatestCheckResult;

namespace wachturm.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<EndpointStatisticsCalculator>();
        services.AddScoped<CreateEndpointHandler>();
        services.AddScoped<GetEndpointsHandler>();
        services.AddScoped<GetEndpointByIdHandler>();
        services.AddScoped<ActivateEndpointHandler>();
        services.AddScoped<DeactivateEndpointHandler>();
        services.AddScoped<DeleteEndpointHandler>();
        services.AddScoped<GetEndpointStatusHandler>();
        services.AddScoped<GetEndpointHistoryHandler>();
        services.AddScoped<GetLatestCheckResultHandler>();
        services.AddScoped<GetDashboardSummaryHandler>();

        return services;
    }
}
