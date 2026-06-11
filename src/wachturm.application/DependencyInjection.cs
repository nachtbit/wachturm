using Microsoft.Extensions.DependencyInjection;
using wachturm.Application.Endpoints.CreateEndpoint;

namespace wachturm.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<CreateEndpointHandler>();

        return services;
    }
}