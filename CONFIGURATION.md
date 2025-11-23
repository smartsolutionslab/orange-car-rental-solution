# Configuration Management - 12-Factor Principles

## Overview

This document describes the configuration management approach for Orange Car Rental, following **12-Factor App - Factor 3: Store config in the environment**.

## 12-Factor Configuration Principles

✅ **Store config in environment variables**
✅ **Never commit secrets to version control**
✅ **Same codebase deploys to any environment**
✅ **Configuration is strictly separated from code**

---

## Current Issues & Solutions

### ❌ **Problem 1: Hardcoded Connection Strings**

**Current (appsettings.json):**
```json
{
  "ConnectionStrings": {
    "FleetDatabase": "Server=localhost;Database=OrangeCarRental_Fleet;Integrated Security=true;"
  }
}
```

**Issues:**
- `localhost` won't work in production
- `Integrated Security` is Windows-specific
- Can't deploy to different environments without code changes

**✅ Solution:**
- Remove connection strings from appsettings.json
- Aspire automatically injects via `ConnectionStrings__fleet` environment variable
- For non-Aspire deployments, use environment variables

---

### ❌ **Problem 2: Hardcoded Service URLs**

**Current (API Gateway appsettings.json):**
```json
{
  "ReverseProxy": {
    "Clusters": {
      "fleet-cluster": {
        "Destinations": {
          "destination1": {
            "Address": "http://localhost:5000"  // ❌ Hardcoded
          }
        }
      }
    }
  }
}
```

**Issues:**
- localhost URLs won't work in containerized/cloud environments
- Different ports per environment (dev/staging/prod)

**✅ Solution:**
- URLs are overridden by environment variables in AppHost
- For non-Aspire: Use `FLEET_API_URL`, `RESERVATIONS_API_URL` env vars
- appsettings.json should have placeholder or be removed

---

### ❌ **Problem 3: Keycloak Configuration in Development.json**

**Current (appsettings.Development.json):**
```json
{
  "Authentication": {
    "Keycloak": {
      "Authority": "http://localhost:8080/realms/orange-car-rental",
      "RequireHttpsMetadata": false
    }
  }
}
```

**Issues:**
- Development-specific file shouldn't have environment config
- Authority URL should come from environment
- `RequireHttpsMetadata` should be environment variable

**✅ Solution:**
- Move to environment variables
- Use `KEYCLOAK__AUTHORITY` environment variable
- Use `KEYCLOAK__REQUIRE_HTTPS` boolean environment variable

---

## Environment Variable Naming Convention

.NET automatically maps environment variables to configuration using `__` (double underscore) as section delimiter:

```
Authentication__Keycloak__Authority  →  Authentication:Keycloak:Authority
ConnectionStrings__fleet             →  ConnectionStrings:fleet
Logging__LogLevel__Default           →  Logging:LogLevel:Default
```

---

## Required Environment Variables by Service

### **Fleet API**
```bash
# Database (injected by Aspire)
ConnectionStrings__fleet="Server=sqlserver;Database=OrangeCarRental_Fleet;User Id=sa;Password=xxx;TrustServerCertificate=true"

# Keycloak Authentication
Authentication__Keycloak__Authority="https://keycloak.example.com/realms/orange-car-rental"
Authentication__Keycloak__Audience="orange-car-rental-api"
Authentication__Keycloak__RequireHttpsMetadata="true"

# Optional: Aspire Service Discovery (injected automatically)
```

### **Reservations API**
```bash
# Database
ConnectionStrings__reservations="Server=sqlserver;Database=OrangeCarRental_Reservations;User Id=sa;Password=xxx"

# Keycloak Authentication
Authentication__Keycloak__Authority="https://keycloak.example.com/realms/orange-car-rental"

# Service Discovery (Aspire auto-injects these)
# No manual URLs needed when using Aspire Service Discovery
```

### **Pricing API**
```bash
ConnectionStrings__pricing="Server=sqlserver;Database=OrangeCarRental_Pricing;User Id=sa;Password=xxx"
Authentication__Keycloak__Authority="https://keycloak.example.com/realms/orange-car-rental"
```

