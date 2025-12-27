using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace OrangeCarRental.Shared.Monitoring;

/// <summary>
/// Configuration for Azure Application Insights monitoring
/// </summary>
public static class ApplicationInsightsConfiguration
{
    public static IServiceCollection AddApplicationInsightsMonitoring(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration["ApplicationInsights:ConnectionString"];

        if (!string.IsNullOrEmpty(connectionString))
        {
            // Add Application Insights telemetry
            services.AddApplicationInsightsTelemetry(options =>
            {
                options.ConnectionString = connectionString;
                options.EnableAdaptiveSampling = true;
                options.EnableQuickPulseMetricStream = true;
                options.EnableDependencyTrackingTelemetryModule = true;
                options.EnablePerformanceCounterCollectionModule = true;
            });

            // Add custom telemetry initializers
            services.AddSingleton<ITelemetryInitializer, CloudRoleTelemetryInitializer>();
            services.AddSingleton<ITelemetryInitializer, UserTelemetryInitializer>();

            Log.Information("Application Insights monitoring enabled");
        }
        else
        {
            Log.Warning("Application Insights connection string not configured");
        }

        return services;
    }
}

/// <summary>
/// Adds cloud role name for better service identification in Application Insights
/// </summary>
public class CloudRoleTelemetryInitializer : ITelemetryInitializer
{
    private readonly string _roleName;

    public CloudRoleTelemetryInitializer(IConfiguration configuration)
    {
        _roleName = configuration["ServiceName"] ?? "OrangeCarRental";
    }

    public void Initialize(Microsoft.ApplicationInsights.Channel.ITelemetry telemetry)
    {
        telemetry.Context.Cloud.RoleName = _roleName;
    }
}

/// <summary>
/// Enriches telemetry with user information
/// </summary>
public class UserTelemetryInitializer : ITelemetryInitializer
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserTelemetryInitializer(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public void Initialize(Microsoft.ApplicationInsights.Channel.ITelemetry telemetry)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext?.User?.Identity?.IsAuthenticated == true)
        {
            telemetry.Context.User.AuthenticatedUserId = httpContext.User.Identity.Name;
        }
    }
}
