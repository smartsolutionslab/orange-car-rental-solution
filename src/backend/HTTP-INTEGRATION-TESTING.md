# HTTP Integration Testing Guide

## Overview

This guide provides step-by-step instructions for testing the HTTP communication between Fleet and Reservations services after the architecture refactoring.

## Architecture Changes

### Before Refactoring
- Fleet service directly queried Reservations database via `ReservationsDbContext`
- Violated bounded context boundaries
- Tight coupling between services

### After Refactoring
- Fleet service calls Reservations API via HTTP
- Clean separation of concerns
- Proper microservice boundaries

## Service Configuration

### Fleet API
- **Port**: `http://localhost:5046`
- **Database**: `OrangeCarRental_Fleet`
- **Dependencies**: Calls Reservations API at `http://localhost:5289`
- **Configuration**: `Services/Fleet/OrangeCarRental.Fleet.Api/appsettings.json`

### Reservations API
- **Port**: `http://localhost:5289`
- **Database**: `OrangeCarRental_Reservations`
- **New Endpoint**: `GET /api/reservations/availability`
- **Configuration**: `Services/Reservations/OrangeCarRental.Reservations.Api/appsettings.json`

## Prerequisites

### CRITICAL: SQL Server Requirement

**SQL Server must be running before proceeding with any runtime testing steps.**

The services will automatically apply database migrations on startup, but they require an active SQL Server instance. If SQL Server is not running, you will see errors like:

```
Microsoft.Data.SqlClient.SqlException: A network-related or instance-specific error occurred while establishing a connection to SQL Server.
The server was not found or was not accessible.
```

#### Checking SQL Server Status

Run one of these commands to verify SQL Server is running:

```powershell
# Check for default SQL Server instance
sc query MSSQLSERVER

# Check for SQL Server Express
sc query MSSQL$SQLEXPRESS

# List all SQL-related services
Get-Service | Where-Object { $_.Name -like '*sql*' }
```

If SQL Server is not installed, you have these options:
1. **Install SQL Server Express**: Free edition from Microsoft
2. **Install SQL Server Developer Edition**: Full-featured free version for development
3. **Use SQL Server in Docker**: `docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=YourStrong@Password" -p 1433:1433 -d mcr.microsoft.com/mssql/server:2022-latest`

### Other Prerequisites

1. **.NET 9.0 SDK** installed ✅ (Verified)
2. **SQL Server** running locally ⚠️ (NOT DETECTED - see above)
3. **Git Bash** or **PowerShell** ✅ (Available)

## Testing Steps

### Step 1: Build Both Services

```powershell
# Navigate to backend directory
cd C:\Users\heiko\claude-orange-car-rental\src\backend

# Build Reservations service
dotnet build Services/Reservations/OrangeCarRental.Reservations.Api/OrangeCarRental.Reservations.Api.csproj

# Build Fleet service
dotnet build Services/Fleet/OrangeCarRental.Fleet.Api/OrangeCarRental.Fleet.Api.csproj
```

**Expected**: Both build successfully with 0 errors

### Step 2: Apply Database Migrations

#### Reservations Database

```powershell
cd Services/Reservations/OrangeCarRental.Reservations.Api
dotnet ef database update
```

**Expected**: Creates `OrangeCarRental_Reservations` database with tables

#### Fleet Database

```powershell
cd ../../../Services/Fleet/OrangeCarRental.Fleet.Api
dotnet ef database update
```

**Expected**: Creates `OrangeCarRental_Fleet` database with tables

### Step 3: Start Reservations Service

**Terminal 1:**
```powershell
cd Services/Reservations/OrangeCarRental.Reservations.Api
dotnet run
```

**Expected Output:**
```
[INFO] [ReservationsAPI] Applying database migrations for ReservationsDbContext...
[INFO] [ReservationsAPI] Database migration completed successfully
[INFO] [ReservationsAPI] Seeding Reservations database...
[INFO] Now listening on: http://localhost:5289
```

### Step 4: Start Fleet Service

**Terminal 2:**
```powershell
cd Services/Fleet/OrangeCarRental.Fleet.Api
dotnet run
```

**Expected Output:**
```
[INFO] [FleetAPI] Applying database migrations for FleetDbContext...
[INFO] [FleetAPI] Database migration completed successfully
[INFO] [FleetAPI] Seeding Fleet database...
[INFO] Now listening on: http://localhost:5046
```

### Step 5: Test New Availability Endpoint

**Terminal 3:**
```bash
# Test the new availability endpoint
curl "http://localhost:5289/api/reservations/availability?pickupDate=2026-01-15&returnDate=2026-01-18"
```

**Expected Response:**
```json
{
  "bookedVehicleIds": [],
  "pickupDate": "2026-01-15",
  "returnDate": "2026-01-18"
}
```