### **Customers API**
```bash
ConnectionStrings__customers="Server=sqlserver;Database=OrangeCarRental_Customers;User Id=sa;Password=xxx"
Authentication__Keycloak__Authority="https://keycloak.example.com/realms/orange-car-rental"
```

### **API Gateway**
```bash
# Backend Service URLs (overridden by Aspire)
FLEET_API_URL="http://fleet-api"
RESERVATIONS_API_URL="http://reservations-api"
PRICING_API_URL="http://pricing-api"
CUSTOMERS_API_URL="http://customers-api"

# Keycloak for JWT validation
Authentication__Keycloak__Authority="https://keycloak.example.com/realms/orange-car-rental"
```

---

## Environment-Specific Configuration

### **Development (Local with Aspire)**
Aspire automatically configures:
- ✅ Connection strings
- ✅ Service URLs via service discovery
- ✅ Keycloak localhost URL

You only need to set in `.env` or user secrets:
```bash
# Optional overrides
ASPIRE_DASHBOARD_OTLP_ENDPOINT_URL="http://localhost:18889"
```

### **Docker Compose (Local)**
```yaml
# docker-compose.override.yml
services:
  fleet-api:
    environment:
      ConnectionStrings__fleet: "Server=sqlserver;Database=OrangeCarRental_Fleet;User Id=sa;Password=${SQL_SA_PASSWORD}"
      Authentication__Keycloak__Authority: "http://keycloak:8080/realms/orange-car-rental"
      Authentication__Keycloak__RequireHttpsMetadata: "false"
```

### **Kubernetes (Production)**
```yaml
# ConfigMap for non-sensitive config
apiVersion: v1
kind: ConfigMap
metadata:
  name: orange-car-rental-config
data:
  KEYCLOAK_AUTHORITY: "https://auth.example.com/realms/orange-car-rental"
  KEYCLOAK_REQUIRE_HTTPS: "true"

---
# Secret for sensitive data
apiVersion: v1
kind: Secret
metadata:
  name: orange-car-rental-secrets
type: Opaque
stringData:
  CONNECTION_STRING_FLEET: "Server=production-sql.database.windows.net;Database=Fleet;User Id=app_user;Password=xxx"
```

---

## Secrets Management Strategy

### **❌ Never Do This:**
```json
// appsettings.json - DON'T COMMIT SECRETS!
{
  "ConnectionStrings": {
    "FleetDatabase": "Server=prod;User Id=sa;Password=SuperSecret123"  // ❌ NO!
  }
}
```

### **✅ Development Secrets:**

**Option 1: .NET User Secrets (Recommended for local dev)**
```bash
# Initialize
dotnet user-secrets init --project src/backend/Services/Fleet/OrangeCarRental.Fleet.Api

# Set secrets
dotnet user-secrets set "ConnectionStrings:fleet" "Server=localhost;Database=Fleet;User Id=sa;Password=YourStrong@Passw0rd"
```

**Option 2: .env File (for Aspire/Docker Compose)**
```bash
# .env (add to .gitignore!)
SQL_SA_PASSWORD=YourStrong@Passw0rd
KEYCLOAK_ADMIN_PASSWORD=admin
```

### **✅ Production Secrets:**

**Azure App Service:**
- Use **Application Settings** (stored as environment variables)
- Use **Azure Key Vault** for high-security secrets

**Kubernetes:**
- Use **Kubernetes Secrets** (base64 encoded)
- Use **External Secrets Operator** + Azure Key Vault/AWS Secrets Manager
- Use **Sealed Secrets** for GitOps

**Docker Swarm:**
- Use **Docker Secrets**

---

## Configuration Validation

Add startup validation to ensure required configuration is present:

```csharp
// Program.cs
var requiredSettings = new[]
{
    "ConnectionStrings:fleet",
    "Authentication:Keycloak:Authority"
};

foreach (var setting in requiredSettings)
{
    if (string.IsNullOrEmpty(builder.Configuration[setting]))
    {
        throw new InvalidOperationException(
            $"Required configuration '{setting}' is missing. " +
            $"Please set it via environment variable or user secrets.");
    }
}
```

---

## Migration Plan

### **Phase 1: Externalize Hardcoded Values** ✅
1. Remove hardcoded connection strings from appsettings.json
2. Remove hardcoded service URLs from appsettings.json
3. Move Keycloak config to environment variables

### **Phase 2: Add Validation**
1. Add configuration validation on startup
2. Add health checks that verify config is correct

### **Phase 3: Documentation**
1. Document all required environment variables (this file)
2. Create `.env.example` template
3. Update deployment documentation

### **Phase 4: Production Readiness**
1. Set up Azure Key Vault / AWS Secrets Manager
2. Configure Kubernetes secrets
3. Automate secret rotation

---

## Example: Complete Environment Setup

### **Development (.env for Aspire)**
```bash
# .env (add to .gitignore)

# SQL Server
SQL_SA_PASSWORD=YourStrong@Passw0rd

# Keycloak Admin
KEYCLOAK_ADMIN_PASSWORD=admin

# Optional: Override Aspire settings
ASPIRE_ALLOW_UNSECURED_TRANSPORT=true
```

### **Production (Kubernetes ConfigMap + Secret)**
```yaml
# configmap.yaml
apiVersion: v1
kind: ConfigMap
metadata:
  name: app-config
  namespace: orange-car-rental
data:
  # Keycloak
  Authentication__Keycloak__Authority: "https://auth.production.com/realms/orange-car-rental"
  Authentication__Keycloak__Audience: "orange-car-rental-api"
  Authentication__Keycloak__RequireHttpsMetadata: "true"
  Authentication__Keycloak__ValidateIssuer: "true"

  # Logging
  Logging__LogLevel__Default: "Information"
  Logging__LogLevel__Microsoft: "Warning"

  # CORS (adjust per environment)
  Cors__AllowedOrigins__0: "https://www.orange-rental.com"
  Cors__AllowedOrigins__1: "https://callcenter.orange-rental.com"

---
# secret.yaml (NEVER commit this - manage via CI/CD)
apiVersion: v1
kind: Secret
metadata:
  name: app-secrets
  namespace: orange-car-rental
type: Opaque
stringData:
  # Database connection strings
  ConnectionStrings__fleet: "Server=prod-sql.database.windows.net,1433;Database=Fleet;User Id=fleet_app;Password=xxx;Encrypt=true"
  ConnectionStrings__reservations: "Server=prod-sql.database.windows.net,1433;Database=Reservations;User Id=res_app;Password=xxx"
  ConnectionStrings__pricing: "Server=prod-sql.database.windows.net,1433;Database=Pricing;User Id=pricing_app;Password=xxx"
  ConnectionStrings__customers: "Server=prod-sql.database.windows.net,1433;Database=Customers;User Id=cust_app;Password=xxx"
```

---

## Checklist: Is Your Config 12-Factor Compliant?

- [ ] No hardcoded URLs in appsettings.json
- [ ] No connection strings with passwords in appsettings.json
- [ ] All environment-specific config comes from environment variables
- [ ] Secrets are never committed to git
- [ ] `.env` is in `.gitignore`
- [ ] `.env.example` exists (without secrets)
- [ ] Documentation lists all required environment variables
- [ ] Configuration validation on startup
- [ ] Same codebase can deploy to dev/staging/prod without changes

---

## Additional Resources

- [12-Factor App - Config](https://12factor.net/config)
- [ASP.NET Core Configuration](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/)
- [.NET User Secrets](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets)
- [Azure Key Vault](https://azure.microsoft.com/en-us/services/key-vault/)
- [Kubernetes Secrets](https://kubernetes.io/docs/concepts/configuration/secret/)

---

**Next Steps:** Implement Phase 1 by refactoring appsettings.json files across all services.
