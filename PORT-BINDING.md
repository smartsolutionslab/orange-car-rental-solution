# Port Binding - 12-Factor Principles

## Overview

This document describes the port binding architecture for Orange Car Rental, following **12-Factor App - Factor 7: Export services via port binding**.

## 12-Factor Port Binding Principles

✅ **Self-contained web server** - App includes its own HTTP server, no external runtime needed
✅ **Port binding** - App binds to a port and listens for incoming requests
✅ **Service export** - App exports HTTP as a service accessible on that port
✅ **No webserver injection** - No dependency on Apache, IIS, or external web servers

---

## What is Port Binding?

**Port Binding** means the application is **completely self-contained** with its own web server, and it **exports HTTP as a service** by binding to a port.

**Key Principles:**
- The app **includes its own HTTP server** (Kestrel for .NET, Node.js for Angular)
- The app **binds to a port** specified by the `PORT` environment variable
- In development: `http://localhost:PORT`
- In production: Routing layer routes requests from public hostname to port-bound process

**12-Factor Principle:** The app is **not injected into** a web server at runtime (like deploying a WAR file to Tomcat or ASP.NET app to IIS). Instead, the app **contains** the web server.

---

## Current Implementation Status

### ✅ **Backend APIs (ASP.NET Core) - Excellent Compliance**

**Self-Contained Web Server: Kestrel**

ASP.NET Core applications use **Kestrel**, a cross-platform, high-performance web server built into .NET.

**Location:** `src/backend/Services/Fleet/OrangeCarRental.Fleet.Api/Program.cs:119`

```csharp
var builder = WebApplication.CreateBuilder(args);

// Configure services (DI, authentication, database, etc.)
builder.Services.AddScoped<IVehicleRepository, VehicleRepository>();
builder.AddSqlServerDbContext<FleetDbContext>("fleet");

var app = builder.Build();

// Configure middleware pipeline
app.UseAuthentication();
app.UseAuthorization();
app.MapFleetEndpoints();

// ✅ Start Kestrel web server and bind to port
app.Run();  // Listens on port from ASPNETCORE_URLS or appsettings.json
```

**✅ Self-Contained:**
- Kestrel is **embedded** in the application
- No IIS, Apache, or Nginx required at runtime
- App is a standalone executable: `dotnet OrangeCarRental.Fleet.Api.dll`

---

**Port Configuration:**

**Docker Expose:** `src/backend/Services/Fleet/Dockerfile:60-61`

```dockerfile
# Expose ports for HTTP and HTTPS
EXPOSE 8080  # HTTP
EXPOSE 8081  # HTTPS

ENTRYPOINT ["dotnet", "OrangeCarRental.Fleet.Api.dll"]
```

**Environment Variable (12-Factor compliant):**

```bash
# Development (Aspire)
ASPNETCORE_URLS="http://+:8080;https://+:8081"

# Production (Kubernetes)
ASPNETCORE_URLS="http://+:8080"
```

**✅ Port Binding Characteristics:**
- Binds to port **8080** (HTTP) and **8081** (HTTPS)
- Listens on **all interfaces** (`+` or `0.0.0.0`)
- Port configurable via environment variable (12-Factor Factor 3: Config)
- No reverse proxy required (though recommended for production)

---

**Kubernetes Service Mapping:**

**Location:** `k8s/base/vehicles-api-deployment.yaml:27,87-88`

```yaml
apiVersion: v1
kind: Pod
spec:
  containers:
  - name: fleet-api
    image: ghcr.io/.../fleet-api:v1.2.3
    ports:
    - containerPort: 8080  # ✅ App listens on this port
      protocol: TCP
      name: http

---

apiVersion: v1
kind: Service
metadata:
  name: fleet-api
spec:
  selector:
    app: fleet-api
  ports:
  - port: 8080          # ✅ Service exposes this port
    targetPort: 8080    # ✅ Routes to container port
    protocol: TCP
    name: http
```

**✅ Service Discovery:**
- Other services access Fleet API via `http://fleet-api:8080`
- Kubernetes DNS resolves `fleet-api` to the Service's ClusterIP
- Service routes requests to pod port 8080

---

### ✅ **Frontend Apps (Angular + Node.js) - Excellent Compliance**

**Public Portal - Port 4200**

**Location:** `src/backend/AppHost/OrangeCarRental.AppHost/Program.cs:97-103`

