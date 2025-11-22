# Monitoring and Observability Guide

## Overview

The Orange Car Rental system implements comprehensive monitoring and observability using a combination of Azure Application Insights, custom metrics, structured logging, and health checks.

## Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                     Monitoring Stack                         │
├─────────────────────────────────────────────────────────────┤
│                                                               │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐      │
│  │ Health Checks│  │   Metrics    │  │    Logs      │      │
│  └──────┬───────┘  └──────┬───────┘  └──────┬───────┘      │
│         │                  │                  │               │
│         └──────────────────┼──────────────────┘               │
│                            │                                  │
│                  ┌─────────▼──────────┐                      │
│                  │ Application        │                      │
│                  │ Insights           │                      │
│                  └─────────┬──────────┘                      │
│                            │                                  │
│         ┌──────────────────┼──────────────────┐             │
│         │                  │                  │              │
│  ┌──────▼───────┐  ┌──────▼───────┐  ┌──────▼───────┐     │
│  │Azure Portal  │  │   Grafana    │  │   Alerting   │     │
│  │Dashboards    │  │  Dashboards  │  │   Rules      │     │
│  └──────────────┘  └──────────────┘  └──────────────┘     │
│                                                               │
└─────────────────────────────────────────────────────────────┘
```

## 1. Health Checks

### Implementation

All services implement standardized health checks:

- **Liveness Probe**: `/health/live` - Service is running
- **Readiness Probe**: `/health/ready` - Service is ready to accept traffic
- **Detailed Health**: `/health` - Comprehensive health status

### Health Check Components

#### Database Health Check
```csharp
// Checks:
// - Database connectivity
// - Query performance
// - Connection pool status

services.AddHealthChecks()
    .AddDbContextCheck<AppDbContext>("database")
    .AddCheck<DatabaseHealthCheck>("database-performance");
```

#### Keycloak Health Check
```csharp
// Checks:
// - Keycloak availability
// - Authentication endpoint reachability
// - Response time

services.AddHealthChecks()
    .AddCheck<KeycloakHealthCheck>("keycloak");
```

### Configuration

Add to `Program.cs` or `Startup.cs`:

```csharp
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
    ResultStatusCodes =
    {
        [HealthStatus.Healthy] = StatusCodes.Status200OK,
        [HealthStatus.Degraded] = StatusCodes.Status200OK,
        [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
    }
});

app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = _ => false // No checks, just alive
});

app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready")
});
```

### Kubernetes Integration

```yaml
livenessProbe:
  httpGet:
    path: /health/live
    port: 8080
  initialDelaySeconds: 10
  periodSeconds: 10

readinessProbe:
  httpGet:
    path: /health/ready
    port: 8080
  initialDelaySeconds: 5
  periodSeconds: 5
```

## 2. Application Insights

### Setup

#### Configuration

Add to `appsettings.json`:

```json
{
  "ApplicationInsights": {
    "ConnectionString": "InstrumentationKey=xxx;IngestionEndpoint=https://xxx.in.applicationinsights.azure.com/"
  },
  "ServiceName": "Vehicles.API"
}
```

#### Code Integration

```csharp
using OrangeCarRental.Shared.Monitoring;

var builder = WebApplication.CreateBuilder(args);

// Add Application Insights
builder.Services.AddApplicationInsightsMonitoring(builder.Configuration);

// Add custom metrics collector
builder.Services.AddSingleton<MetricsCollector>(sp =>
    new MetricsCollector(builder.Configuration["ServiceName"] ?? "Unknown"));

var app = builder.Build();

// Add metrics middleware
app.UseMiddleware<MetricsMiddleware>();
```

### Features Enabled

- ✅ Request tracking
- ✅ Exception tracking
- ✅ Dependency tracking (SQL, HTTP, external services)
- ✅ Performance counters
- ✅ Custom metrics
- ✅ User tracking
- ✅ Adaptive sampling
- ✅ Live metrics stream

## 3. Structured Logging

### Serilog Configuration

#### Setup

```csharp
using OrangeCarRental.Shared.Monitoring;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = SerilogConfiguration
    .ConfigureLogging(
        new LoggerConfiguration(),
        serviceName: "Vehicles.API",
        environment: builder.Environment.EnvironmentName,
        applicationInsightsKey: builder.Configuration["ApplicationInsights:InstrumentationKey"])
    .CreateLogger();

