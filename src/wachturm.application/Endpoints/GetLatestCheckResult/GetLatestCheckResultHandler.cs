using wachturm.Application.Abstractions;
using wachturm.Application.CheckResults;
using wachturm.Application.Common;

namespace wachturm.Application.Endpoints.GetLatestCheckResult;

public sealed class GetLatestCheckResultHandler
{
    private readonly IMonitoredEndpointRepository _endpointRepository;
    private readonly ICheckResultRepository _checkResultRepository;

    public GetLatestCheckResultHandler(
        IMonitoredEndpointRepository endpointRepository,
        ICheckResultRepository checkResultRepository)
    {
        _endpointRepository = endpointRepository;
        _checkResultRepository = checkResultRepository;
    }

    public async Task<Result<CheckResultResponse>> HandleAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var endpoint = await _endpointRepository.GetByIdAsync(id, cancellationToken);

        if (endpoint is null)
            return Result<CheckResultResponse>.Failure("Endpoint not found.");

        var latestResult = await _checkResultRepository.GetLatestByEndpointIdAsync(id, cancellationToken);

        return latestResult is null
            ? Result<CheckResultResponse>.Failure("No check results found for this endpoint.")
            : Result<CheckResultResponse>.Success(CheckResultResponse.FromEntity(latestResult));
    }
}
