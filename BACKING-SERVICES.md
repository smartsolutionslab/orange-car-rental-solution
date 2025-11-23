# Backing Services - 12-Factor Principles

## Overview

This document describes the backing services architecture for Orange Car Rental, following **12-Factor App - Factor 4: Treat backing services as attached resources**.

## 12-Factor Backing Services Principles

✅ **Treat backing services as attached resources** - No code distinction between local and third-party services
✅ **Resource binding via configuration** - Connect to services using URLs/credentials from config
✅ **Swappable without code changes** - Replace local database with managed service by changing config only
✅ **Loose coupling** - Services should be attachable and detachable at will

---

## What is a Backing Service?

A **backing service** is any service the application consumes over the network as part of its normal operation:

- **Databases** - SQL Server, PostgreSQL, MySQL, MongoDB
- **Message/Queue Systems** - RabbitMQ, Azure Service Bus, Amazon SQS
- **SMTP Services** - SendGrid, AWS SES, Azure Communication Services
- **Caching Systems** - Redis, Memcached
- **API Services** - Third-party APIs (payment gateways, geolocation, etc.)
- **Authentication** - Keycloak, Auth0, Azure AD

**Key Principle:** The code should make **no distinction** between local and third-party services. Both should be accessed via URL/credentials in configuration.

---

## Current Backing Services

### ✅ **1. SQL Server Databases**

**Purpose:** Persistent data storage for microservices
**Provider:** Microsoft SQL Server 2022
**Deployment:** Aspire-managed container (development), Azure SQL (production recommended)

**Databases:**
- `OrangeCarRental_Fleet` - Vehicle inventory and availability
- `OrangeCarRental_Reservations` - Customer bookings and rental history
- `OrangeCarRental_Pricing` - Pricing policies and rate calculations
- `OrangeCarRental_Customers` - Customer profiles and driver's license information
- `OrangeCarRental_Notifications` - Notification history and templates

**Configuration:** `src/backend/AppHost/OrangeCarRental.AppHost/Program.cs:18-31`

```csharp
// SQL Server container with persistent lifetime (data survives restarts)
var sqlServer = builder.AddSqlServer("sql")
    .WithLifetime(ContainerLifetime.Persistent);

// Individual databases (attached resources)
var fleetDb = sqlServer.AddDatabase("fleet", "OrangeCarRental_Fleet");
var reservationsDb = sqlServer.AddDatabase("reservations", "OrangeCarRental_Reservations");
var pricingDb = sqlServer.AddDatabase("pricing", "OrangeCarRental_Pricing");
var customersDb = sqlServer.AddDatabase("customers", "OrangeCarRental_Customers");
```

**Connection Strings** (injected via environment by Aspire):
```bash
# Development (Aspire auto-injects)
ConnectionStrings__fleet="Server=localhost,1433;Database=OrangeCarRental_Fleet;User Id=sa;Password=xxx;TrustServerCertificate=true"

# Production (Azure SQL example)
ConnectionStrings__fleet="Server=tcp:orangecar.database.windows.net,1433;Database=OrangeCarRental_Fleet;User ID=admin;Password=xxx;Encrypt=true"
```

**✅ Compliance:** Fully compliant - databases are attached via connection strings in configuration.

---

### ✅ **2. Keycloak (Authentication & Authorization)**

**Purpose:** Identity and Access Management (IAM)
**Provider:** Keycloak 26.0.7
**Deployment:** Aspire-managed container (development), Managed Keycloak or Azure AD (production)

**Configuration:** `src/backend/AppHost/OrangeCarRental.AppHost/Program.cs:5-14`

```csharp
var keycloak = builder.AddContainer("keycloak", "quay.io/keycloak/keycloak", "26.0.7")
    .WithHttpEndpoint(port: 8080, targetPort: 8080, name: "http")
    .WithEnvironment("KEYCLOAK_ADMIN", "admin")
    .WithEnvironment("KEYCLOAK_ADMIN_PASSWORD", "admin")
    .WithEnvironment("KC_HEALTH_ENABLED", "true")
    .WithEnvironment("KC_METRICS_ENABLED", "true")
    .WithArgs("start-dev")
    .WithLifetime(ContainerLifetime.Persistent);
```