builder.Host.UseSerilog();
```

### Log Levels

| Level | When to Use |
|-------|-------------|
| **Verbose** | Detailed debugging information |
| **Debug** | Development diagnostic information |
| **Information** | General informational messages |
| **Warning** | Potential issues or deprecated usage |
| **Error** | Error events (exceptions, failures) |
| **Fatal** | Critical failures requiring immediate attention |

### Best Practices

```csharp
// ✅ DO: Use structured logging
_logger.LogInformation(
    "Reservation {ReservationId} created for customer {CustomerId}",
    reservationId,
    customerId);

// ❌ DON'T: Use string interpolation
_logger.LogInformation($"Reservation {reservationId} created for customer {customerId}");

// ✅ DO: Log with context
using (_logger.BeginScope(new Dictionary<string, object>
{
    ["UserId"] = userId,
    ["CorrelationId"] = correlationId
}))
{
    _logger.LogInformation("Processing user request");
}

// ✅ DO: Time operations
using (_logger.BeginTimedOperation("Database query"))
{
    // Perform database operation
}
```

### Log Queries (Application Insights)

```kusto
// Find errors in last hour
traces
| where timestamp > ago(1h)
| where severityLevel >= 3
| project timestamp, message, severityLevel, customDimensions
| order by timestamp desc

// Find slow requests
requests
| where timestamp > ago(1h)
| where duration > 2000
| project timestamp, name, duration, resultCode
| order by duration desc

// Exception analysis
exceptions
| where timestamp > ago(24h)
| summarize count() by type, outerMessage
| order by count_ desc
```

## 4. Custom Metrics

### Metrics Collected

#### Request Metrics
- `requests_total` - Total HTTP requests (by method, path, status)
- `request_duration_seconds` - Request duration histogram
- `active_requests` - Current active requests

#### Database Metrics
- `database_query_duration_seconds` - Query performance
- `database_connection_errors` - Connection failures

#### Business Metrics
- `reservations_created_total` - Total reservations
- `reservations_cancelled_total` - Cancelled reservations
- `vehicles_searched_total` - Vehicle searches
- `authentication_attempts_total` - Auth attempts (success/failure)

### Adding Custom Metrics

```csharp
public class ReservationService
{
    private readonly MetricsCollector _metrics;

    public ReservationService(MetricsCollector metrics)
    {
        _metrics = metrics;
    }

    public async Task CreateReservationAsync(Reservation reservation)
    {
        try
        {
            // Business logic
            await _repository.AddAsync(reservation);

            // Track success metric
            _metrics.RecordBusinessEvent("reservation_created", new Dictionary<string, string>
            {
                ["vehicle_category"] = reservation.CategoryCode,
                ["location"] = reservation.PickupLocationCode
            });
        }
        catch (Exception ex)
        {
            // Track error
            _metrics.RecordError(ex.GetType().Name, "CreateReservation");
            throw;
        }
    }
}
```

## 5. Dashboards

### Azure Portal Dashboard

Deploy the pre-configured dashboard:

```bash
az portal dashboard create \
  --resource-group orange-car-rental-rg \
  --name orange-car-rental-dashboard \
  --input-path monitoring/azure-dashboard.json
```

**Metrics Displayed:**
- Total requests (24h)
- Average response time
- Failed requests
- Availability percentage
- Database query performance
- Exception count

### Grafana Dashboard

Import the Grafana dashboard:

1. Open Grafana
2. Go to **Dashboards** → **Import**
3. Upload `monitoring/grafana-dashboard.json`
4. Select Prometheus data source

**Panels:**
- Request rate (req/s)
- P95 response time gauge
- Success rate gauge
- Active requests timeline
- Error rate by type
- Database query performance (P95)

## 6. Alerting

### Alert Rules

Deploy alert rules using Azure Bicep:

```bash
az deployment group create \
  --resource-group orange-car-rental-rg \
  --template-file monitoring/alert-rules.bicep \
  --parameters appInsightsName=orange-car-rental-ai \
               actionGroupId=/subscriptions/.../resourceGroups/.../providers/Microsoft.Insights/actionGroups/orange-alerts \
               environment=production
```

### Alert Types

| Alert | Threshold | Severity | Action |
|-------|-----------|----------|--------|
| High Error Rate | > 5% failed requests | 🟡 Warning | Email team |
| Slow Response | > 2s avg response time | 🟡 Warning | Email team |
| Low Availability | < 99% availability | 🔴 Critical | Page on-call |
| High Exceptions | > 100 exceptions/15min | 🟡 Warning | Email team |
| Database Slow | > 500ms avg query time | 🟡 Warning | Email team |
| High Memory | < 500MB available | 🟡 Warning | Email devops |

### Action Groups

Create action group for notifications:

```bash
az monitor action-group create \
  --name orange-alerts \
  --resource-group orange-car-rental-rg \
  --short-name OrangeAlert \
  --email-receiver name=DevOps email=devops@orange-rental.de \
  --email-receiver name=OnCall email=oncall@orange-rental.de \
  --webhook-receiver name=Slack service-uri=https://hooks.slack.com/services/YOUR/WEBHOOK/URL
