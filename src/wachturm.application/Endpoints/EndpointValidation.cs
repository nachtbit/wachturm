using wachturm.Application.Common;

namespace wachturm.Application.Endpoints;

public static class EndpointValidation
{
    private static readonly HashSet<string> AllowedMethods = new(StringComparer.OrdinalIgnoreCase)
    {
        "GET",
        "POST",
        "PUT",
        "PATCH",
        "DELETE",
        "HEAD"
    };

    public static Result Validate(string name, string url, string method, int intervalSeconds, int timeoutThresholdMs)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure("Endpoint name is required.");

        if (string.IsNullOrWhiteSpace(url))
            return Result.Failure("Endpoint URL is required.");

        if (!Uri.TryCreate(url.Trim(), UriKind.Absolute, out var uri) ||
            (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
            return Result.Failure("Endpoint URL must be a valid HTTP or HTTPS URL.");

        if (string.IsNullOrWhiteSpace(method))
            return Result.Failure("HTTP method is required.");

        if (!AllowedMethods.Contains(method.Trim()))
            return Result.Failure("HTTP method must be one of: GET, POST, PUT, PATCH, DELETE, HEAD.");

        if (intervalSeconds is < 5 or > 86400)
            return Result.Failure("Interval must be between 5 and 86400 seconds.");

        if (timeoutThresholdMs is < 100 or > 120000)
            return Result.Failure("Timeout threshold must be between 100 and 120000 ms.");

        return Result.Success();
    }
}