**JWT Configuration** (per microservice, e.g., Fleet API):
```bash
# Development
Authentication__Keycloak__Authority="http://localhost:8080/realms/orange-car-rental"
Authentication__Keycloak__Audience="orange-car-rental-api"
Authentication__Keycloak__RequireHttpsMetadata="false"

# Production
Authentication__Keycloak__Authority="https://auth.orangecarrental.com/realms/orange-car-rental"
Authentication__Keycloak__Audience="orange-car-rental-api"
Authentication__Keycloak__RequireHttpsMetadata="true"
```

**✅ Compliance:** Fully compliant - Keycloak is attached via environment variables.

---

### ⚠️ **3. Email Service (Notifications API)**

**Purpose:** Send email notifications to customers
**Provider:** Currently **STUB** implementation (logs only)
**Recommended:** SendGrid, AWS SES, Azure Communication Services, Mailgun

**Interface:** `src/backend/Services/Notifications/OrangeCarRental.Notifications.Application/Services/IEmailService.cs`

```csharp
public interface IEmailService
{
    Task<string> SendEmailAsync(
        string toEmail,
        string subject,
        string body,
        CancellationToken cancellationToken = default);
}
```

**Current Implementation:** `src/backend/Services/Notifications/OrangeCarRental.Notifications.Infrastructure/Services/EmailService.cs`

```csharp
// Stub - logs emails instead of sending
logger.LogInformation(
    "STUB: Sending email to {ToEmail} with subject '{Subject}'",
    toEmail, subject);
```

**⚠️ Issue:** Not configurable for production email providers.

**📋 Recommended Implementation:**

**Option A: SendGrid (Recommended)**

1. Add NuGet package: `SendGrid` (v9.x)
2. Configure API key via environment:
   ```bash
   Email__Provider="SendGrid"
   Email__SendGrid__ApiKey="SG.xxx"
   Email__From="noreply@orangecarrental.com"
   Email__FromName="Orange Car Rental"
   ```
3. Update `EmailService` implementation:
   ```csharp
   public sealed class EmailService : IEmailService
   {
       private readonly SendGridClient _client;
       private readonly IConfiguration _config;

       public EmailService(IConfiguration config)
       {
           var apiKey = config["Email:SendGrid:ApiKey"];
           _client = new SendGridClient(apiKey);
           _config = config;
       }

       public async Task<string> SendEmailAsync(string toEmail, string subject, string body, CancellationToken ct)
       {
           var from = new EmailAddress(_config["Email:From"], _config["Email:FromName"]);
           var to = new EmailAddress(toEmail);
           var msg = MailHelper.CreateSingleEmail(from, to, subject, body, body);
           var response = await _client.SendEmailAsync(msg, ct);
           return response.Headers.GetValues("X-Message-Id").FirstOrDefault();
       }
   }
   ```

**Option B: AWS SES**

1. Add NuGet package: `AWSSDK.SimpleEmail`
2. Configure via environment:
   ```bash
   Email__Provider="AWSSES"
   Email__AWSSES__Region="us-east-1"
   Email__AWSSES__AccessKey="AKIA..."
   Email__AWSSES__SecretKey="xxx"
   ```

**Option C: Azure Communication Services**

1. Add NuGet package: `Azure.Communication.Email`
2. Configure connection string:
   ```bash
   Email__Provider="AzureCS"
   Email__AzureCS__ConnectionString="endpoint=https://xxx.communication.azure.com/;accesskey=xxx"
   ```

**✅ After Implementation:** Fully compliant - email provider swappable via configuration.

---

### ⚠️ **4. SMS Service (Notifications API)**

**Purpose:** Send SMS notifications to customers
**Provider:** Currently **STUB** implementation (logs only)
**Recommended:** Twilio, AWS SNS, Azure Communication Services

**Interface:** Similar to `IEmailService`

**⚠️ Issue:** Not configurable for production SMS providers.

**📋 Recommended Implementation (Twilio):**