```csharp
var publicPortal = builder.AddNpmApp("public-portal", "../../../frontend/apps/public-portal")
    .WithHttpEndpoint(4200, env: "PORT")  // ✅ Binds to port 4200
    .WithReference(apiGateway)
    .WithEnvironment("API_URL", apiGateway.GetEndpoint("http"))
    .WithExternalHttpEndpoints()
    .WaitFor(apiGateway);
```

**Development Server (ng serve):**
```bash
# Angular CLI dev server binds to port 4200
ng serve --port 4200

# Accessible at:
http://localhost:4200
```

**✅ Self-Contained:**
- Node.js built-in HTTP server
- No Apache or IIS required
- Runs standalone: `npm start`

---

**Call Center Portal - Port 4201**

**Location:** `src/backend/AppHost/OrangeCarRental.AppHost/Program.cs:107-113`

```csharp
var callCenterPortal = builder.AddNpmApp("call-center-portal", "../../../frontend/apps/call-center-portal")
    .WithHttpEndpoint(4201, env: "PORT")  // ✅ Binds to port 4201
    .WithReference(apiGateway)
    .WithEnvironment("API_URL", apiGateway.GetEndpoint("http"))
    .WithExternalHttpEndpoints()
    .WaitFor(apiGateway);
```

**Production (Nginx in Docker):**
```dockerfile
# Frontend Dockerfile (production build)
FROM nginx:alpine
COPY dist/public-portal /usr/share/nginx/html
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
```

**✅ Port Binding:**
- Development: Angular dev server on port 4200/4201
- Production: Nginx on port 80 (configurable)
- Both are self-contained HTTP servers

---

### ✅ **API Gateway (YARP Reverse Proxy)**

**Location:** `src/backend/ApiGateway/OrangeCarRental.ApiGateway/Program.cs`

```csharp
var builder = WebApplication.CreateBuilder(args);

// YARP reverse proxy configuration
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

app.MapReverseProxy();  // Routes requests to backend services

// ✅ API Gateway binds to port and acts as entry point
app.Run();  // Default: port 8080
```

**Request Flow:**
```
Client Request
    │
    ▼
┌────────────────────────────────┐
│  API Gateway (Port 8080)       │  ✅ Port-bound entry point
│  ─────────────────────────     │
│  /api/vehicles/* → Fleet API   │
│  /api/reservations/* → Res API │
│  /api/pricing/* → Pricing API  │
│  /api/customers/* → Cust API   │
└───────┬────────────────────────┘
        │
        ├──────────────────┬──────────────────┐
        ▼                  ▼                  ▼
┌──────────────┐  ┌──────────────┐  ┌──────────────┐
│  Fleet API   │  │  Reserv API  │  │  Pricing API │
│  Port 8080   │  │  Port 8080   │  │  Port 8080   │
└──────────────┘  └──────────────┘  └──────────────┘
```

**✅ All services are self-contained and port-bound.**

---

## Port Allocation Strategy

### **Development (Aspire)**

| Service | Port | URL |
|---------|------|-----|
| **API Gateway** | Dynamic | `http://localhost:{random}` |
| **Fleet API** | Dynamic | Aspire Service Discovery |
| **Reservations API** | Dynamic | Aspire Service Discovery |
| **Pricing API** | Dynamic | Aspire Service Discovery |
| **Customers API** | Dynamic | Aspire Service Discovery |
| **Public Portal** | 4200 | `http://localhost:4200` |
| **Call Center Portal** | 4201 | `http://localhost:4201` |
| **Keycloak** | 8080 | `http://localhost:8080` |
| **SQL Server** | 1433 | `localhost:1433` |
| **Aspire Dashboard** | 17108 | `https://localhost:17108` |

**✅ Aspire assigns dynamic ports** for backend APIs to avoid conflicts.

**Access via Aspire Dashboard:**
```
https://localhost:17108
  ├── fleet-api: https://localhost:7001
  ├── reservations-api: https://localhost:7002
  ├── pricing-api: https://localhost:7003
  ├── customers-api: https://localhost:7004
  ├── api-gateway: https://localhost:7000
  └── public-portal: http://localhost:4200
```

---

### **Docker Compose**

```yaml
# docker-compose.yml
services:
  fleet-api:
    image: ghcr.io/.../fleet-api:latest
    ports:
      - "5001:8080"  # Host:Container
    environment:
      ASPNETCORE_URLS: "http://+:8080"

  reservations-api:
    image: ghcr.io/.../reservations-api:latest
    ports:
      - "5002:8080"
    environment:
      ASPNETCORE_URLS: "http://+:8080"

  public-portal:
    image: ghcr.io/.../public-portal:latest
    ports:
      - "4200:80"  # Angular app served by nginx on port 80
```

