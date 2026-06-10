using Scalar.AspNetCore;
using wachturm.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapSwagger("/openapi/{documentName}.json");

    app.MapScalarApiReference(options =>
    {
        options.Title = "Wachturm API";
        options.OpenApiRoutePattern = "/openapi/{documentName}.json";
    });
}

app.MapControllers();

app.Run();