```bash
Sms__Provider="Twilio"
Sms__Twilio__AccountSid="ACxxx"
Sms__Twilio__AuthToken="xxx"
Sms__Twilio__FromNumber="+15551234567"
```

---

## Swapping Backing Services

### **Example 1: Replace Aspire SQL Server with Azure SQL**

**Current (Development with Aspire):**
```csharp
// AppHost/Program.cs
var sqlServer = builder.AddSqlServer("sql");
var fleetDb = sqlServer.AddDatabase("fleet", "OrangeCarRental_Fleet");
```

**Production (Azure SQL) - Option A: Aspire Azure SQL Extension**
```csharp
// AppHost/Program.cs
var sqlServer = builder.AddAzureSqlServer("sql");  // Uses Azure SQL instead
var fleetDb = sqlServer.AddDatabase("fleet", "OrangeCarRental_Fleet");
```

**Production (Azure SQL) - Option B: Direct Connection String**
```bash
# Set environment variable (Kubernetes, Azure App Service, etc.)
ConnectionStrings__fleet="Server=tcp:orangecar.database.windows.net,1433;Database=OrangeCarRental_Fleet;Authentication=Active Directory Default;Encrypt=true"
```

**✅ Result:** Zero code changes in microservices - only configuration changes.

---

### **Example 2: Replace Keycloak with Azure AD**

**Current (Keycloak):**
```bash
Authentication__Keycloak__Authority="http://localhost:8080/realms/orange-car-rental"
Authentication__Keycloak__Audience="orange-car-rental-api"
```

**Swap to Azure AD (Microsoft Entra):**
```bash
Authentication__AzureAD__Authority="https://login.microsoftonline.com/{tenant-id}"
Authentication__AzureAD__Audience="api://orange-car-rental"
```

Update JWT configuration code to read from `AzureAD` section instead of `Keycloak`.

---

### **Example 3: Add Redis Caching Layer**

**Scenario:** Add Redis for caching vehicle availability queries.

**Step 1: Add to AppHost**
```csharp
// AppHost/Program.cs
var redis = builder.AddRedis("cache")
    .WithLifetime(ContainerLifetime.Persistent);

var fleetApi = builder
    .AddProject<OrangeCarRental_Fleet_Api>("fleet-api")
    .WithReference(fleetDb)
    .WithReference(redis)  // Attach Redis
    .WaitFor(redis);
```

**Step 2: Configure Fleet API**
```csharp
// Fleet.Api/Program.cs
builder.AddRedisClient("cache");  // Aspire injects connection string

// Use Redis via IConnectionMultiplexer
builder.Services.AddSingleton<ICacheService, RedisCacheService>();
```

**Environment Configuration (Production):**
```bash
# Azure Redis Cache
ConnectionStrings__cache="orangecar.redis.cache.windows.net:6380,password=xxx,ssl=True"

# AWS ElastiCache
ConnectionStrings__cache="orangecar.cache.amazonaws.com:6379,password=xxx,ssl=True"
```

**✅ Result:** Cache provider is swappable (local Redis → Azure Redis → AWS ElastiCache) via configuration.

---

## Health Checks for Backing Services

**Current Status:** ⚠️ Basic health endpoints exist (see `MONITORING.md`), but no backing service health checks.

**📋 Recommended Implementation:**

### **Add SQL Server Health Checks**

**Update `Fleet.Api/Program.cs`:**
```csharp
builder.Services.AddHealthChecks()
    .AddSqlServer(
        connectionString: builder.Configuration.GetConnectionString("fleet")!,
        name: "fleet-database",
        tags: new[] { "database", "sql" })
    .AddDbContextCheck<FleetDbContext>(
        name: "fleet-dbcontext",
        tags: new[] { "database", "ef-core" });
```

### **Add Keycloak Health Checks**

```csharp
builder.Services.AddHealthChecks()
    .AddUrlGroup(
        uri: new Uri($"{builder.Configuration["Authentication:Keycloak:Authority"]}/.well-known/openid-configuration"),
        name: "keycloak",
        tags: new[] { "auth", "external" });
```