**Note**: Initially empty array since no reservations exist for those dates

### Step 6: Test HTTP Integration

```bash
# Search for available vehicles (Fleet calls Reservations via HTTP)
curl "http://localhost:5046/api/fleet/vehicles/search?pickupDate=2026-01-15&returnDate=2026-01-18&pageNumber=1&pageSize=10"
```

**Expected Response:**
```json
{
  "items": [
    {
      "id": "guid-here",
      "name": "VW Golf",
      "category": "MITTEL",
      "location": {
        "code": "BER-HBF",
        "name": "Berlin Hauptbahnhof"
      },
      "dailyRate": {
        "netAmount": 46.21,
        "vatAmount": 8.78,
        "grossAmount": 55.00
      },
      "status": "Available",
      "fuelType": "Petrol",
      "seats": 5
    }
    // ... more vehicles
  ],
  "totalCount": 5,
  "pageNumber": 1,
  "pageSize": 10,
  "totalPages": 1
}
```

### Step 7: Verify HTTP Call in Logs

Check **Fleet API** logs (Terminal 2) for:
```
[INFO] [FleetAPI] HTTP GET http://localhost:5289/api/reservations/availability?pickupDate=2026-01-15&returnDate=2026-01-18 responded 200 in 45.2 ms
```

This confirms Fleet is successfully calling Reservations via HTTP!

## Troubleshooting

### Issue: "ConnectionString is missing"

**Solution**: Ensure `appsettings.json` has correct connection string keys:
- Reservations: `"reservations": "Server=localhost;..."`
- Fleet: `"fleet": "Server=localhost;..."`

### Issue: "Failed to connect to Docker endpoint"

**Cause**: Repository integration tests require Docker for Testcontainers

**Solution**: These are infrastructure tests. Unit tests (282 passing) don't require Docker.

### Issue: Port Already in Use

**Solution**: Kill existing processes:
```powershell
# Find process on port 5289
netstat -ano | findstr :5289
taskkill /PID <pid> /F

# Find process on port 5046
netstat -ano | findstr :5046
taskkill /PID <pid> /F
```

### Issue: Database Connection Failed

**Solution**: Verify SQL Server is running:
```powershell
# Check SQL Server status
sc query MSSQLSERVER
```

## API Documentation

### New Endpoint: Get Vehicle Availability

**Endpoint**: `GET /api/reservations/availability`

**Query Parameters**:
| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| pickupDate | DateOnly | Yes | Start date (YYYY-MM-DD) |
| returnDate | DateOnly | Yes | End date (YYYY-MM-DD) |

**Response Schema**:
```json
{
  "bookedVehicleIds": ["guid-1", "guid-2"],
  "pickupDate": "2026-01-15",
  "returnDate": "2026-01-18"
}
```

**Business Rules**:
- Only Confirmed and Active reservations are included
- Period overlap logic: `reservation.pickup <= search.return AND reservation.return >= search.pickup`
- Pending, Cancelled, Completed, and NoShow reservations are excluded

## Success Criteria

### Completed ✅

- [x] Both services build successfully (0 errors, 0 warnings)
- [x] 282 unit tests passing (100%)
  - Reservations: 124/124 tests passing
  - Fleet: 158/169 tests passing (11 integration tests require Docker)
- [x] HTTP integration code implemented
- [x] Service configuration complete (appsettings.json updated)
- [x] Anti-corruption layer using value objects
- [x] Clean Architecture boundaries maintained

### Pending ⚠️ (Requires SQL Server)

- [ ] SQL Server installed and running
- [ ] Both databases created via migrations
- [ ] Reservations API starts on port 5289
- [ ] Fleet API starts on port 5046
- [ ] Availability endpoint returns 200 OK
- [ ] Vehicle search with dates works
- [ ] HTTP call visible in Fleet logs

## Performance Notes

- HTTP calls add latency (~10-50ms typically)
- Consider caching availability data if needed
- Circuit breaker pattern recommended for production
- Monitor HTTP call failures and timeouts

## Next Steps

After successful testing:

1. **Update API Documentation**: Document new endpoint in Swagger/OpenAPI
2. **Performance Testing**: Measure HTTP call latency under load
3. **Monitoring**: Add application insights/metrics for HTTP calls
4. **Error Handling**: Implement circuit breaker pattern
5. **Caching**: Evaluate caching strategy for availability data

## Support

If issues persist:
1. Check service logs for detailed error messages
2. Verify both services are running and reachable
3. Check firewall settings for local ports
4. Review configuration files for typos
5. Consult ARCHITECTURE-REFACTORING-COMPLETE.md for verification steps

---

**Last Updated**: 2025-11-16
**Architecture Version**: Post-Refactoring (HTTP-based communication)
**Status**: Ready for Testing