```

### Notification Channels

- **Email**: devops@orange-rental.de, oncall@orange-rental.de
- **Slack**: #orange-car-rental-alerts
- **SMS**: On-call phone (critical alerts only)
- **PagerDuty**: Integration for critical incidents

## 7. Monitoring Best Practices

### Log Sensitive Data

❌ **Never log:**
- Passwords
- Credit card numbers
- Personal identification numbers
- Authentication tokens
- API keys

✅ **Safe to log:**
- User IDs (UUIDs)
- Reservation IDs
- Timestamps
- Error codes
- Performance metrics

### Performance Impact

- Use **adaptive sampling** to control costs
- Set appropriate **log levels** per environment
- Implement **rate limiting** on expensive operations
- Use **async logging** to avoid blocking

### Cost Management

| Environment | Sampling Rate | Retention |
|-------------|---------------|-----------|
| Development | 100% | 7 days |
| Staging | 100% | 30 days |
| Production | 20-50% (adaptive) | 90 days |

## 8. Troubleshooting

### High Response Times

1. **Check Application Insights** → Performance blade
2. **Identify slow dependencies** (database, external APIs)
3. **Review slow requests** in traces
4. **Check database query performance**

```kusto
dependencies
| where timestamp > ago(1h)
| where duration > 1000
| summarize avg(duration), count() by name
| order by avg_duration desc
```

### High Error Rate

1. **Check exceptions** in Application Insights
2. **Review error logs** in Serilog output
3. **Check health endpoints** for service status
4. **Review recent deployments** for changes

```kusto
exceptions
| where timestamp > ago(1h)
| summarize count() by type, outerMessage
| order by count_ desc
| take 10
```

### Database Issues

1. **Check database health** via `/health` endpoint
2. **Review slow queries** in Application Insights
3. **Check connection pool** metrics
4. **Verify database server** health

```kusto
dependencies
| where type == "SQL"
| where timestamp > ago(1h)
| summarize avg(duration), max(duration), count() by name
| order by avg_duration desc
```

## 9. Runbook

### Incident Response

#### Severity 1 (Critical - Service Down)

1. **Check health endpoints** for all services
2. **Review recent deployments** and rollback if needed
3. **Check Azure service health** for platform issues
4. **Review Application Insights** for errors
5. **Notify stakeholders** via incident channel

#### Severity 2 (High - Degraded Performance)

1. **Identify affected service** via metrics
2. **Review slow requests** and dependencies
3. **Check database performance**
4. **Scale resources** if needed
5. **Create incident ticket**

#### Severity 3 (Medium - Minor Issues)

1. **Document the issue** in monitoring system
2. **Create bug ticket** for engineering team
3. **Monitor for escalation**
4. **Schedule fix** for next sprint

### Common Issues

| Symptom | Likely Cause | Solution |
|---------|--------------|----------|
| 503 errors | Service unhealthy | Check health endpoint, restart service |
| Slow responses | Database slow | Review queries, add indexes, scale DB |
| High memory | Memory leak | Review application logs, restart, investigate |
| Auth failures | Keycloak down | Check Keycloak health, restart if needed |
| High CPU | Inefficient code | Profile application, optimize hot paths |

## 10. Resources

### Documentation
- [Application Insights Documentation](https://docs.microsoft.com/azure/azure-monitor/app/app-insights-overview)
- [Serilog Documentation](https://serilog.net/)
- [Grafana Documentation](https://grafana.com/docs/)
- [Prometheus Documentation](https://prometheus.io/docs/)

### Useful Queries

See `monitoring/useful-queries.kql` for Application Insights KQL queries.

### Dashboard URLs

- **Azure Portal**: https://portal.azure.com/#@tenant/dashboard/arm/subscriptions/.../dashboards/orange-car-rental
- **Grafana**: https://grafana.orange-rental.de/d/orange-car-rental
- **Application Insights**: https://portal.azure.com/#@tenant/resource/.../logs

## 11. Maintenance

### Weekly
- ✅ Review alert history
- ✅ Check dashboard for anomalies
- ✅ Review top errors
- ✅ Verify health check status

### Monthly
- ✅ Review and optimize log retention
- ✅ Analyze performance trends
- ✅ Update alert thresholds if needed
- ✅ Review monitoring costs

### Quarterly
- ✅ Review and update dashboards
- ✅ Audit alerting rules
- ✅ Update runbooks
- ✅ Training for new team members
