# Orange Car Rental - Monitoring Setup

Complete monitoring solution using Prometheus and Grafana for all microservices.

## 📊 Components

### Prometheus
- **Port:** 9090
- **Purpose:** Metrics collection and alerting
- **URL:** http://localhost:9090

### Grafana
- **Port:** 3000
- **Purpose:** Visualization and dashboards
- **URL:** http://localhost:3000
- **Default Credentials:**
  - Username: `admin`
  - Password: `admin`

### Node Exporter (Optional)
- **Port:** 9100
- **Purpose:** System-level metrics (CPU, Memory, Disk)

## 🚀 Quick Start

### 1. Start Monitoring Stack

```bash
# From project root
docker-compose -f docker-compose.monitoring.yml up -d
```

### 2. Access Dashboards

- **Prometheus:** http://localhost:9090
- **Grafana:** http://localhost:3000 (admin/admin)

### 3. Verify Metrics Collection

In Prometheus UI:
1. Go to Status → Targets
2. Verify all services are "UP"

## 📈 Monitored Services

The monitoring stack tracks these services:

| Service | Port | Metrics Path |
|---------|------|--------------|
| API Gateway | 5000 | /metrics |
| Customers Service | 5001 | /metrics |
| Fleet Service | 5002 | /metrics |
| Location Service | 5003 | /metrics |
| Reservations Service | 5004 | /metrics |
| Pricing Service | 5005 | /metrics |
| Payments Service | 5006 | /metrics |
| Notifications Service | 5007 | /metrics |

## 🔔 Alerts Configured

### Critical Alerts
- **ServiceDown:** Service unavailable for >1 minute
- **DatabaseConnectionPoolExhausted:** Connection pool >90% utilized

### Warning Alerts
- **HighErrorRate:** Error rate >5% for 5 minutes
- **HighResponseTime:** 95th percentile >1 second
- **HighMemoryUsage:** Memory usage >500MB

### Info Alerts
- **HighAPIGatewayTraffic:** >1000 requests/second

## 📊 Pre-configured Dashboards

### Orange Car Rental Dashboard
Located in Grafana at: Home → Dashboards → Orange Car Rental

**Panels Include:**
- Request rate per service
- Error rate trends
- Response time percentiles (p50, p95, p99)
- Memory usage
- Active database connections
- HTTP status codes distribution

## 🛠️ Configuration Files

```
monitoring/
├── prometheus.yml           # Prometheus configuration
├── alert-rules.yml         # Alert rules
├── grafana-dashboard.json  # Pre-built dashboard
├── grafana-datasources.yml # Grafana datasource config
└── README.md              # This file
```

## 🔧 Customization

### Adding New Service to Monitor

Edit `monitoring/prometheus.yml`:

```yaml
- job_name: 'your-new-service'
  metrics_path: '/metrics'
  static_configs:
    - targets: ['host.docker.internal:PORT']
```

### Adding Custom Alerts

Edit `monitoring/alert-rules.yml`:

```yaml
- alert: YourAlertName
  expr: your_metric_query
  for: 5m
  labels:
    severity: warning
  annotations:
    summary: "Alert summary"
    description: "Detailed description"
```

### Reload Configuration

```bash
# Reload Prometheus config without restart
curl -X POST http://localhost:9090/-/reload
```

## 📝 Example Queries

### Request Rate by Service
```promql
rate(http_requests_total[5m])
```

### Error Rate
```promql
rate(http_requests_total{status=~"5.."}[5m])
```

### 95th Percentile Response Time
```promql
histogram_quantile(0.95, rate(http_request_duration_seconds_bucket[5m]))
```

### Memory Usage
```promql
process_resident_memory_bytes / 1024 / 1024
```

## 🐛 Troubleshooting

### Services showing as "DOWN" in Prometheus

1. Check if your microservices are running
2. Verify they expose `/metrics` endpoint
3. Check firewall/network connectivity
4. Verify port numbers in `prometheus.yml`

### Grafana Dashboard Not Loading

1. Ensure Prometheus datasource is configured
2. Check Prometheus URL in datasource: `http://prometheus:9090`
3. Verify dashboard JSON is valid

### No Data in Dashboards

1. Check Prometheus targets: http://localhost:9090/targets
2. Verify services are exposing metrics
3. Check time range selector in Grafana

## 📊 Metrics Exposed by Services

All .NET services expose these standard metrics:

- `http_requests_total` - Total HTTP requests
- `http_request_duration_seconds` - Request duration
- `process_cpu_usage` - CPU usage
- `process_memory_usage` - Memory usage
- `dotnet_collection_count_total` - GC collections
- `dotnet_threadpool_*` - Thread pool metrics

## 🔐 Security

### Production Deployment

1. **Change default Grafana password:**
   ```bash
   docker exec -it orange-car-rental-grafana grafana-cli admin reset-admin-password newpassword
   ```

2. **Enable authentication** in Prometheus (use reverse proxy)

3. **Use HTTPS** for both Prometheus and Grafana

4. **Restrict network access** to monitoring ports

## 📚 Additional Resources

- [Prometheus Documentation](https://prometheus.io/docs/)
- [Grafana Documentation](https://grafana.com/docs/)
- [Prometheus Best Practices](https://prometheus.io/docs/practices/)
- [.NET Metrics](https://docs.microsoft.com/en-us/dotnet/core/diagnostics/metrics)

## 🎯 Next Steps

1. Start the monitoring stack
2. Verify all services are being scraped
3. Explore the pre-built dashboard
4. Customize alerts for your needs
5. Set up alert notifications (email, Slack, etc.)