### **Add Redis Health Checks (if implemented)**

```csharp
builder.Services.AddHealthChecks()
    .AddRedis(
        redisConnectionString: builder.Configuration.GetConnectionString("cache")!,
        name: "redis-cache",
        tags: new[] { "cache", "redis" });
```

**Health Check Endpoint:**
```bash
curl http://localhost:5000/health
# Response:
{
  "status": "Healthy",
  "checks": [
    { "name": "fleet-database", "status": "Healthy", "duration": "00:00:00.023" },
    { "name": "keycloak", "status": "Healthy", "duration": "00:00:00.105" }
  ]
}
```

---

## Resilience Patterns for Backing Services

### **Circuit Breaker for External API Calls**

**Use Polly for resilience** (already in `Directory.Packages.props`):

**Example: Email Service with Circuit Breaker**
```csharp
// Notifications.Api/Program.cs
builder.Services.AddHttpClient<IEmailService, SendGridEmailService>()
    .AddStandardResilienceHandler(options =>
    {
        options.CircuitBreaker.SamplingDuration = TimeSpan.FromSeconds(10);
        options.CircuitBreaker.BreakDuration = TimeSpan.FromSeconds(30);
        options.Retry.MaxRetryAttempts = 3;
    });
```

**Benefits:**
- Prevents cascading failures when email provider is down
- Automatic retry for transient errors
- Circuit opens after threshold failures, preventing resource exhaustion

---

### **Retry Policies for Database Connections**

**Entity Framework Core already includes built-in retry logic:**

```csharp
builder.AddSqlServerDbContext<FleetDbContext>("fleet", configureDbContextOptions: options =>
{
    options.UseSqlServer(sqlOptions =>
    {
        sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorNumbersToAdd: null);
    });
});
```

**Aspire automatically configures this** when using `.AddSqlServerDbContext()`.

---

## Deployment Scenarios

### **Development (Aspire)**

All backing services run as **containers** orchestrated by Aspire:

```bash
dotnet run --project src/backend/AppHost/OrangeCarRental.AppHost
```

Aspire automatically:
- Starts SQL Server container
- Starts Keycloak container
- Injects connection strings into microservices
- Provides observability dashboard at https://localhost:17108

---

### **Docker Compose (Local/CI)**

**`docker-compose.yml` example:**
```yaml
services:
  sqlserver:
    image: mcr.microsoft.com/mssql/server:2022-latest
    environment:
      ACCEPT_EULA: "Y"
      MSSQL_SA_PASSWORD: "YourStrong@Passw0rd"
    ports:
      - "1433:1433"
    volumes:
      - sqldata:/var/opt/mssql

  keycloak:
    image: quay.io/keycloak/keycloak:26.0.7
    environment:
      KEYCLOAK_ADMIN: admin
      KEYCLOAK_ADMIN_PASSWORD: admin
    ports:
      - "8080:8080"
    command: start-dev

  fleet-api:
    build: ./src/backend/Services/Fleet
    environment:
      ConnectionStrings__fleet: "Server=sqlserver,1433;Database=OrangeCarRental_Fleet;User Id=sa;Password=YourStrong@Passw0rd"
      Authentication__Keycloak__Authority: "http://keycloak:8080/realms/orange-car-rental"
    depends_on:
      - sqlserver
      - keycloak
```

**✅ Configuration via Environment:** All backing services configured externally.

---

### **Kubernetes (Production)**

**ConfigMap for non-sensitive configuration:**
```yaml
apiVersion: v1
kind: ConfigMap
metadata:
  name: fleet-api-config
data:
  Authentication__Keycloak__Authority: "https://auth.orangecarrental.com/realms/orange-car-rental"
  Email__Provider: "SendGrid"
  Email__From: "noreply@orangecarrental.com"
```

**Secret for sensitive credentials:**
```yaml
apiVersion: v1
kind: Secret
metadata:
  name: fleet-api-secrets
type: Opaque
stringData:
  ConnectionStrings__fleet: "Server=orangecar.database.windows.net;Database=Fleet;User ID=admin;Password=xxx"
  Email__SendGrid__ApiKey: "SG.xxx"
```

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
        image: orangecarrental/fleet-api:1.0.0
        envFrom:
        - configMapRef:
            name: fleet-api-config
        - secretRef:
            name: fleet-api-secrets
