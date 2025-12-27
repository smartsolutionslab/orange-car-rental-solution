# Monitoring

## Local Development

For local development, .NET Aspire provides built-in observability:

```bash
cd src/backend/AppHost/OrangeCarRental.AppHost
dotnet run
```

The **Aspire Dashboard** provides:
- **Structured Logs** - Filter by service, level, or search
- **Distributed Traces** - End-to-end request tracking
- **Metrics** - Request rates, durations, resource usage
- **Resource Status** - Container and service health

Access the dashboard URL shown in the console output.

## Production (Azure)

For production environments, use Azure Application Insights:

- `azure-dashboard.json` - Pre-configured Azure dashboard
- `alert-rules.bicep` - Azure Monitor alert rules

Deploy with:
```bash
cd infrastructure/azure
az deployment sub create \
  --template-file main.bicep \
  --parameters @parameters.production.json
```

## Legacy Files

The following files are from a previous Prometheus/Grafana setup:
- `prometheus.yml`
- `grafana-dashboard.json`
- `grafana-datasources.yml`
- `alert-rules.yml`

These can be used if you prefer Prometheus/Grafana over Aspire's built-in monitoring.
