using Serilog;
using wachturm.Infrastructure;
using wachturm.Infrastructure.Persistence;
using wachturm.Worker;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSerilog((services, configuration) =>
{
    configuration
        .ReadFrom.Configuration(builder.Configuration)
        .Enrich.FromLogContext()
        .Enrich.WithProperty("ServiceName", "wachturm.Worker")
        .WriteTo.Console();
});

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddHttpClient();
builder.Services.AddHostedService<Worker>();

var host = builder.Build();

await DatabaseInitializer.EnsureDatabaseCreatedAsync(host.Services);

host.Run();