```

**✅ Swappable Resources:** Change ConfigMap/Secret to swap backing services (no code changes, no redeployment).

---

### **Azure App Service (Managed Platform)**

**Configure backing services via Azure Portal or CLI:**

```bash
# Azure SQL Database (managed service)
az webapp config connection-string set \
  --name fleet-api \
  --resource-group orange-car-rental \
  --connection-string-type SQLAzure \
  --settings fleet="Server=tcp:orangecar.database.windows.net,1433;Database=Fleet;Authentication=Active Directory Default"

# Azure AD Authentication
az webapp config appsettings set \
  --name fleet-api \
  --resource-group orange-car-rental \
  --settings \
    Authentication__AzureAD__Authority="https://login.microsoftonline.com/{tenant-id}" \
    Authentication__AzureAD__Audience="api://orange-car-rental"

# Azure Communication Services Email
az webapp config appsettings set \
  --name notifications-api \
  --resource-group orange-car-rental \
  --settings \
    Email__Provider="AzureCS" \
    Email__AzureCS__ConnectionString="@Microsoft.KeyVault(SecretUri=https://kv.vault.azure.net/secrets/acs-connection)"
```

**✅ Fully Managed:** Azure App Service manages scaling, availability, and security.

---

## Cloud-Agnostic Backing Services

### **Multi-Cloud Strategy**

**12-Factor compliance enables cloud portability:**

| Backing Service | Azure | AWS | GCP |
|----------------|-------|-----|-----|
| **SQL Database** | Azure SQL Database | Amazon RDS for SQL Server | Cloud SQL for SQL Server |
| **Authentication** | Azure AD (Entra) | Amazon Cognito | Google Identity Platform |
| **Email** | Azure Communication Services | Amazon SES | SendGrid (multi-cloud) |
| **SMS** | Azure Communication Services | Amazon SNS | Twilio (multi-cloud) |
| **Cache** | Azure Cache for Redis | Amazon ElastiCache | Google Memorystore |
| **Message Queue** | Azure Service Bus | Amazon SQS | Google Cloud Pub/Sub |

**Example: Swap Azure SQL → AWS RDS:**

```bash
# Before (Azure SQL)
ConnectionStrings__fleet="Server=tcp:orangecar.database.windows.net,1433;Database=Fleet;Authentication=Active Directory Default"

