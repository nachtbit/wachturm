using System.Diagnostics;
using wachturm.Application.Abstractions;
using wachturm.Domain.Entities;

namespace wachturm.Worker;

public class Worker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<Worker> _logger;

    public Worker(
        IServiceScopeFactory scopeFactory,
        IHttpClientFactory httpClientFactory,
        ILogger<Worker> logger)
    {
        _scopeFactory = scopeFactory;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Wachturm worker started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();

                var endpointRepository =
                    scope.ServiceProvider.GetRequiredService<IMonitoredEndpointRepository>();

                var resultRepository =
                    scope.ServiceProvider.GetRequiredService<ICheckResultRepository>();

                var endpoints = await endpointRepository.GetActiveAsync(stoppingToken);

                foreach (var endpoint in endpoints)
                {
                    var shouldCheck =
                        endpoint.LastCheckedAtUtc is null ||
                        endpoint.LastCheckedAtUtc.Value.AddSeconds(endpoint.IntervalSeconds) <= DateTime.UtcNow;

                    if (!shouldCheck)
                    {
                        continue;
                    }

                    try
                    {
                        await CheckEndpointAsync(endpoint, endpointRepository, resultRepository, stoppingToken);
                    }
                    catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                    {
                        throw;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(
                            ex,
                            "Endpoint check crashed for {EndpointId} {Url}. Other endpoints will continue.",
                            endpoint.Id,
                            endpoint.Url);
                    }
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Worker polling loop failed. The worker will retry.");
            }

            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }

    private async Task CheckEndpointAsync(
        MonitoredEndpoint endpoint,
        IMonitoredEndpointRepository endpointRepository,
        ICheckResultRepository resultRepository,
        CancellationToken cancellationToken)
    {
        var client = _httpClientFactory.CreateClient();
        var stopwatch = Stopwatch.StartNew();
        using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeoutCts.CancelAfter(TimeSpan.FromMilliseconds(endpoint.TimeoutThresholdMs));

        try
        {
            using var request = new HttpRequestMessage(
                new HttpMethod(endpoint.Method),
                endpoint.Url);

            using var response = await client.SendAsync(request, timeoutCts.Token);

            stopwatch.Stop();

            var result = new CheckResult
            {
                EndpointId = endpoint.Id,
                StatusCode = (int)response.StatusCode,
                ResponseTimeMs = stopwatch.ElapsedMilliseconds,
                IsSuccess = response.IsSuccessStatusCode,
                CheckedAtUtc = DateTime.UtcNow
            };

            await resultRepository.AddAsync(result, cancellationToken);

            DetectLatencyWarning(endpoint, result);

            await DetectAlertAsync(endpoint, resultRepository, cancellationToken);

            _logger.LogInformation(
                "Checked endpoint {EndpointId} {Url}. StatusCode: {StatusCode}. ResponseTimeMs: {ResponseTimeMs}. IsSuccess: {IsSuccess}",
                endpoint.Id,
                endpoint.Url,
                result.StatusCode,
                result.ResponseTimeMs,
                result.IsSuccess);

            await MarkEndpointAsCheckedAsync(endpoint, endpointRepository, cancellationToken);
        }
        catch (OperationCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            stopwatch.Stop();

            var result = new CheckResult
            {
                EndpointId = endpoint.Id,
                StatusCode = 0,
                ResponseTimeMs = stopwatch.ElapsedMilliseconds,
                IsSuccess = false,
                CheckedAtUtc = DateTime.UtcNow
            };

            await resultRepository.AddAsync(result, cancellationToken);

            DetectLatencyWarning(endpoint, result);

            await DetectAlertAsync(endpoint, resultRepository, cancellationToken);

            _logger.LogWarning(
                ex,
                "Endpoint check timed out for {EndpointId} {Url}. StatusCode: {StatusCode}. ResponseTimeMs: {ResponseTimeMs}. IsSuccess: {IsSuccess}",
                endpoint.Id,
                endpoint.Url,
                result.StatusCode,
                result.ResponseTimeMs,
                result.IsSuccess);

            await MarkEndpointAsCheckedAsync(endpoint, endpointRepository, cancellationToken);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            var result = new CheckResult
            {
                EndpointId = endpoint.Id,
                StatusCode = 0,
                ResponseTimeMs = stopwatch.ElapsedMilliseconds,
                IsSuccess = false,
                CheckedAtUtc = DateTime.UtcNow
            };

            await resultRepository.AddAsync(result, cancellationToken);

            await DetectAlertAsync(endpoint, resultRepository, cancellationToken);

            _logger.LogError(
                ex,
                "Failed to check endpoint {EndpointId} {Url}. StatusCode: {StatusCode}. ResponseTimeMs: {ResponseTimeMs}. IsSuccess: {IsSuccess}",
                endpoint.Id,
                endpoint.Url,
                result.StatusCode,
                result.ResponseTimeMs,
                result.IsSuccess);

            await MarkEndpointAsCheckedAsync(endpoint, endpointRepository, cancellationToken);
        }
    }

    private void DetectLatencyWarning(
        MonitoredEndpoint endpoint,
        CheckResult result)
    {
        if (result.ResponseTimeMs <= endpoint.TimeoutThresholdMs)
        {
            return;
        }

        _logger.LogWarning(
            "WARNING: Endpoint {EndpointId} {Url} exceeded latency threshold. Response time: {ResponseTimeMs}ms. Threshold: {ThresholdMs}ms",
            endpoint.Id,
            endpoint.Url,
            result.ResponseTimeMs,
            endpoint.TimeoutThresholdMs);
    }

    private static async Task MarkEndpointAsCheckedAsync(
        MonitoredEndpoint endpoint,
        IMonitoredEndpointRepository endpointRepository,
        CancellationToken cancellationToken)
    {
        endpoint.LastCheckedAtUtc = DateTime.UtcNow;

        await endpointRepository.UpdateAsync(endpoint, cancellationToken);
    }

    private async Task DetectAlertAsync(
        MonitoredEndpoint endpoint,
        ICheckResultRepository resultRepository,
        CancellationToken cancellationToken)
    {
        var latestResults = await resultRepository.GetLatestByEndpointIdAsync(
            endpoint.Id,
            3,
            cancellationToken);

        if (latestResults.Count < 3)
        {
            return;
        }

        var allFailed = latestResults.All(x => !x.IsSuccess);

        if (allFailed)
        {
            _logger.LogWarning(
                "ALERT: Endpoint {EndpointId} {Url} failed 3 consecutive checks.",
                endpoint.Id,
                endpoint.Url);
        }
    }
}