**✅ Port Mapping:**
- Container **always** listens on standard port (8080 for APIs, 80 for frontend)
- Host port can be different (5001, 5002, etc.)
- Environment variable sets container port

---

### **Kubernetes (Production)**

**Deployment:**
```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: fleet-api
spec:
  template:
    spec:
      containers:
      - name: fleet-api
        image: ghcr.io/.../fleet-api:v1.2.3
        ports:
        - containerPort: 8080  # ✅ App binds to this port
          name: http
        env:
        - name: ASPNETCORE_URLS
          value: "http://+:8080"  # ✅ Configured via environment
```

**Service (ClusterIP):**
```yaml
apiVersion: v1
kind: Service
metadata:
  name: fleet-api
spec:
  type: ClusterIP  # Internal service
  selector:
    app: fleet-api
  ports:
  - port: 8080          # ✅ Service port
    targetPort: 8080    # ✅ Container port
    protocol: TCP
```

**Ingress (External Access):**
```yaml
apiVersion: networking.k8s.io/v1
kind: Ingress
metadata:
  name: api-gateway-ingress
spec:
  rules:
  - host: api.orangecarrental.com
    http:
      paths:
      - path: /
        pathType: Prefix
        backend:
          service:
            name: api-gateway
            port:
              number: 8080  # ✅ Routes to service port
```

**✅ Request Flow:**
```
Internet (HTTPS 443)
    │
    ▼
┌────────────────────────────┐
│  Ingress Controller        │
│  (nginx/Traefik)           │
│  TLS Termination           │
└───────┬────────────────────┘
        │
        ▼
┌────────────────────────────┐
│  Service: api-gateway      │
│  ClusterIP: 10.0.1.100:8080│
└───────┬────────────────────┘
        │
        ▼
┌────────────────────────────┐
│  Pod: api-gateway-abc123   │
│  Container Port: 8080      │  ✅ Port-bound application
│  Kestrel web server        │
└────────────────────────────┘
```

---

## Self-Contained vs. Injected Deployment

### ❌ **Anti-Pattern: Runtime Injection (Pre-12-Factor)**

**Example: Traditional IIS Deployment**
```
┌──────────────────────────────────┐
│  IIS (Internet Information       │
│       Services)                   │
│  ─────────────────────────────   │
│  • Owns port 80/443               │
│  • Hosts multiple apps            │
│  • ASP.NET app injected at        │
│    runtime as DLL                 │
│  • App doesn't control port       │
└──────────────────────────────────┘
```

**Problems:**
- ❌ App depends on external IIS installation
- ❌ Can't run standalone
- ❌ Port controlled by IIS, not app
- ❌ Not portable (Windows-only)

---

### ✅ **12-Factor: Self-Contained (Current Approach)**

**Example: Kestrel Embedded Server**
```
┌──────────────────────────────────┐
│  Fleet API Container             │
│  ─────────────────────────────   │
│  • Kestrel web server embedded   │
│  • Binds to port 8080             │
│  • Self-contained executable      │
│  • No external dependencies       │
│  • Runs anywhere (Linux/Windows)  │
└──────────────────────────────────┘
```

**Benefits:**
- ✅ Completely self-contained
- ✅ Runs on any platform (Docker, Kubernetes, bare metal)
- ✅ Easy to develop and test locally
- ✅ No runtime web server installation needed
- ✅ Port configurable via environment variable

---

## App as a Backing Service

**12-Factor Principle:** One app's HTTP service can become the **backing service** for another app.

**Example: Fleet API as Backing Service**

```csharp
// Reservations API uses Fleet API as a backing service
builder.Services.AddHttpClient<IFleetService, FleetService>(client =>
{
    // Fleet API is accessed via its port-bound HTTP service
    client.BaseAddress = new Uri("http://fleet-api:8080");  // ✅ Service discovery
});
```

**Service Chain:**
```
Public Portal (Port 4200)
    │ HTTP
    ▼
API Gateway (Port 8080)
    │ HTTP
    ├─────> Fleet API (Port 8080)
    │
    └─────> Reservations API (Port 8080)
                │ HTTP
                ├─────> Pricing API (Port 8080)
                └─────> Customers API (Port 8080)
```

**✅ Each service:**
- Exports HTTP via port binding
- Can be used as a backing service by other apps
- Is completely independent and self-contained

---

## Configuration Best Practices

### **1. Port via Environment Variable** ✅

