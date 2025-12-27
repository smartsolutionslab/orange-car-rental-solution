# Quick Reference

## Local Development with .NET Aspire

```bash
# Start everything (SQL Server, Keycloak, all APIs, frontend apps)
cd src/backend/AppHost/OrangeCarRental.AppHost
dotnet run

# Access points:
# - Aspire Dashboard: https://localhost:17225
# - Shell (Main App): http://localhost:4300
# - Public Portal: http://localhost:4301
# - Call Center Portal: http://localhost:4302
# - API Gateway: http://localhost:5002
# - Keycloak: http://localhost:8080
```

## Database Migrations

Migrations are handled by EF Core and run automatically via the `db-migrator` project in Aspire.

```bash
# Add a new migration (example for Fleet service)
cd src/backend/Services/Fleet/OrangeCarRental.Fleet.Infrastructure
dotnet ef migrations add MigrationName --startup-project ../OrangeCarRental.Fleet.Api

# List migrations
dotnet ef migrations list --startup-project ../OrangeCarRental.Fleet.Api
```

## Running Tests

```bash
# Unit tests
cd src/backend
dotnet test --filter "FullyQualifiedName!~IntegrationTests"

# Integration tests (uses Aspire.Testing)
dotnet test --filter "FullyQualifiedName~IntegrationTests"

# All tests
dotnet test
```

## Azure Deployment

### Initial Setup

```bash
# Deploy Azure infrastructure
cd infrastructure/azure
az deployment sub create \
  --name orange-prod-deployment \
  --location westeurope \
  --template-file main.bicep \
  --parameters @parameters.production.json

# Get AKS credentials
az aks get-credentials \
  --resource-group orange-production-rg \
  --name orange-production-aks
```

### Common Kubernetes Operations

```bash
# View pods
kubectl get pods -n orange-production

# View logs
kubectl logs -f deployment/api-gateway -n orange-production

# Scale service
kubectl scale deployment api-gateway --replicas=3 -n orange-production

# Restart deployment
kubectl rollout restart deployment/api-gateway -n orange-production

# Rollback
kubectl rollout undo deployment/api-gateway -n orange-production
```

## Resource Names

| Environment | Resource Group | AKS Cluster | Namespace |
|-------------|----------------|-------------|-----------|
| Production | orange-production-rg | orange-production-aks | orange-production |
| Staging | orange-staging-rg | orange-staging-aks | orange-staging |
| Development | orange-dev-rg | orange-dev-aks | orange-dev |

## URLs

### Production
- Public Portal: https://orange-rental.de
- API Gateway: https://api.orange-rental.de
- Keycloak: https://auth.orange-rental.de

### Staging
- Public Portal: https://staging.orange-rental.de
- API Gateway: https://api-staging.orange-rental.de
