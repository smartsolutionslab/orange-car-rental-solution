# Orange Car Rental - API Gateway

YARP-based reverse proxy that routes requests to backend microservices using Aspire service discovery.

## Features

- **Service Discovery Integration** - Automatically discovers Fleet and Reservations APIs through Aspire
- **Dynamic Routing** - Routes `/api/vehicles/*` to Fleet API and `/api/reservations/*` to Reservations API
- **CORS Support** - Configured for frontend applications on ports 4300-4302
- **Health Monitoring** - `/health` endpoint for load balancers
- **Scalar UI** - Interactive API documentation (development only)

## Configuration

### Service Discovery

The gateway uses `Microsoft.Extensions.ServiceDiscovery` to automatically resolve backend service endpoints:

```csharp
builder.Services.AddServiceDiscovery();
builder.Services.ConfigureHttpClientDefaults(http =>
{
    http.AddServiceDiscovery();
});
```

### Routing Configuration (`appsettings.json`)

```json
{
  "ReverseProxy": {
    "Routes": {
      "fleet-route": {
        "ClusterId": "fleet-cluster",
        "Match": {
          "Path": "/api/vehicles/{**catch-all}"
        }
      },
      "reservations-route": {
        "ClusterId": "reservations-cluster",
        "Match": {
          "Path": "/api/reservations/{**catch-all}"
        }
      }
    },
    "Clusters": {
      "fleet-cluster": {
        "Destinations": {
          "fleet-api": {
            "Address": "https+http://fleet-api"
          }
        }
      },
      "reservations-cluster": {
        "Destinations": {
          "reservations-api": {
            "Address": "https+http://reservations-api"
          }
        }
      }
    }
  }
}
```

**Key Points:**
- `https+http://fleet-api` format enables service discovery to resolve the actual endpoint
- Service names (`fleet-api`, `reservations-api`) match the Aspire resource names
- `{**catch-all}` pattern forwards all sub-paths to backend services

## How It Works

1. **Aspire Discovery** - Gateway receives service endpoint information from Aspire AppHost
2. **YARP Proxy** - YARP resolves `fleet-api` and `reservations-api` through service discovery
3. **Dynamic Routing** - Incoming requests are matched to routes and forwarded to appropriate services
4. **Response Forwarding** - Backend responses are returned to clients

## Request Flow

```
Frontend (localhost:4300)
  ↓
API Gateway (localhost:5002)
  ↓ /api/vehicles/* → Fleet API
  ↓ /api/reservations/* → Reservations API
```

## Endpoints

### Health Check
```
GET /health
```
Returns gateway health status.

### Proxied Routes
- `GET /api/vehicles` → Fleet API vehicle search
- `POST /api/reservations` → Reservations API create reservation
- `GET /api/reservations/{id}` → Reservations API get reservation

## Development

### Running Locally

The gateway is automatically started by Aspire AppHost:

```bash
cd src/backend/AppHost/OrangeCarRental.AppHost
dotnet run
```

Access the gateway at `http://localhost:5002`

### Testing

Integration tests validate gateway routing:

```bash
cd src/backend/Tests/OrangeCarRental.IntegrationTests
dotnet test --filter ClassName=ApiGatewayTests
```

## CORS Configuration

Configured to accept requests from:
- `http://localhost:4300` (Shell)
- `http://localhost:4301` (Public Portal)
- `http://localhost:4302` (Call Center Portal)
- `https://localhost:4300`
- `https://localhost:4301`
- `https://localhost:4302`

## Monitoring

Enable detailed YARP logging in `appsettings.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Yarp": "Debug"
    }
  }
}
```

## Troubleshooting

### Service not found errors

**Problem:** `503 Service Unavailable` or service discovery failures

**Solutions:**
1. Ensure backend services are running through Aspire
2. Check service names in `appsettings.json` match Aspire resource names
3. Verify AppHost configuration has `.WithReference()` calls

### CORS errors

**Problem:** Browser blocks requests due to CORS

**Solutions:**
1. Check frontend origin is in CORS allowed origins list
2. Ensure credentials support is enabled for cookie-based auth
3. Verify `app.UseCors()` is called before `app.MapReverseProxy()`

### Route not matching

**Problem:** 404 errors for valid endpoints

**Solutions:**
1. Check route path patterns in `appsettings.json`
2. Verify `{**catch-all}` pattern is present for sub-paths
3. Test route matching with YARP's debug logging enabled
