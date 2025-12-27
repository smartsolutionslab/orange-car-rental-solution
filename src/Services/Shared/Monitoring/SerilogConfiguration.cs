using Serilog;
using Serilog.Events;
using Serilog.Formatting.Json;
using Serilog.Sinks.SystemConsole.Themes;

namespace OrangeCarRental.Shared.Monitoring;

/// <summary>
/// Centralized Serilog logging configuration
/// </summary>
public static class SerilogConfiguration
{
    public static LoggerConfiguration ConfigureLogging(
        LoggerConfiguration loggerConfig,
        string serviceName,
        string environment,
        string? applicationInsightsKey = null)
    {
        var isDevelopment = environment.Equals("Development", StringComparison.OrdinalIgnoreCase);

        loggerConfig
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
            .MinimumLevel.Override("System", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .Enrich.WithProperty("ServiceName", serviceName)
            .Enrich.WithProperty("Environment", environment)
            .Enrich.WithMachineName()
            .Enrich.WithThreadId();

        // Console logging (formatted for development, JSON for production)
        if (isDevelopment)
        {
            loggerConfig.WriteTo.Console(
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}",
                theme: AnsiConsoleTheme.Code);
        }
        else
        {
            loggerConfig.WriteTo.Console(new JsonFormatter());
        }

        // File logging (structured JSON)
        loggerConfig.WriteTo.File(
            new JsonFormatter(),
            path: $"logs/{serviceName}-.json",
            rollingInterval: RollingInterval.Day,
            retainedFileCountLimit: 7,
            fileSizeLimitBytes: 100 * 1024 * 1024, // 100 MB
            rollOnFileSizeLimit: true);

        // Application Insights (if configured)
        if (!string.IsNullOrEmpty(applicationInsightsKey))
        {
            loggerConfig.WriteTo.ApplicationInsights(
                instrumentationKey: applicationInsightsKey,
                TelemetryConverter.Traces);
        }

        return loggerConfig;
    }

    /// <summary>
    /// Log request details for monitoring
    /// </summary>
    public static ILogger ForContext<T>()
    {
        return Log.ForContext<T>();
    }

    /// <summary>
    /// Log performance metrics
    /// </summary>
    public static IDisposable BeginTimedOperation(this ILogger logger, string operationName)
    {
        return logger.BeginTimedOperation(operationName, LogEventLevel.Information);
    }

    /// <summary>
    /// Log performance metrics with custom level
    /// </summary>
    public static IDisposable BeginTimedOperation(
        this ILogger logger,
        string operationName,
        LogEventLevel level)
    {
        return new TimedOperation(logger, operationName, level);
    }

    private class TimedOperation : IDisposable
    {
        private readonly ILogger _logger;
        private readonly string _operationName;
        private readonly LogEventLevel _level;
        private readonly System.Diagnostics.Stopwatch _stopwatch;

        public TimedOperation(ILogger logger, string operationName, LogEventLevel level)
        {
            _logger = logger;
            _operationName = operationName;
            _level = level;
            _stopwatch = System.Diagnostics.Stopwatch.StartNew();
        }

        public void Dispose()
        {
            _stopwatch.Stop();
            _logger.Write(
                _level,
                "Operation {OperationName} completed in {ElapsedMs}ms",
                _operationName,
                _stopwatch.ElapsedMilliseconds);
        }
    }
}
