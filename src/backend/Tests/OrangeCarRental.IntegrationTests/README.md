# Orange Car Rental - Integration Tests

This project contains integration tests that spin up the entire Aspire application including:
- SQL Server database
- Fleet API
- Reservations API
- API Gateway
- All infrastructure components

## Prerequisites

- .NET 9 SDK
- Docker Desktop (for SQL Server container)
- **IMPORTANT:** Stop any running instances of the Aspire AppHost before running tests or building

## Running the Tests

### Stop Running Services First

Before running integration tests, make sure to stop the Aspire AppHost if it's running:

```bash
# Find and kill any running processes
# Windows PowerShell:
Get-Process | Where-Object {$_.ProcessName -like "*OrangeCarRental*"} | Stop-Process -Force
```

### Run All Tests

```bash
cd src/backend/Tests/OrangeCarRental.IntegrationTests
dotnet test
```

### Run Specific Test Class

```bash
# Run only API Gateway tests
dotnet test --filter ClassName=ApiGatewayTests

# Run only End-to-End tests
dotnet test --filter ClassName=EndToEndScenarioTests
```

### Run Specific Test

```bash
dotnet test --filter Name=ApiGateway_RoutesToFleetApi_SearchVehicles
```

## Test Structure

### `ApiGatewayTests`
Tests the API Gateway routing and integration with backend services:
- Health check endpoints
- Routing to Fleet API
- Routing to Reservations API
- Direct access to services
- SQL Server availability

### `EndToEndScenarioTests`
Tests complete user workflows:
- Complete rental flow (search vehicles + create reservation)
- Vehicle search with filters
- German VAT pricing validation
- Cross-service communication

### `DistributedApplicationFixture`
Shared fixture that manages the Aspire application lifecycle:
- Starts the AppHost
- Waits for all resources to be ready
- Provides HTTP clients for each service
- Cleans up after tests complete

## How It Works

The integration tests use **Aspire.Hosting.Testing** which:

1. Builds and starts the entire Aspire application
2. Waits for all resources (SQL Server, APIs) to be healthy
3. Provides `CreateHttpClient()` to call services
4. Automatically handles cleanup

This gives you **true integration tests** with:
- Real SQL Server database (in Docker)
- Real HTTP calls between services
- Real API Gateway routing
- Complete Aspire orchestration

## Test Data

Tests use the database seed data from `DbInitializer`:
- Vehicles at different locations (BER, MUC, FRA, HAM)
- Different vehicle categories (ECONOMY, COMPACT, etc.)
- German VAT pricing (19%)

## Troubleshooting

### Port conflicts
If you see port binding errors, make sure no other instances are running:
```bash
netstat -ano | findstr "5002"  # API Gateway
netstat -ano | findstr "1433"  # SQL Server
```

### SQL Server not starting
Ensure Docker Desktop is running and has enough resources allocated.

### Tests timeout
The fixture waits up to 2 minutes for resources. If tests timeout:
- Check Docker is running
- Increase timeout in `DistributedApplicationFixture.cs`
- Check for resource constraints

## CI/CD Integration

These tests are designed to run in CI/CD pipelines:

```yaml
# Example GitHub Actions
- name: Run Integration Tests
  run: dotnet test src/backend/Tests/OrangeCarRental.IntegrationTests
  env:
    ASPIRE_ALLOW_UNSECURED_TRANSPORT: true
```

## Performance

- First run: ~30-60 seconds (downloads SQL Server image)
- Subsequent runs: ~15-30 seconds
- Fixture is shared across tests in the same class for efficiency
