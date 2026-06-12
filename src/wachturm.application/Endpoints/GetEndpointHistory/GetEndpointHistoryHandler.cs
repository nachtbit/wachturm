using wachturm.Application.Abstractions;
using wachturm.Application.CheckResults;
using wachturm.Application.Common;

namespace wachturm.Application.Endpoints.GetEndpointHistory;

public sealed class GetEndpointHistoryHandler
{
    private readonly IMonitoredEndpointRepository _endpointRepository;
    private readonly ICheckResultRepository _checkResultRepository;

    public GetEndpointHistoryHandler(
        IMonitoredEndpointRepository endpointRepository,
        ICheckResultRepository checkResultRepository)
    {
        _endpointRepository = endpointRepository;
        _checkResultRepository = checkResultRepository;
    }

    public async Task<Result<List<CheckResultResponse>>> HandleAsync(
        Guid id,
        int take,
        CancellationToken cancellationToken = default)
    {
        if (take is < 1 or > 1000)
            return Result<List<CheckResultResponse>>.Failure("Take must be between 1 and 1000.");

        var endpoint = await _endpointRepository.GetByIdAsync(id, cancellationToken);

        if (endpoint is null)
            return Result<List<CheckResultResponse>>.Failure("Endpoint not found.");

        var history = await _checkResultRepository.GetHistoryByEndpointIdAsync(id, take, cancellationToken);

        return Result<List<CheckResultResponse>>.Success(
            history.Select(CheckResultResponse.FromEntity).ToList());
    }
}
