# EF Core Migrations Guide

This guide explains how to manage database migrations for the Orange Car Rental application in both local development (Aspire) and Azure production environments.

## Overview

The application uses a flexible migration strategy that supports:
- **Aspire (Local Development)**: Automatic migrations on startup
- **Azure Deployment**: Separate migration jobs that run before services start
- **Manual Control**: Ability to disable auto-migrations for production scenarios

## Creating New Migrations

### Fleet Service

```powershell
# From the solution root
dotnet ef migrations add <MigrationName> `
  --project Services/Fleet/OrangeCarRental.Fleet.Infrastructure `
  --startup-project Services/Fleet/OrangeCarRental.Fleet.Api
```

### Reservations Service

```powershell
# From the solution root
dotnet ef migrations add <MigrationName> `
  --project Services/Reservations/OrangeCarRental.Reservations.Infrastructure `
  --startup-project Services/Reservations/OrangeCarRental.Reservations.Api
```

## Running Migrations

### Option 1: Aspire (Default Development Mode)

By default, migrations run automatically when services start:

```powershell
# Run Aspire normally - migrations apply on startup
dotnet run --project AppHost/OrangeCarRental.AppHost
```

The APIs detect they're in Development mode and automatically apply pending migrations before serving requests.

### Option 2: Aspire with Separate Migration Jobs

To simulate the Azure deployment pattern locally:

1. Add to `AppHost/appsettings.Development.json`:
```json
{
  "RunMigrationJobs": true
}
```

2. Run Aspire:
```powershell
dotnet run --project AppHost/OrangeCarRental.AppHost
```

This will:
- Start migration jobs (`fleet-migration` and `reservations-migration`)
- Wait for migrations to complete
- Start the API services only after migrations succeed

### Option 3: Manual Migrations (Production Pattern)

For production-like environments where you want manual control:

1. Set environment variable or configuration:
```json
{
  "Database": {
    "AutoMigrate": false
  }
}
```

2. Run migrations manually:
```powershell
# Fleet
dotnet run --project Services/Fleet/OrangeCarRental.Fleet.Api -- --migrate-only

# Reservations
dotnet run --project Services/Reservations/OrangeCarRental.Reservations.Api -- --migrate-only
```

3. Start services normally

## Azure Deployment Strategies

### Strategy 1: Azure Container Apps with Init Containers

Use init containers to run migrations before the main container starts:

```yaml
# Azure Container App configuration
containers:
  # Migration init container
  - name: fleet-migration
    image: your-registry.azurecr.io/fleet-api:latest
    args: ["--migrate-only"]
    env:
      - name: ConnectionStrings__fleet
        value: "your-azure-sql-connection-string"
      - name: ASPNETCORE_ENVIRONMENT
        value: "Production"

  # Main application container (starts after init container completes)
  - name: fleet-api
    image: your-registry.azurecr.io/fleet-api:latest
    env:
      - name: ConnectionStrings__fleet
        value: "your-azure-sql-connection-string"
      - name: Database__AutoMigrate
        value: "false"
```

### Strategy 2: Azure Container Jobs

Run migrations as separate Azure Container Jobs before deployment:

```bash
# Run migration job
az containerapp job create \
  --name fleet-migration-job \
  --resource-group your-rg \
  --image your-registry.azurecr.io/fleet-api:latest \
  --args "--migrate-only" \
  --env-vars \
    "ConnectionStrings__fleet=your-connection-string" \
    "ASPNETCORE_ENVIRONMENT=Production"

# Execute the job
az containerapp job start --name fleet-migration-job --resource-group your-rg

# After job completes, deploy the API
az containerapp update --name fleet-api --resource-group your-rg --image your-registry.azurecr.io/fleet-api:latest
```

### Strategy 3: Azure DevOps / GitHub Actions Pipeline

Run migrations as part of your CI/CD pipeline:

```yaml
# Example GitHub Actions workflow
- name: Run Database Migrations
  run: |
    # Fleet Service
    dotnet run --project src/backend/Services/Fleet/OrangeCarRental.Fleet.Api -- --migrate-only

    # Reservations Service
    dotnet run --project src/backend/Services/Reservations/OrangeCarRental.Reservations.Api -- --migrate-only
  env:
    ConnectionStrings__fleet: ${{ secrets.AZURE_SQL_FLEET_CONNECTION }}
    ConnectionStrings__reservations: ${{ secrets.AZURE_SQL_RESERVATIONS_CONNECTION }}
    ASPNETCORE_ENVIRONMENT: Production

- name: Deploy to Azure Container Apps
  # Deploy after migrations succeed
  run: |
    az containerapp update --name fleet-api ...
    az containerapp update --name reservations-api ...
```

