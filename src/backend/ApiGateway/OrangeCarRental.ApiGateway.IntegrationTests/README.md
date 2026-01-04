# API Gateway Integration Tests

Comprehensive integration tests for the Orange Car Rental API Gateway using Aspire Hosting Testing framework.

## Overview

These tests verify the complete distributed application stack, including:
- API Gateway (YARP reverse proxy)
- Fleet API service
- Reservations API service
- SQL Server database
- Service discovery and orchestration

## Test Coverage

### Health Check Tests
- ✅ Gateway health endpoint verification
- ✅ Fleet API health through gateway
- ✅ Reservations API health through gateway

### Routing Tests
- ✅ Fleet API routing (`/api/vehicles/*`)
- ✅ Reservations API routing (`/api/reservations/*`)
- ✅ Invalid route handling (404 responses)

### Functionality Tests
- ✅ Vehicle search through gateway
- ✅ Vehicle search with filters (category, pagination)
- ✅ Pagination metadata validation
- ✅ Reservation creation through gateway
- ✅ Concurrent requests to multiple services

## Running the Tests

### Prerequisites
- .NET 10.0 SDK
- Docker Desktop (for SQL Server container)
- All backend services buildable

### Run All Tests
```bash
dotnet test
```

### Run Specific Test
```bash
dotnet test --filter "FullyQualifiedName~Gateway_HealthEndpoint_ReturnsHealthy"
```

### Run with Verbose Output
```bash
dotnet test --verbosity detailed
```

## Test Architecture

### Aspire Hosting Testing
The tests use `Aspire.Hosting.Testing` which provides:
- Automatic distributed application lifecycle management
- Test isolation with separate app instances per test class
- HTTP client creation for testing services
- Proper cleanup and disposal

### Test Structure
```csharp
public class ApiGatewayIntegrationTests : IAsyncLifetime
{
    // InitializeAsync: Starts the distributed application
    // Test methods: Make HTTP requests and assert responses
    // DisposeAsync: Cleans up resources
}
```

### Key Features
1. **Full Stack Testing**: Tests run against real services, not mocks
2. **Isolation**: Each test class gets its own app instance
3. **Realistic**: Includes database migrations, seed data, networking
4. **Fast**: Aspire optimizes startup and parallel execution

## Test Data

Tests rely on seed data created during service startup:
- **Fleet API**: 30 pre-seeded vehicles across 5 German cities
- **Reservations API**: Empty initially, tests create reservations

## Important Notes

### Running Locally
- ⚠️ Do not run these tests while the AppHost is already running
- The tests will start their own instance of the distributed application
- Stop any running `dotnet run` commands from AppHost before testing

### CI/CD Integration
These tests are designed for CI/CD pipelines:
```yaml
- name: Run Integration Tests
  run: dotnet test --filter Category=Integration
```

### Test Duration
- Expected duration: 30-60 seconds per test class
- Includes application startup, database migrations, and cleanup
- Parallel execution enabled for faster CI/CD

## Troubleshooting

### Port Conflicts
If tests fail with port binding errors:
1. Stop any running backend services
2. Check Docker containers: `docker ps`
3. Stop conflicting containers: `docker stop <container-id>`

### Database Issues
If database migrations fail:
1. Ensure Docker Desktop is running
2. Clear old SQL Server containers: `docker rm -f $(docker ps -aq --filter ancestor=mcr.microsoft.com/mssql/server)`
3. Restart tests

### Timeout Issues
If tests timeout during startup:
1. Increase test timeout in `xunit.runner.json`
2. Check Docker resource allocation (CPU/Memory)
3. Verify all services build successfully

## Future Enhancements

Potential additions to the test suite:
- [ ] Performance tests (load testing through gateway)
- [ ] Security tests (authentication, authorization)
- [ ] Resilience tests (circuit breaker, retry policies)
- [ ] WebSocket support tests (if applicable)
- [ ] Rate limiting tests
- [ ] Response caching tests

## References

- [Aspire Testing Documentation](https://learn.microsoft.com/en-us/dotnet/aspire/fundamentals/testing)
- [YARP Documentation](https://microsoft.github.io/reverse-proxy/)
- [xUnit Documentation](https://xunit.net/)
