using Microsoft.AspNetCore.Mvc;
using wachturm.Application.Common;
using wachturm.Application.Endpoints.ActivateEndpoint;
using wachturm.Application.Endpoints.CreateEndpoint;
using wachturm.Application.Endpoints.DeactivateEndpoint;
using wachturm.Application.Endpoints.DeleteEndpoint;
using wachturm.Application.Endpoints.GetEndpointById;
using wachturm.Application.Endpoints.GetEndpointHistory;
using wachturm.Application.Endpoints.GetEndpoints;
using wachturm.Application.Endpoints.GetEndpointStatus;
using wachturm.Application.Endpoints.GetLatestCheckResult;

namespace wachturm.Api.Controllers;

[ApiController]
[Route("api/endpoints")]
public sealed class EndpointsController : ControllerBase
{
    private readonly CreateEndpointHandler _createEndpointHandler;
    private readonly GetEndpointsHandler _getEndpointsHandler;
    private readonly GetEndpointByIdHandler _getEndpointByIdHandler;
    private readonly ActivateEndpointHandler _activateEndpointHandler;
    private readonly DeactivateEndpointHandler _deactivateEndpointHandler;
    private readonly DeleteEndpointHandler _deleteEndpointHandler;
    private readonly GetEndpointStatusHandler _getEndpointStatusHandler;
    private readonly GetEndpointHistoryHandler _getEndpointHistoryHandler;
    private readonly GetLatestCheckResultHandler _getLatestCheckResultHandler;

    public EndpointsController(
        CreateEndpointHandler createEndpointHandler,
        GetEndpointsHandler getEndpointsHandler,
        GetEndpointByIdHandler getEndpointByIdHandler,
        ActivateEndpointHandler activateEndpointHandler,
        DeactivateEndpointHandler deactivateEndpointHandler,
        DeleteEndpointHandler deleteEndpointHandler,
        GetEndpointStatusHandler getEndpointStatusHandler,
        GetEndpointHistoryHandler getEndpointHistoryHandler,
        GetLatestCheckResultHandler getLatestCheckResultHandler)
    {
        _createEndpointHandler = createEndpointHandler;
        _getEndpointsHandler = getEndpointsHandler;
        _getEndpointByIdHandler = getEndpointByIdHandler;
        _activateEndpointHandler = activateEndpointHandler;
        _deactivateEndpointHandler = deactivateEndpointHandler;
        _deleteEndpointHandler = deleteEndpointHandler;
        _getEndpointStatusHandler = getEndpointStatusHandler;
        _getEndpointHistoryHandler = getEndpointHistoryHandler;
        _getLatestCheckResultHandler = getLatestCheckResultHandler;
    }

    [HttpPost]
    public async Task<IActionResult> CreateEndpoint(
        [FromBody] CreateEndpointRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _createEndpointHandler.HandleAsync(request, cancellationToken);

        if (result.IsFailure)
            return ToErrorResponse(result);

        return CreatedAtAction(
            nameof(GetEndpointById),
            new { id = result.Value!.Id },
            result.Value);
    }

    [HttpGet]
    public async Task<IActionResult> GetEndpoints(CancellationToken cancellationToken)
    {
        var endpoints = await _getEndpointsHandler.HandleAsync(cancellationToken);
        return Ok(endpoints);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetEndpointById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _getEndpointByIdHandler.HandleAsync(id, cancellationToken);
        return result.IsFailure ? ToErrorResponse(result) : Ok(result.Value);
    }
    
    [HttpPatch("{id:guid}/activate")]
    public async Task<IActionResult> ActivateEndpoint(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _activateEndpointHandler.HandleAsync(id, cancellationToken);
        return result.IsFailure ? ToErrorResponse(result) : Ok(result.Value);
    }

    [HttpPatch("{id:guid}/deactivate")]
    public async Task<IActionResult> DeactivateEndpoint(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _deactivateEndpointHandler.HandleAsync(id, cancellationToken);
        return result.IsFailure ? ToErrorResponse(result) : Ok(result.Value);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteEndpoint(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _deleteEndpointHandler.HandleAsync(id, cancellationToken);
        return result.IsFailure ? ToErrorResponse(result) : NoContent();
    }
    
    [HttpGet("{id:guid}/results")]
    public async Task<IActionResult> GetEndpointResults(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _getEndpointHistoryHandler.HandleAsync(id, 100, cancellationToken);
        return result.IsFailure ? ToErrorResponse(result) : Ok(result.Value);
    }
    
    [HttpGet("{id:guid}/status")]
    public async Task<IActionResult> GetEndpointStatus(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _getEndpointStatusHandler.HandleAsync(id, cancellationToken);
        return result.IsFailure ? ToErrorResponse(result) : Ok(result.Value);
    }
    
    [HttpGet("{id:guid}/latest")]
    public async Task<IActionResult> GetLatestResult(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _getLatestCheckResultHandler.HandleAsync(id, cancellationToken);
        return result.IsFailure ? ToErrorResponse(result) : Ok(result.Value);
    }

    [HttpGet("{id:guid}/history")]
    public async Task<IActionResult> GetEndpointHistory(
        Guid id,
        [FromQuery] int take = 100,
        CancellationToken cancellationToken = default)
    {
        var result = await _getEndpointHistoryHandler.HandleAsync(id, take, cancellationToken);
        return result.IsFailure ? ToErrorResponse(result) : Ok(result.Value);
    }

    private IActionResult ToErrorResponse(Result result)
    {
        if (string.Equals(result.Error, "Endpoint not found.", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(result.Error, "No check results found for this endpoint.", StringComparison.OrdinalIgnoreCase))
        {
            return NotFound(new { error = result.Error });
        }

        return BadRequest(new { error = result.Error });
    }
}
