using wachturm.Application.Abstractions;
using wachturm.Application.Endpoints.CreateEndpoint;
using wachturm.Domain.Entities;
using Xunit;

namespace wachturm.application.tests;

public sealed class CreateEndpointHandlerTests
{
    [Fact]
    public async Task HandleAsync_ReturnsFailure_WhenUrlIsInvalid()
    {
        var handler = new CreateEndpointHandler(new FakeEndpointRepository());

        var result = await handler.HandleAsync(new CreateEndpointRequest
        {
            Name = "Bad endpoint",
            Url = "not-a-url",
            Method = "GET",
            IntervalSeconds = 30,
            TimeoutThresholdMs = 3000
        });

        Assert.True(result.IsFailure);
        Assert.Equal("Endpoint URL must be a valid HTTP or HTTPS URL.", result.Error);
    }

    [Fact]
    public async Task HandleAsync_ReturnsFailure_WhenMethodIsNotAllowed()
    {
        var handler = new CreateEndpointHandler(new FakeEndpointRepository());

        var result = await handler.HandleAsync(new CreateEndpointRequest
        {
            Name = "Bad endpoint",
            Url = "https://example.com",
            Method = "OPTIONS",
            IntervalSeconds = 30,
            TimeoutThresholdMs = 3000
        });

        Assert.True(result.IsFailure);
        Assert.Equal("HTTP method must be one of: GET, POST, PUT, PATCH, DELETE, HEAD.", result.Error);
    }

    [Fact]
    public async Task HandleAsync_CreatesEndpoint_WhenRequestIsValid()
    {
        var repository = new FakeEndpointRepository();
        var handler = new CreateEndpointHandler(repository);

        var result = await handler.HandleAsync(new CreateEndpointRequest
        {
            Name = "Example",
            Url = "https://example.com",
            Method = "post",
            IntervalSeconds = 30,
            TimeoutThresholdMs = 3000
        });

        Assert.True(result.IsSuccess);
        Assert.Equal("POST", result.Value!.Method);
        Assert.Single(repository.Endpoints);
    }

    private sealed class FakeEndpointRepository : IMonitoredEndpointRepository
    {
        public List<MonitoredEndpoint> Endpoints { get; } = [];

        public Task AddAsync(MonitoredEndpoint endpoint, CancellationToken cancellationToken = default)
        {
            Endpoints.Add(endpoint);
            return Task.CompletedTask;
        }

        public Task<List<MonitoredEndpoint>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Endpoints);
        }

        public Task<MonitoredEndpoint?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Endpoints.FirstOrDefault(x => x.Id == id));
        }

        public Task UpdateAsync(MonitoredEndpoint endpoint, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public Task DeleteAsync(MonitoredEndpoint endpoint, CancellationToken cancellationToken = default)
        {
            Endpoints.Remove(endpoint);
            return Task.CompletedTask;
        }

        public Task<List<MonitoredEndpoint>> GetActiveAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Endpoints.Where(x => x.IsActive).ToList());
        }
    }
}
