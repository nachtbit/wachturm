using Microsoft.AspNetCore.Mvc;
using wachturm.Application.Dashboard;

namespace wachturm.Api.Controllers;

[ApiController]
[Route("api/dashboard")]
public sealed class DashboardController : ControllerBase
{
    private readonly GetDashboardSummaryHandler _getDashboardSummaryHandler;

    public DashboardController(GetDashboardSummaryHandler getDashboardSummaryHandler)
    {
        _getDashboardSummaryHandler = getDashboardSummaryHandler;
    }

    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary(CancellationToken cancellationToken)
    {
        var summary = await _getDashboardSummaryHandler.HandleAsync(cancellationToken);
        return Ok(summary);
    }
}
