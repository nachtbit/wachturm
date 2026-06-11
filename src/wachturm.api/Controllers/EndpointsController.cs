using Microsoft.AspNetCore.Mvc;
using wachturm.Application.Abstractions;
using wachturm.Application.Endpoints.CreateEndpoint;
using wachturm.Application.Endpoints.GetEndpointStatus;

namespace wachturm.Api.Controllers;

[ApiController]
[Route("api/endpoints")]
public sealed class EndpointsController : ControllerBase
{
    private readonly CreateEndpointHandler _createEndpointHandler;
    private readonly IMonitoredEndpointRepository _endpointRepository;
    private readonly ICheckResultRepository _checkResultRepository;

    public EndpointsController(
        CreateEndpointHandler createEndpointHandler,
        IMonitoredEndpointRepository endpointRepository,
        ICheckResultRepository checkResultRepository)
    {
        _createEndpointHandler = createEndpointHandler;
        _endpointRepository = endpointRepository;
        _checkResultRepository = checkResultRepository;
    }

    [HttpPost]
    public async Task<IActionResult> CreateEndpoint(
        [FromBody] CreateEndpointRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _createEndpointHandler.HandleAsync(request, cancellationToken);

        if (result.IsFailure)
            return BadRequest(new { error = result.Error });

        return CreatedAtAction(
            nameof(CreateEndpoint),
            new { id = result.Value!.Id },
            result.Value);
    }

    [HttpGet]
    public async Task<IActionResult> GetEndpoints(CancellationToken cancellationToken)
    {
        var endpoints = await _endpointRepository.GetAllAsync(cancellationToken);
        return Ok(endpoints);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetEndpointById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var endpoint = await _endpointRepository.GetByIdAsync(id, cancellationToken);

        if (endpoint is null)
            return NotFound(new { error = "Endpoint not found." });
        
        return Ok(endpoint);
    }
    
    [HttpPatch("{id:guid}/activate")]
    public async Task<IActionResult> ActivateEndpoint(
        Guid id,
        CancellationToken cancellationToken)
    {
        var endpoint = await _endpointRepository.GetByIdAsync(id, cancellationToken);

        if (endpoint is null)
            return NotFound(new { error = "Endpoint not found." });

        endpoint.IsActive = true;

        await _endpointRepository.UpdateAsync(endpoint, cancellationToken);

        return Ok(endpoint);
    }

    [HttpPatch("{id:guid}/deactivate")]
    public async Task<IActionResult> DeactivateEndpoint(
        Guid id,
        CancellationToken cancellationToken)
    {
        var endpoint = await _endpointRepository.GetByIdAsync(id, cancellationToken);

        if (endpoint is null)
            return NotFound(new { error = "Endpoint not found." });

        endpoint.IsActive = false;

        await _endpointRepository.UpdateAsync(endpoint, cancellationToken);

        return Ok(endpoint);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteEndpoint(
        Guid id,
        CancellationToken cancellationToken)
    {
        var endpoint = await _endpointRepository.GetByIdAsync(id, cancellationToken);

        if (endpoint is null)
            return NotFound(new { error = "Endpoint not found." });

        await _endpointRepository.DeleteAsync(endpoint, cancellationToken);

        return NoContent();
    }
    
    [HttpGet("{id:guid}/results")]
    public async Task<IActionResult> GetEndpointResults(
        Guid id,
        CancellationToken cancellationToken)
    {
        var endpoint = await _endpointRepository.GetByIdAsync(id, cancellationToken);

        if (endpoint is null)
            return NotFound(new { error = "Endpoint not found." });

        var results = await _checkResultRepository.GetByEndpointIdAsync(id, cancellationToken);

        return Ok(results);
    }
    
    [HttpGet("{id:guid}/status")]
    public async Task<IActionResult> GetEndpointStatus(
        Guid id,
        CancellationToken cancellationToken)
    {
        var endpoint = await _endpointRepository.GetByIdAsync(id, cancellationToken);

        if (endpoint is null)
            return NotFound(new { error = "Endpoint not found." });

        var results = await _checkResultRepository.GetByEndpointIdAsync(id, cancellationToken);

        if (results.Count == 0)
        {
            return Ok(new GetEndpointStatusResponse
            {
                EndpointId = id,
                IsHealthy = false,
                UptimePercentage = 0,
                AverageResponseTimeMs = 0,
                TotalChecks = 0,
                FailedChecks = 0
            });
        }

        var lastResult = results[0];
        var successfulChecks = results.Count(x => x.IsSuccess);
        var failedChecks = results.Count - successfulChecks;

        return Ok(new GetEndpointStatusResponse
        {
            EndpointId = id,
            IsHealthy = lastResult.IsSuccess,
            LastStatusCode = lastResult.StatusCode,
            LastResponseTimeMs = lastResult.ResponseTimeMs,
            UptimePercentage = Math.Round((double)successfulChecks / results.Count * 100, 2),
            AverageResponseTimeMs = Math.Round(results.Average(x => x.ResponseTimeMs), 2),
            TotalChecks = results.Count,
            FailedChecks = failedChecks
        });
    }
    
    [HttpGet("{id:guid}/latest")]
    public async Task<IActionResult> GetLatestResult(
        Guid id,
        CancellationToken cancellationToken)
    {
        var endpoint = await _endpointRepository.GetByIdAsync(id, cancellationToken);

        if (endpoint is null)
            return NotFound(new { error = "Endpoint not found." });

        var latestResult = await _checkResultRepository.GetLatestByEndpointIdAsync(
            id,
            cancellationToken);

        if (latestResult is null)
            return NotFound(new { error = "No check results found for this endpoint." });

        return Ok(latestResult);
    }

    [HttpGet("{id:guid}/history")]
    public async Task<IActionResult> GetEndpointHistory(
        Guid id,
        [FromQuery] int take = 100,
        CancellationToken cancellationToken = default)
    {
        var endpoint = await _endpointRepository.GetByIdAsync(id, cancellationToken);

        if (endpoint is null)
            return NotFound(new { error = "Endpoint not found." });

        if (take is < 1 or > 1000)
            return BadRequest(new { error = "Take must be between 1 and 1000." });

        var history = await _checkResultRepository.GetHistoryByEndpointIdAsync(
            id,
            take,
            cancellationToken);

        return Ok(history);
    }
}