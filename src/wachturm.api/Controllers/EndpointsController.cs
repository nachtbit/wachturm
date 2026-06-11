using Microsoft.AspNetCore.Mvc;
using wachturm.Application.Abstractions;
using wachturm.Application.Endpoints.CreateEndpoint;

namespace wachturm.Api.Controllers;

[ApiController]
[Route("api/endpoints")]
public sealed class EndpointsController : ControllerBase
{
    private readonly CreateEndpointHandler _createEndpointHandler;
    private readonly IMonitoredEndpointRepository _endpointRepository;

    public EndpointsController(
        CreateEndpointHandler createEndpointHandler,
        IMonitoredEndpointRepository endpointRepository)
    {
        _createEndpointHandler = createEndpointHandler;
        _endpointRepository = endpointRepository;
    }

    [HttpPost]
    public async Task<IActionResult> CreateEndpoint(
        [FromBody] CreateEndpointRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _createEndpointHandler.HandleAsync(request, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(new
            {
                error = result.Error
            });
        }

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
        {
            return NotFound(new
            {
                error = "Endpoint not found."
            });
        }
        
        return Ok(endpoint);
    }
}