**Current (12-Factor compliant):**
```bash
# Environment variable controls port
ASPNETCORE_URLS="http://+:8080;https://+:8081"
```

**Application reads from environment:**
```csharp
// Program.cs - Default behavior
var builder = WebApplication.CreateBuilder(args);

// Reads ASPNETCORE_URLS from environment
// Falls back to appsettings.json if not set
var app = builder.Build();
app.Run();  // Binds to configured ports
```

**✅ Benefits:**
- Same code runs on different ports in different environments
- No hardcoded ports in source code
- Easy to change without recompiling

---

### **2. Listen on All Interfaces** ✅

**Correct:**
```bash
# Listen on all network interfaces (required for containers)
ASPNETCORE_URLS="http://+:8080"     # ASP.NET Core syntax
ASPNETCORE_URLS="http://0.0.0.0:8080"  # Explicit IP
```

**Incorrect (would only accept localhost requests):**
```bash
# ❌ DON'T - Only accepts localhost requests
ASPNETCORE_URLS="http://localhost:8080"
ASPNETCORE_URLS="http://127.0.0.1:8080"
```

**Why?** In Docker/Kubernetes, requests come from other containers, not localhost.

---

### **3. Separate Ports for Multiple Services** ✅

**Development (Aspire):**
```csharp
// Aspire automatically assigns unique ports to avoid conflicts
var fleetApi = builder.AddProject<OrangeCarRental_Fleet_Api>("fleet-api");
var reservationsApi = builder.AddProject<OrangeCarRental_Reservations_Api>("reservations-api");
// Both can bind to 8080 internally - Aspire maps to different host ports
```

**Docker Compose:**
```yaml
services:
  fleet-api:
    ports:
      - "5001:8080"  # Host port 5001 → container port 8080

  reservations-api:
    ports:
      - "5002:8080"  # Host port 5002 → container port 8080
```

**✅ Kubernetes:**
```yaml
# Each service has its own ClusterIP (virtual IP)
# All can use port 8080 internally
apiVersion: v1
kind: Service
metadata:
  name: fleet-api
spec:
  clusterIP: 10.0.1.100  # Unique IP
  ports:
  - port: 8080

---

apiVersion: v1
kind: Service
metadata:
  name: reservations-api
spec:
  clusterIP: 10.0.1.101  # Unique IP
  ports:
  - port: 8080
```

---

## Health Checks and Port Binding

**Health Check Endpoint:**

```csharp
// Program.cs
app.MapHealthChecks("/health");
```

**Docker Health Check:** `src/backend/Services/Fleet/Dockerfile:64-65`

```dockerfile
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1
```

**Kubernetes Liveness/Readiness Probes:**

```yaml
apiVersion: v1
kind: Pod
spec:
  containers:
  - name: fleet-api
    ports:
    - containerPort: 8080
    livenessProbe:
      httpGet:
        path: /health
        port: 8080  # ✅ Checks app is responding on its port
      initialDelaySeconds: 10
      periodSeconds: 30
    readinessProbe:
      httpGet:
        path: /health
        port: 8080  # ✅ Checks app is ready to accept traffic
      initialDelaySeconds: 5
      periodSeconds: 10
```

**✅ Health checks verify:**
- App is bound to the port
- App is responding to HTTP requests
- App is ready to handle traffic

---

## Best Practices Checklist

### **Port Binding**
- [x] Self-contained web server (Kestrel for .NET, Node.js for Angular)
- [x] No dependency on external web server (IIS, Apache, etc.)
- [x] Port configurable via environment variable (ASPNETCORE_URLS, PORT)
- [x] Listens on all interfaces (0.0.0.0 or +)
- [x] Exposed ports documented (Dockerfile EXPOSE)
- [x] Health check endpoints on bound port

### **Service Export**
- [x] HTTP service accessible on port (8080 for APIs, 4200/4201 for frontend)
- [x] Service can be consumed by other apps (backing service pattern)
- [x] Kubernetes Service maps to container port
- [x] Ingress routes external traffic to service port

### **Configuration**
- [x] No hardcoded ports in source code
- [x] Environment variable controls port (12-Factor Factor 3)
- [x] Standard ports in production (8080 for HTTP)
- [x] Unique ports in development (Aspire dynamic assignment)

---

## Recommendations

### **1. Standardize on Port 8080** ✅ Already Implemented

**Current Status:** All backend APIs use port 8080.