### Strategy 4: Aspire Azure Deployment (azd)

When deploying with `azd` (Azure Developer CLI), the AppHost manifest supports container arguments:

```powershell
# The AppHost will generate proper Azure resources with migration jobs
azd up
```

The Aspire tooling will:
1. Convert migration jobs to Azure Container Jobs or init containers
2. Configure proper startup dependencies
3. Apply migrations before starting services

## Configuration Reference

### appsettings.json

```json
{
  "Database": {
    // Auto-migrate on startup
    // Default: true in Development, false in Production
    "AutoMigrate": true
  }
}
```

### Environment Variables

```bash
# Disable auto-migration
Database__AutoMigrate=false

# Connection strings (provided by Aspire or Azure)
ConnectionStrings__fleet=Server=...
ConnectionStrings__reservations=Server=...
```

### Command Line Arguments

```bash
# Run as migration job (exits after applying migrations)
--migrate-only
```

## Migration Best Practices

### Development
1. ✅ Use auto-migration (default) for rapid iteration
2. ✅ Commit migration files to source control
3. ✅ Review generated migration code before committing
4. ✅ Test migrations with realistic data

### Production
1. ✅ **Always** run migrations before deploying new app versions
2. ✅ Use separate migration jobs/containers
3. ✅ Set `Database:AutoMigrate` to `false` in production
4. ✅ Test migrations in staging environment first
5. ✅ Have a rollback plan
6. ✅ Monitor migration job logs
7. ⚠️ Never run auto-migrations in production APIs

### Azure Deployment Checklist
- [ ] Migrations run as separate job/init container
- [ ] Migration job uses same container image as API
- [ ] Migration job has correct connection string
- [ ] Migration job completes before API starts
- [ ] API has `Database:AutoMigrate` set to `false`
- [ ] Migration logs are captured and monitored
- [ ] Rollback plan is documented

## Common Migration Commands

```powershell
# List migrations
dotnet ef migrations list --project <InfraProject> --startup-project <ApiProject>

# Remove last migration (if not applied to database)
dotnet ef migrations remove --project <InfraProject> --startup-project <ApiProject>

# Generate SQL script for review
dotnet ef migrations script --project <InfraProject> --startup-project <ApiProject>

# Generate SQL script for specific migration range
dotnet ef migrations script FromMigration ToMigration --project <InfraProject> --startup-project <ApiProject>

# Rollback to specific migration
dotnet ef database update <PreviousMigrationName> --project <InfraProject> --startup-project <ApiProject>
```

## Troubleshooting

### Issue: Migrations not applying in Aspire

**Check:**
1. Is `ASPNETCORE_ENVIRONMENT` set to `Development`?
2. Are there any compilation errors?
3. Check Aspire dashboard logs for migration errors

### Issue: Migration job fails in Azure

**Check:**
1. Connection string is correct and accessible
2. Azure SQL firewall allows container IP
3. Migration job has sufficient timeout
4. Check job logs in Azure Portal

### Issue: "Migrations already applied" but database schema is wrong

**Solution:**
```powershell
# Check migration history
dotnet ef migrations list --project <InfraProject> --startup-project <ApiProject>

# If needed, remove wrong migration and recreate
dotnet ef database update <PreviousGoodMigration>
dotnet ef migrations remove
dotnet ef migrations add FixedMigration
```

## Security Considerations

1. **Connection Strings**: Never commit connection strings to source control
2. **Azure SQL**: Use Managed Identity when possible
3. **Least Privilege**: Migration jobs need `db_ddladmin` and `db_datareader` roles
4. **Secrets**: Store connection strings in Azure Key Vault
5. **Firewall**: Ensure Azure SQL allows container subnet access

## Example Azure SQL Connection String Formats

```bash
# SQL Authentication (not recommended for production)
Server=tcp:your-server.database.windows.net,1433;Database=OrangeCarRental_Fleet;User ID=admin;Password=<password>;Encrypt=True;TrustServerCertificate=False;

# Managed Identity (recommended)
Server=tcp:your-server.database.windows.net,1433;Database=OrangeCarRental_Fleet;Authentication=Active Directory Managed Identity;Encrypt=True;
```

## References

- [EF Core Migrations Documentation](https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/)
- [.NET Aspire Documentation](https://learn.microsoft.com/en-us/dotnet/aspire/)
- [Azure Container Apps](https://learn.microsoft.com/en-us/azure/container-apps/)
- [Azure Container Apps Jobs](https://learn.microsoft.com/en-us/azure/container-apps/jobs)