# After (AWS RDS)
ConnectionStrings__fleet="Server=orangecar.abc123.us-east-1.rds.amazonaws.com,1433;Database=Fleet;User ID=admin;Password=xxx;Encrypt=true"
```

**✅ Result:** Zero code changes - only connection string changes.

---

## Best Practices Checklist

### **Current Status**

- [x] SQL Server databases attached via connection strings (Aspire)
- [x] Keycloak configured via environment variables
- [x] Persistent container lifetime for data retention
- [x] Service interfaces (IEmailService, ISmsService) for swappability
- [x] No hardcoded database credentials
- [ ] **TODO:** Implement production email provider (SendGrid/AWS SES/Azure CS)
- [ ] **TODO:** Implement production SMS provider (Twilio/AWS SNS)
- [ ] **TODO:** Add health checks for all backing services
- [ ] **TODO:** Add circuit breakers for external service calls
- [ ] **TODO:** Document managed service configurations (Azure SQL, etc.)

---

## Migration Roadmap

### **Phase 1: Complete Email/SMS Integration** 🔴 High Priority

**Goal:** Replace stub implementations with production-ready providers.

**Tasks:**
1. Choose email provider (recommended: **SendGrid** for simplicity)
2. Add NuGet package: `SendGrid` (v9.x)
3. Update `EmailService` implementation to use SendGrid API
4. Add configuration section to `.env.example`:
   ```bash
   Email__Provider=SendGrid
   Email__SendGrid__ApiKey=SG.xxx
   Email__From=noreply@orangecarrental.com
   ```
5. Repeat for SMS provider (recommended: **Twilio**)
6. Add integration tests for email/SMS services

**Acceptance Criteria:**
- Email notifications sent via SendGrid (or chosen provider)
- SMS notifications sent via Twilio (or chosen provider)
- Configuration fully externalized (no hardcoded API keys)
- Providers swappable via config changes

---

### **Phase 2: Add Health Checks** 🟡 Medium Priority

**Goal:** Monitor backing service availability.

**Tasks:**
1. Add `AspNetCore.HealthChecks.SqlServer` NuGet package
2. Configure SQL Server health checks in each microservice
3. Add Keycloak health check (HTTP endpoint check)
4. Add Redis health check (if caching implemented)
5. Expose `/health` endpoint on all APIs
6. Configure Aspire to monitor health checks

**Acceptance Criteria:**
- All microservices expose `/health` endpoint
- Health checks include all backing services (SQL, Keycloak, etc.)
- Aspire dashboard shows health status for all services

---

### **Phase 3: Add Resilience Patterns** 🟡 Medium Priority

**Goal:** Handle backing service failures gracefully.

**Tasks:**
1. Add circuit breaker to email/SMS services (Polly)
2. Configure retry policies for database connections (EF Core)
3. Add fallback logging when email/SMS providers fail
4. Document failure handling in `MONITORING.md`

**Acceptance Criteria:**
- Circuit breaker prevents cascading failures
- Transient errors automatically retried
- Failed notifications logged for manual retry
- System remains operational when email/SMS providers down

---

### **Phase 4: Document Cloud Migrations** 🟢 Low Priority

**Goal:** Provide examples for migrating to managed cloud services.

**Tasks:**
1. Create migration guide for Azure SQL Database
2. Create migration guide for AWS RDS
3. Document Azure AD authentication setup
4. Document Azure Communication Services integration
5. Provide Terraform/Bicep templates for infrastructure

**Acceptance Criteria:**
- Step-by-step guides for cloud migrations
- Infrastructure-as-Code templates available
- Tested migration paths for Azure and AWS

---

## Verification Commands

### **Check SQL Server Connection**
```bash
# From any microservice
dotnet ef database update --project src/backend/Services/Fleet/OrangeCarRental.Fleet.Infrastructure
```

### **Check Keycloak Availability**
```bash
curl http://localhost:8080/realms/orange-car-rental/.well-known/openid-configuration
```

### **Verify Health Endpoints (after Phase 2)**
```bash
curl http://localhost:5000/health | jq
```

### **Test Email Service (after Phase 1)**
```bash
# Send test email via API
curl -X POST http://localhost:5005/api/notifications/email \
  -H "Content-Type: application/json" \
  -d '{
    "to": "test@example.com",
    "subject": "Test Email",
    "body": "This is a test email from Orange Car Rental"
  }'
```

---

## Additional Resources

- [12-Factor App - Backing Services](https://12factor.net/backing-services)
- [Aspire Service Defaults](https://learn.microsoft.com/en-us/dotnet/aspire/fundamentals/service-defaults)
- [.NET Health Checks](https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/health-checks)
- [Polly Resilience Library](https://www.pollydocs.org/)
- [SendGrid .NET Library](https://github.com/sendgrid/sendgrid-csharp)
- [Twilio .NET SDK](https://www.twilio.com/docs/libraries/csharp-dotnet)

---

## Summary

| Backing Service | Status | Swappable? | Health Check | Resilience |
|-----------------|--------|-----------|--------------|------------|
| SQL Server | ✅ Configured | ✅ Yes | ❌ Missing | ✅ EF Core Retry |
| Keycloak | ✅ Configured | ✅ Yes | ❌ Missing | ⚠️ Basic |
| Email | ⚠️ Stub Only | ✅ Via Interface | N/A | ❌ None |
| SMS | ⚠️ Stub Only | ✅ Via Interface | N/A | ❌ None |

**Next Steps:** Implement **Phase 1** (Email/SMS providers) to achieve full 12-Factor Factor 4 compliance.
