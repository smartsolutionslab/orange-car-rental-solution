# Orange Car Rental - Troubleshooting Guide

Quick solutions to common issues when developing with the Orange Car Rental system.

**Last Updated:** 2025-11-22

---

## Table of Contents

1. [Port Conflicts](#port-conflicts)
2. [Aspire Issues](#aspire-issues)
3. [Database Problems](#database-problems)
4. [Frontend Issues](#frontend-issues)
5. [API Errors](#api-errors)
6. [Docker Issues](#docker-issues)
7. [Authentication Problems](#authentication-problems)
8. [Build Failures](#build-failures)

---

## Port Conflicts

### Problem: Port 4201 Already in Use

**Symptom:**
```
fail: Could not start the proxy for the service: listen tcp [::1]:4201: bind:
Only one usage of each socket address is normally permitted.
```

**Cause:** Another process (usually a previous frontend instance or another Aspire service) is using port 4201.

**Solutions:**

**Option 1: Kill the existing process**
```bash
# Find process using port 4201
netstat -ano | findstr :4201

# Kill the process (replace PID with actual process ID)
taskkill /PID <pid> /F
```

**Option 2: Change the port in AppHost configuration**

Edit `src/backend/AppHost/OrangeCarRental.AppHost/Program.cs`:
```csharp
var callCenterPortal = builder.AddNpmApp("call-center-portal", ...)
    .WithHttpEndpoint(port: 4202)  // Change from 4201 to 4202
    .WithExternalHttpEndpoints();
```

**Option 3: Accept the conflict (if Public Portal is running)**

If the Public Portal is already running on 4201, you can ignore this error. The Call Center Portal will retry but the system remains functional with just the Public Portal.

---

### Problem: Port 5002 (API Gateway) Already in Use

**Symptom:**
```
Error: Address already in use: localhost:5002
```

**Solution:**
```bash
# Find and kill the process
netstat -ano | findstr :5002
taskkill /PID <pid> /F

# Restart Aspire
cd src/backend/AppHost/OrangeCarRental.AppHost
dotnet run
```

---

### Problem: SQL Server Port 1433 Conflict

**Symptom:**
```
Docker: Bind for 0.0.0.0:1433 failed: port is already allocated
```

**Cause:** SQL Server is already running locally or another container is using port 1433.

**Solutions:**

**Option 1: Stop local SQL Server**
```bash
# Windows Services
services.msc
# Find "SQL Server (MSSQLSERVER)" and stop it
```

**Option 2: Stop conflicting container**
```bash
docker ps | findstr 1433
docker stop <container_id>
```

**Option 3: Change Docker SQL Server port**

Edit `docker-compose.sqlserver.yml`:
```yaml
ports:
  - "1434:1433"  # Map to different host port
```

---

## Aspire Issues

### Problem: "no endpoints configured" Error

**Symptom:**
```
fail: Error handling TCP connection {"Service": {"name":"customers-api-http"},
"error": "no endpoints configured"}
```

**Cause:** The service hasn't fully started yet, or there's a configuration issue.

**Solutions:**

**Option 1: Wait for services to fully start**

Aspire services can take 30-60 seconds to fully initialize. Check the Aspire Dashboard:
```
https://localhost:17161
```

Navigate to **Resources** tab and verify all services show "Running" status.

**Option 2: Restart the affected service**

In Aspire Dashboard:
1. Go to **Resources** tab
2. Find the affected service (e.g., "customers-api")
3. Click the **Restart** button

**Option 3: Check service logs**

In Aspire Dashboard → **Logs** tab:
- Filter by the affected service
- Look for startup errors or exceptions
- Check for missing dependencies or configuration

---

### Problem: Aspire Dashboard Not Accessible

**Symptom:**
```
Cannot access https://localhost:17161
```

**Solutions:**

**Option 1: Check if Aspire is running**
```bash
# Windows
Get-Process | Where-Object {$_.ProcessName -like "*OrangeCarRental*"}

# If not running, start it
cd src/backend/AppHost/OrangeCarRental.AppHost
dotnet run
```

**Option 2: Check the console output for actual URL**

The Aspire dashboard URL might be different. Look for:
```
info: Now listening on: https://localhost:17161
info: Login to the dashboard at https://localhost:17161/login?t={token}
```

**Option 3: Certificate issues**

If you see SSL/certificate warnings, accept the self-signed certificate:
```bash
dotnet dev-certs https --trust
```

---

### Problem: Aspire Build Lock Error

**Symptom:**
```
error MSB3027: Could not copy "apphost.exe".
The file is locked by: "OrangeCarRental.AppHost (PID)"
```

**Solution:**
```bash
# Find the locking process from error message
taskkill /PID <pid> /F

# Clean and rebuild
cd src/backend
dotnet clean
dotnet build

# Restart Aspire
cd AppHost/OrangeCarRental.AppHost
dotnet run
```

---

## Database Problems

### Problem: Cannot Connect to SQL Server

**Symptom:**
```
SqlException: A network-related or instance-specific error occurred
```

**Solutions:**

**Option 1: Verify SQL Server container is running**
```bash
docker ps | findstr sqlserver

# If not running:
docker-compose -f docker-compose.sqlserver.yml up -d

# Wait 10 seconds for SQL Server to fully start
timeout /t 10
```

**Option 2: Test connection manually**
```bash
docker exec -it orange-rental-sqlserver /opt/mssql-tools18/bin/sqlcmd \
  -S localhost -U sa -P "YourStrong@Passw0rd" -C -Q "SELECT @@VERSION"
```

**Option 3: Check connection string**

Aspire auto-generates connection strings. Check the Aspire Dashboard:
1. Go to **Resources** → Select SQL Server
2. View **Environment Variables**
3. Look for `ConnectionStrings__*` variables

---

### Problem: Database Migrations Not Applied

**Symptom:**
```
SqlException: Invalid object name 'customers.Customers'
```

**Cause:** Entity Framework migrations haven't been applied to the database.

**Solution:**
```bash
# Apply migrations for each service
cd src/backend

# Customers
dotnet ef database update --project Services/Customers/OrangeCarRental.Customers.Api

# Fleet
dotnet ef database update --project Services/Fleet/OrangeCarRental.Fleet.Api

# Reservations
dotnet ef database update --project Services/Reservations/OrangeCarRental.Reservations.Api

# Pricing
dotnet ef database update --project Services/Pricing/OrangeCarRental.Pricing.Api
```

---

### Problem: Seed Data Not Loading

**Symptom:** APIs return empty results even after running seed script.

**Solutions:**

**Option 1: Verify seed data was applied**
```bash
# Check record counts
docker exec -it orange-rental-sqlserver /opt/mssql-tools18/bin/sqlcmd \
  -S localhost -U sa -P "YourStrong@Passw0rd" -C \
  -Q "USE OrangeCarRental_Fleet; SELECT COUNT(*) FROM fleet.Vehicles;"
```

**Option 2: Re-run seed script**
```powershell
Get-Content scripts/db/seed-test-data-sqlserver.sql | `
  docker exec -i orange-rental-sqlserver /opt/mssql-tools18/bin/sqlcmd `
  -S localhost -U sa -P "YourStrong@Passw0rd" -C
```

**Option 3: Check for SQL errors**

Look for error messages in the seed script output. Common issues:
- Column name mismatches
- Foreign key violations
- Duplicate primary keys

---

### Problem: Database Performance Issues

**Symptom:** Slow queries, timeouts.

**Solutions:**

**Option 1: Add indexes**

Create a migration to add missing indexes:
```bash
cd src/backend/Services/Fleet/OrangeCarRental.Fleet.Api
dotnet ef migrations add AddPerformanceIndexes
```

**Option 2: Check connection pool settings**

In `appsettings.json`, verify connection string has appropriate pooling:
```json
"ConnectionStrings": {
  "FleetDb": "Server=localhost;Database=OrangeCarRental_Fleet;...;Max Pool Size=100;"
}
```

**Option 3: Enable SQL Server query logging**

Add to service `appsettings.Development.json`:
```json
"Logging": {
  "Microsoft.EntityFrameworkCore.Database.Command": "Information"
}
```

---

## Frontend Issues

### Problem: Frontend Not Loading (Blank Page)

**Symptom:** Browser shows blank page or "Cannot GET /"

**Solutions:**

**Option 1: Check if frontend is running**
```bash
netstat -ano | findstr :4201

# If not running:
cd src/frontend/apps/public-portal
npm install
npm start
```

**Option 2: Clear browser cache**

Hard reload: `Ctrl + Shift + R` (Windows) or `Cmd + Shift + R` (Mac)

**Option 3: Check browser console for errors**

Open DevTools (F12) → Console tab → Look for:
- JavaScript errors
- Failed API calls
- CORS errors

---

### Problem: CORS Errors in Browser

**Symptom:**
```
Access to fetch at 'http://localhost:5002/api/vehicles' from origin
'http://localhost:4201' has been blocked by CORS policy
```

**Cause:** API Gateway CORS configuration doesn't include frontend origin.

**Solution:**

Edit `src/backend/ApiGateway/Program.cs`:
```csharp
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(
            "http://localhost:4200",
            "http://localhost:4201"  // Ensure this is included
        )
        .AllowAnyMethod()
        .AllowAnyHeader();
    });
});
```

Restart the API Gateway or Aspire.

---

### Problem: npm install Failures

**Symptom:**
```
npm ERR! code ECONNRESET
npm ERR! network Socket timeout
```

**Solutions:**

**Option 1: Clear npm cache**
```bash
npm cache clean --force
rm -rf node_modules package-lock.json
npm install
```

**Option 2: Use different registry**
```bash
npm config set registry https://registry.npmjs.org/
npm install
```

**Option 3: Increase timeout**
```bash
npm install --timeout=60000
```

---

## API Errors

### Problem: 401 Unauthorized

**Symptom:** API calls return 401 status code.

**Cause:** Missing or invalid JWT token.

**Solutions:**

**Option 1: Get a valid token from Keycloak**
```bash
# Login to Keycloak admin console
# http://localhost:8080
# Username: admin
# Password: admin

# Create a test user in the "orange-car-rental" realm
# Generate a token using the user credentials
```

**Option 2: Disable authentication temporarily (development only)**

In the API service `Program.cs`, comment out:
```csharp
// app.UseAuthentication();
// app.UseAuthorization();
```

**Warning:** Only for local development testing!

---

### Problem: 500 Internal Server Error

**Symptom:** API returns 500 error with no details.

**Solutions:**

**Option 1: Check Aspire Dashboard logs**
1. Open https://localhost:17161
2. Go to **Logs** tab
3. Filter by the failing service
4. Look for exceptions around the time of the error

**Option 2: Enable detailed error messages**

Add to `appsettings.Development.json`:
```json
{
  "DetailedErrors": true,
  "Logging": {
    "LogLevel": {
      "Default": "Debug"
    }
  }
}
```

**Option 3: Use API health checks**
```bash
# Check each service health
curl http://localhost:5002/health
curl http://localhost:5002/api/vehicles/health
curl http://localhost:5002/api/customers/health
```

---

### Problem: API Returns Empty Results

**Symptom:** GET requests return `[]` or `null` when data should exist.

**Solutions:**

**Option 1: Verify database has data**
```bash
# Check Fleet database
docker exec orange-rental-sqlserver /opt/mssql-tools18/bin/sqlcmd \
  -S localhost -U sa -P "YourStrong@Passw0rd" -C \
  -Q "USE OrangeCarRental_Fleet; SELECT COUNT(*) FROM fleet.Vehicles;"
```

**Option 2: Check query filters**

Review the API endpoint code for filters that might exclude all results:
```csharp
// Example: Check for date filters, status filters, etc.
var vehicles = await _context.Vehicles
    .Where(v => v.Status == VehicleStatus.Available)  // Might be too restrictive
    .ToListAsync();
```

**Option 3: Test with raw SQL**
```bash
docker exec -it orange-rental-sqlserver /opt/mssql-tools18/bin/sqlcmd \
  -S localhost -U sa -P "YourStrong@Passw0rd" -C

# Then run:
USE OrangeCarRental_Fleet;
SELECT TOP 10 * FROM fleet.Vehicles;
GO
```

---

## Docker Issues

### Problem: Docker Desktop Not Running

**Symptom:**
```
Cannot connect to the Docker daemon
```

**Solution:**
1. Start Docker Desktop from Windows Start Menu
2. Wait for Docker to fully start (whale icon in system tray)
3. Verify: `docker version`
4. Restart Aspire

---

### Problem: Container Won't Start

**Symptom:**
```
docker: Error response from daemon: driver failed programming external
connectivity on endpoint
```

**Solutions:**

**Option 1: Restart Docker Desktop**
1. Right-click Docker Desktop system tray icon
2. Select "Quit Docker Desktop"
3. Wait 10 seconds
4. Start Docker Desktop again

**Option 2: Remove conflicting containers**
```bash
# List all containers
docker ps -a

# Remove specific container
docker rm -f <container_name_or_id>

# Or remove all stopped containers
docker container prune -f
```

**Option 3: Check for port conflicts**
```bash
# See which container is using a port
docker ps --format "table {{.Names}}\t{{.Ports}}"
```

---

### Problem: Out of Disk Space

**Symptom:**
```
docker: no space left on device
```

**Solution:**
```bash
# Clean up unused images and containers
docker system prune -a --volumes

# Check disk usage
docker system df

# Clean build cache
docker builder prune -a
```

---

## Authentication Problems

### Problem: Keycloak Not Accessible

**Symptom:** Cannot access http://localhost:8080

**Solutions:**

**Option 1: Check if Keycloak container is running**
```bash
docker ps | findstr keycloak

# If not running:
docker-compose -f docker-compose.keycloak.yml up -d

# Wait for Keycloak to fully start (can take 30-60 seconds)
timeout /t 30
```

**Option 2: Check Keycloak logs**
```bash
docker logs orange-rental-keycloak
```

**Option 3: Verify Keycloak in Aspire Dashboard**

Check https://localhost:17161 → Resources → Keycloak status

---

### Problem: JWT Token Validation Fails

**Symptom:**
```
Bearer error="invalid_token", error_description="The signature is invalid"
```

**Cause:** Mismatch between Keycloak public key and API configuration.

**Solution:**

Verify JWT settings in `appsettings.json`:
```json
{
  "Authentication": {
    "Schemes": {
      "Bearer": {
        "Authority": "http://localhost:8080/realms/orange-car-rental",
        "RequireHttpsMetadata": false,  // For development only
        "ValidateIssuer": true,
        "ValidateAudience": false,  // Adjust based on your setup
        "ValidateLifetime": true
      }
    }
  }
}
```

Restart the API service.

---

## Build Failures

### Problem: NuGet Restore Fails

**Symptom:**
```
error NU1301: Unable to load the service index for source
```

**Solutions:**

**Option 1: Clear NuGet cache**
```bash
dotnet nuget locals all --clear
dotnet restore
```

**Option 2: Check internet connection**
```bash
# Test connectivity
curl https://api.nuget.org/v3/index.json
```

**Option 3: Use offline packages**
```bash
dotnet restore --no-cache --force
```

---

### Problem: TypeScript Compilation Errors

**Symptom:**
```
error TS2304: Cannot find name 'Observable'
```

**Solutions:**

**Option 1: Update dependencies**
```bash
cd src/frontend/apps/public-portal
npm update
```

**Option 2: Reinstall node_modules**
```bash
rm -rf node_modules package-lock.json
npm install
```

**Option 3: Check TypeScript version**
```bash
npx tsc --version
# Should be 5.x or higher for Angular 18+
```

---

## Emergency Recovery

### Nuclear Option: Complete System Reset

If all else fails, perform a complete reset:

```bash
# 1. Stop all processes
taskkill /F /IM dotnet.exe
taskkill /F /IM node.exe

# 2. Stop and remove all Docker containers
docker-compose -f docker-compose.sqlserver.yml down -v
docker-compose -f docker-compose.keycloak.yml down -v
docker system prune -a --volumes -f

# 3. Clean all build artifacts
cd src/backend
dotnet clean
rm -rf */bin */obj **/bin **/obj

cd ../frontend
rm -rf node_modules
rm -rf apps/*/node_modules

# 4. Reinstall dependencies
cd src/backend
dotnet restore

cd ../frontend
npm install

# 5. Start fresh
cd src/backend/AppHost/OrangeCarRental.AppHost
dotnet run
```

Wait for all services to start (2-3 minutes), then verify:
- Aspire Dashboard: https://localhost:17161
- API Gateway: http://localhost:5002/health
- Frontend: http://localhost:4201

---

## Getting Help

### Documentation Resources

- **Quick Start:** [ASPIRE-QUICKSTART.md](./ASPIRE-QUICKSTART.md)
- **Service Management:** [SERVICE-MANAGEMENT.md](./SERVICE-MANAGEMENT.md)
- **System Status:** [DEPLOYMENT-STATUS.md](./DEPLOYMENT-STATUS.md)
- **Next Steps:** [NEXT-STEPS.md](./NEXT-STEPS.md)

### Diagnostic Commands

**System Health Check:**
```bash
# Check all ports
netstat -ano | findstr "LISTENING" | findstr ":4201 :5002 :1433 :8080 :17161"

# Check Docker
docker ps

# Check Aspire Dashboard
curl http://localhost:5002/health

# Check databases
docker exec orange-rental-sqlserver /opt/mssql-tools18/bin/sqlcmd \
  -S localhost -U sa -P "YourStrong@Passw0rd" -C \
  -Q "SELECT name FROM sys.databases WHERE name LIKE 'OrangeCarRental%'"
```

**Collect Diagnostic Information:**
```bash
# Create diagnostics report
echo "=== System Info ===" > diagnostics.txt
date >> diagnostics.txt
echo "" >> diagnostics.txt

echo "=== .NET Info ===" >> diagnostics.txt
dotnet --info >> diagnostics.txt
echo "" >> diagnostics.txt

echo "=== Node/npm Info ===" >> diagnostics.txt
node --version >> diagnostics.txt
npm --version >> diagnostics.txt
echo "" >> diagnostics.txt

echo "=== Docker Info ===" >> diagnostics.txt
docker --version >> diagnostics.txt
docker ps >> diagnostics.txt
echo "" >> diagnostics.txt

echo "=== Port Usage ===" >> diagnostics.txt
netstat -ano | findstr "LISTENING" >> diagnostics.txt

# Share diagnostics.txt when asking for help
```

---

## Common Error Messages Reference

| Error | Likely Cause | Quick Fix |
|-------|--------------|-----------|
| `EADDRINUSE` | Port already in use | Kill process using the port |
| `SqlException` | Database connection issue | Verify SQL Server container running |
| `401 Unauthorized` | Missing/invalid JWT token | Check Keycloak and authentication config |
| `CORS policy` | Frontend origin not allowed | Update CORS configuration in API |
| `no endpoints configured` | Service not fully started | Wait or restart service in Aspire Dashboard |
| `npm ERR! code ECONNRESET` | Network/registry issue | Clear npm cache and retry |
| `Could not copy apphost.exe` | Process lock on executable | Kill locking process and rebuild |
| `Invalid object name` | Missing migrations | Run `dotnet ef database update` |

---

**Last Updated:** 2025-11-22
**Aspire Version:** 9.5.2
**.NET Version:** 9.0
**Angular Version:** 18+