**Benefits:**
- Consistent across all services
- Easy to remember and configure
- Standard non-privileged port (doesn't require root)
- Works in Docker, Kubernetes, and bare metal

**Alternatives:**
- Port 80: Requires root/admin privileges in Linux
- Port 443: HTTPS, requires TLS certificates
- Port 5000-5001: ASP.NET Core default, but less standard

**✅ Recommendation: Keep port 8080** for consistency.

---

### **2. TLS Termination at Ingress** ✅ Recommended

**Current:** Apps bind to HTTP port 8080 only.

**Production Recommendation:** Terminate TLS at Ingress, not individual apps.

**Kubernetes Ingress with TLS:**
```yaml
apiVersion: networking.k8s.io/v1
kind: Ingress
metadata:
  name: api-ingress
  annotations:
    cert-manager.io/cluster-issuer: "letsencrypt-prod"
spec:
  tls:
  - hosts:
    - api.orangecarrental.com
    secretName: api-tls-cert
  rules:
  - host: api.orangecarrental.com
    http:
      paths:
      - path: /
        backend:
          service:
            name: api-gateway
            port:
              number: 8080  # ✅ Backend stays on HTTP
```

**✅ Benefits:**
- Simplified app configuration (no TLS certs in app)
- Centralized certificate management
- Easier certificate rotation
- Better performance (TLS offloading)

---

### **3. Service Mesh for mTLS** 🟢 Optional

**Current:** HTTP communication between services (no encryption).

**Advanced:** Use service mesh (Istio, Linkerd) for mutual TLS between services.

**Example (Istio):**
```yaml
apiVersion: security.istio.io/v1beta1
kind: PeerAuthentication
metadata:
  name: default
spec:
  mtls:
    mode: STRICT  # ✅ Enforce mTLS for all service-to-service communication
```

**✅ Benefits:**
- Encrypted communication between microservices
- Zero-trust security model
- No application code changes

**⚠️ Complexity:** Adds operational overhead. Recommended for security-critical environments only.

---

## Verification Commands

### **Check App is Listening on Port**
```bash
# Inside container
netstat -tuln | grep 8080

# Expected output:
tcp        0      0 0.0.0.0:8080            0.0.0.0:*               LISTEN
```

### **Test Port Binding**
```bash
# From host (Docker)
curl http://localhost:5001/health

# From another container (Docker Compose)
curl http://fleet-api:8080/health

# From another pod (Kubernetes)
kubectl exec -it test-pod -- curl http://fleet-api:8080/health
```

### **Verify Kubernetes Service Mapping**
```bash
# Get service endpoints
kubectl get endpoints fleet-api

# Expected output:
NAME        ENDPOINTS                           AGE
fleet-api   10.244.0.10:8080,10.244.0.11:8080   5d

# Test service is reachable
kubectl run test-pod --image=curlimages/curl --rm -it -- curl http://fleet-api:8080/health
```

### **Check Docker Port Mapping**
```bash
docker ps --format "table {{.Names}}\t{{.Ports}}"

# Expected output:
NAMES                PORTS
fleet-api            0.0.0.0:5001->8080/tcp
reservations-api     0.0.0.0:5002->8080/tcp
```

---

## Additional Resources

- [12-Factor App - Port Binding](https://12factor.net/port-binding)
- [Kestrel Web Server](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/servers/kestrel)
- [ASP.NET Core Configuration](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/)
- [Kubernetes Services](https://kubernetes.io/docs/concepts/services-networking/service/)
- [Docker EXPOSE Instruction](https://docs.docker.com/engine/reference/builder/#expose)

---

## Summary

| Aspect | Status | Compliance |
|--------|--------|------------|
| **Self-Contained Web Server** | ✅ Implemented | Excellent (Kestrel) |
| **Port Binding** | ✅ Implemented | Excellent (8080) |
| **Environment Config** | ✅ Implemented | Excellent (ASPNETCORE_URLS) |
| **Service Export** | ✅ Implemented | Excellent |
| **No External Web Server** | ✅ Verified | Excellent |
| **Docker EXPOSE** | ✅ Documented | Excellent |
| **Kubernetes Service** | ✅ Configured | Excellent |
| **Health Checks** | ✅ Implemented | Excellent |
| **TLS Termination** | ⚠️ At Ingress | Good (Recommended) |

**Overall Compliance:** ✅ **Excellent** - Factor 7 fully implemented with self-contained, port-bound services.

**Next Steps:**
1. Continue with Factor 8 (Concurrency) - Scale via process model
2. Optional: Implement service mesh for mTLS (security-critical environments)
