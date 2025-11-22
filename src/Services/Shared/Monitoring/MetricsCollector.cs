using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace OrangeCarRental.Shared.Monitoring;

/// <summary>
/// Custom metrics collection for application monitoring
/// </summary>
public class MetricsCollector
{
    private readonly Meter _meter;
    private readonly Counter<long> _requestCounter;
    private readonly Counter<long> _errorCounter;
    private readonly Histogram<double> _requestDuration;
    private readonly Histogram<double> _databaseQueryDuration;
    private readonly ObservableGauge<int> _activeRequests;
    private int _activeRequestCount;

    public MetricsCollector(string serviceName)
    {
        _meter = new Meter($"OrangeCarRental.{serviceName}", "1.0.0");

        // Counters
        _requestCounter = _meter.CreateCounter<long>(
            "requests_total",
            description: "Total number of HTTP requests");

        _errorCounter = _meter.CreateCounter<long>(
            "errors_total",
            description: "Total number of errors");

        // Histograms
        _requestDuration = _meter.CreateHistogram<double>(
            "request_duration_seconds",
            unit: "seconds",
            description: "HTTP request duration in seconds");

        _databaseQueryDuration = _meter.CreateHistogram<double>(
            "database_query_duration_seconds",
            unit: "seconds",
            description: "Database query duration in seconds");

        // Gauges
        _activeRequests = _meter.CreateObservableGauge<int>(
            "active_requests",
            () => _activeRequestCount,
            description: "Number of active HTTP requests");
    }

    /// <summary>
    /// Record an HTTP request
    /// </summary>
    public void RecordRequest(string method, string path, int statusCode, double durationSeconds)
    {
        _requestCounter.Add(1, new KeyValuePair<string, object?>("method", method),
                               new KeyValuePair<string, object?>("path", path),
                               new KeyValuePair<string, object?>("status", statusCode));

        _requestDuration.Record(durationSeconds,
                                new KeyValuePair<string, object?>("method", method),
                                new KeyValuePair<string, object?>("path", path),
                                new KeyValuePair<string, object?>("status", statusCode));
    }

    /// <summary>
    /// Record an error
    /// </summary>
    public void RecordError(string errorType, string operation)
    {
        _errorCounter.Add(1, new KeyValuePair<string, object?>("type", errorType),
                             new KeyValuePair<string, object?>("operation", operation));
    }

    /// <summary>
    /// Record database query duration
    /// </summary>
    public void RecordDatabaseQuery(string queryType, double durationSeconds)
    {
        _databaseQueryDuration.Record(durationSeconds,
                                      new KeyValuePair<string, object?>("query_type", queryType));
    }

    /// <summary>
    /// Start tracking an active request
    /// </summary>
    public IDisposable TrackActiveRequest()
    {
        Interlocked.Increment(ref _activeRequestCount);
        return new ActiveRequestTracker(this);
    }

    private void DecrementActiveRequests()
    {
        Interlocked.Decrement(ref _activeRequestCount);
    }

    private class ActiveRequestTracker : IDisposable
    {
        private readonly MetricsCollector _collector;

        public ActiveRequestTracker(MetricsCollector collector)
        {
            _collector = collector;
        }

        public void Dispose()
        {
            _collector.DecrementActiveRequests();
        }
    }
}

/// <summary>
/// Middleware for automatic request metrics collection
/// </summary>
public class MetricsMiddleware
{
    private readonly RequestDelegate _next;
    private readonly MetricsCollector _metricsCollector;

    public MetricsMiddleware(RequestDelegate next, MetricsCollector metricsCollector)
    {
        _next = next;
        _metricsCollector = metricsCollector;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        using var activeRequest = _metricsCollector.TrackActiveRequest();

        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();
            _metricsCollector.RecordRequest(
                context.Request.Method,
                context.Request.Path,
                context.Response.StatusCode,
                stopwatch.Elapsed.TotalSeconds);
        }
    }
}
