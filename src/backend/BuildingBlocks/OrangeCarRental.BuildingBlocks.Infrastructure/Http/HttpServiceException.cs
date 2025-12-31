using System.Net;

namespace SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Infrastructure.Http;

/// <summary>
///     Exception thrown when an HTTP service call fails.
///     Provides structured information about the failure for logging and error handling.
/// </summary>
public sealed class HttpServiceException : Exception
{
    /// <summary>
    ///     The HTTP status code returned by the service.
    /// </summary>
    public HttpStatusCode StatusCode { get; }

    /// <summary>
    ///     The name of the service that was called.
    /// </summary>
    public string ServiceName { get; }

    /// <summary>
    ///     The request URI that failed.
    /// </summary>
    public string RequestUri { get; }

    /// <summary>
    ///     The response body content, if available.
    /// </summary>
    public string? ResponseContent { get; }

    public HttpServiceException(
        string serviceName,
        string requestUri,
        HttpStatusCode statusCode,
        string? responseContent = null)
        : base(FormatMessage(serviceName, requestUri, statusCode, responseContent))
    {
        ServiceName = serviceName;
        RequestUri = requestUri;
        StatusCode = statusCode;
        ResponseContent = responseContent;
    }

    public HttpServiceException(
        string serviceName,
        string requestUri,
        HttpStatusCode statusCode,
        string? responseContent,
        Exception innerException)
        : base(FormatMessage(serviceName, requestUri, statusCode, responseContent), innerException)
    {
        ServiceName = serviceName;
        RequestUri = requestUri;
        StatusCode = statusCode;
        ResponseContent = responseContent;
    }

    private static string FormatMessage(
        string serviceName,
        string requestUri,
        HttpStatusCode statusCode,
        string? responseContent)
    {
        var message = $"{serviceName} request to '{requestUri}' failed with status {(int)statusCode} ({statusCode})";

        if (!string.IsNullOrWhiteSpace(responseContent))
        {
            message += $": {responseContent}";
        }

        return message;
    }
}
