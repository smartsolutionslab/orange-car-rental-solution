# Implementation Guide: Decoupling Fleet and Reservations Services

**Priority:** 🔴 CRITICAL
**Estimated Effort:** 2-3 days
**Impact:** Enables independent deployment and scaling of Fleet and Reservations services

---

## Problem Statement

Currently, the Fleet service directly queries the Reservations database to check vehicle availability. This violates microservice boundaries and creates tight coupling.

**Current Implementation:**
```csharp
// Fleet\Infrastructure\Persistence\VehicleRepository.cs:80-86
var bookedVehicleIds = await reservationsContext.Reservations
    .Where(r => (r.Status == ReservationStatus.Confirmed || r.Status == ReservationStatus.Active) &&
                r.Period.PickupDate <= searchPeriod.ReturnDate &&
                r.Period.ReturnDate >= searchPeriod.PickupDate)
    .Select(r => r.VehicleId)
    .ToListAsync(cancellationToken);
```

**Issues:**
- Fleet.Infrastructure depends on Reservations.Infrastructure
- Direct database access across service boundaries
- Cannot deploy services independently
- Schema changes in Reservations affect Fleet
- No clear service contract

---

## Solution: API-Based Communication

Replace direct database access with HTTP API calls from Fleet to Reservations.

### Architecture Overview

```
┌─────────────────┐          HTTP GET          ┌──────────────────────┐
│                 │  /api/reservations/        │                      │
│  Fleet Service  │  availability?             │ Reservations Service │
│                 │  ─────────────────────>    │                      │
│  VehicleRepo    │  <- Booked Vehicle IDs     │  New API Endpoint    │
└─────────────────┘                            └──────────────────────┘
```

---

## Implementation Steps

### Step 1: Create Availability DTO in Reservations

**File:** `Services/Reservations/OrangeCarRental.Reservations.Application/Queries/GetVehicleAvailability/VehicleAvailabilityDto.cs`

```csharp
namespace SmartSolutionsLab.OrangeCarRental.Reservations.Application.Queries.GetVehicleAvailability;

/// <summary>
///     DTO for vehicle availability check results.
/// </summary>
public sealed record VehicleAvailabilityDto
{
    /// <summary>
    ///     IDs of vehicles that are booked during the specified period.
    /// </summary>
    public required IReadOnlyList<Guid> BookedVehicleIds { get; init; }

    /// <summary>
    ///     The period that was checked.
    /// </summary>
    public required DateOnly PickupDate { get; init; }
    public required DateOnly ReturnDate { get; init; }

    /// <summary>
    ///     Total number of active reservations during this period.
    /// </summary>
    public required int TotalActiveReservations { get; init; }
}
```

---

### Step 2: Create Query in Reservations

**File:** `Services/Reservations/OrangeCarRental.Reservations.Application/Queries/GetVehicleAvailability/GetVehicleAvailabilityQuery.cs`

```csharp
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;

namespace SmartSolutionsLab.OrangeCarRental.Reservations.Application.Queries.GetVehicleAvailability;

/// <summary>
///     Query to get IDs of vehicles that are booked during a specific period.
/// </summary>
public sealed record GetVehicleAvailabilityQuery(
    DateOnly PickupDate,
    DateOnly ReturnDate
) : IQuery<VehicleAvailabilityDto>;
```

---

### Step 3: Create Query Handler in Reservations

**File:** `Services/Reservations/OrangeCarRental.Reservations.Application/Queries/GetVehicleAvailability/GetVehicleAvailabilityQueryHandler.cs`

```csharp
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Reservation;

namespace SmartSolutionsLab.OrangeCarRental.Reservations.Application.Queries.GetVehicleAvailability;

/// <summary>
///     Handler for getting vehicle availability (booked vehicle IDs).
/// </summary>
public sealed class GetVehicleAvailabilityQueryHandler(IReservationRepository reservations)
    : IQueryHandler<GetVehicleAvailabilityQuery, VehicleAvailabilityDto>
{
    public async Task<VehicleAvailabilityDto> HandleAsync(
        GetVehicleAvailabilityQuery query,
        CancellationToken cancellationToken = default)
    {
        // Get all confirmed or active reservations that overlap with the search period
        var bookedVehicleIds = await reservations.GetBookedVehicleIdsAsync(
            query.PickupDate,
            query.ReturnDate,
            cancellationToken);

        return new VehicleAvailabilityDto
        {
            BookedVehicleIds = bookedVehicleIds,
            PickupDate = query.PickupDate,
            ReturnDate = query.ReturnDate,
            TotalActiveReservations = bookedVehicleIds.Count
        };
    }
}
```

---

### Step 4: Add Repository Method in Reservations

**File:** `Services/Reservations/OrangeCarRental.Reservations.Domain/Reservation/IReservationRepository.cs`

Add this method to the interface:

```csharp
/// <summary>
///     Gets IDs of all vehicles that are booked (Confirmed or Active) during the specified period.
/// </summary>
/// <param name="pickupDate">Start of the period to check.</param>
/// <param name="returnDate">End of the period to check.</param>
/// <param name="cancellationToken">Cancellation token.</param>
/// <returns>List of vehicle IDs that are unavailable during this period.</returns>
Task<IReadOnlyList<Guid>> GetBookedVehicleIdsAsync(
    DateOnly pickupDate,
    DateOnly returnDate,
    CancellationToken cancellationToken = default);
```

---

### Step 5: Implement Repository Method

**File:** `Services/Reservations/OrangeCarRental.Reservations.Infrastructure/Persistence/Repositories/ReservationRepository.cs`

```csharp
public async Task<IReadOnlyList<Guid>> GetBookedVehicleIdsAsync(
    DateOnly pickupDate,
    DateOnly returnDate,
    CancellationToken cancellationToken = default)
{
    return await context.Reservations
        .Where(r =>
            (r.Status == ReservationStatus.Confirmed || r.Status == ReservationStatus.Active) &&
            r.Period.PickupDate <= returnDate &&
            r.Period.ReturnDate >= pickupDate)
        .Select(r => r.VehicleId)
        .Distinct()
        .ToListAsync(cancellationToken);
}
```

---

### Step 6: Create API Endpoint in Reservations

**File:** `Services/Reservations/OrangeCarRental.Reservations.Api/Extensions/ReservationEndpoints.cs`

Add this endpoint:

```csharp
/// <summary>
///     GET /api/reservations/availability
///     Checks which vehicles are booked during a specific period.
/// </summary>
reservations.MapGet("/availability", async (
    [FromQuery] DateOnly pickupDate,
    [FromQuery] DateOnly returnDate,
    GetVehicleAvailabilityQueryHandler handler,
    CancellationToken cancellationToken) =>
{
    var query = new GetVehicleAvailabilityQuery(pickupDate, returnDate);
    var result = await handler.HandleAsync(query, cancellationToken);
    return Results.Ok(result);
})
.WithName("GetVehicleAvailability")
.WithTags("Reservations")
.Produces<VehicleAvailabilityDto>(StatusCodes.Status200OK)
.WithOpenApi(operation => new(operation)
{
    Summary = "Get vehicle availability",
    Description = "Returns IDs of vehicles that are booked during the specified period"
});
```

Register the handler in `Program.cs`:

```csharp
builder.Services.AddScoped<GetVehicleAvailabilityQueryHandler>();
```

---

### Step 7: Create Service Interface in Fleet

**File:** `Services/Fleet/OrangeCarRental.Fleet.Application/Services/IReservationService.cs`

```csharp
namespace SmartSolutionsLab.OrangeCarRental.Fleet.Application.Services;

/// <summary>
///     Service for checking vehicle availability via the Reservations API.
/// </summary>
public interface IReservationService
{
    /// <summary>
    ///     Gets IDs of vehicles that are booked during the specified period.
    /// </summary>
    /// <param name="pickupDate">Start date of the rental period.</param>
    /// <param name="returnDate">End date of the rental period.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of vehicle IDs that are unavailable.</returns>
    Task<IReadOnlyList<Guid>> GetBookedVehicleIdsAsync(
        DateOnly pickupDate,
        DateOnly returnDate,
        CancellationToken cancellationToken = default);
}
```

---

### Step 8: Implement HTTP Client in Fleet

**File:** `Services/Fleet/OrangeCarRental.Fleet.Infrastructure/Services/ReservationService.cs`

```csharp
using System.Net.Http.Json;
using SmartSolutionsLab.OrangeCarRental.Fleet.Application.Services;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Infrastructure.Services;

/// <summary>
///     HTTP client for communicating with the Reservations service.
/// </summary>
public sealed class ReservationService(HttpClient httpClient) : IReservationService
{
    public async Task<IReadOnlyList<Guid>> GetBookedVehicleIdsAsync(
        DateOnly pickupDate,
        DateOnly returnDate,
        CancellationToken cancellationToken = default)
    {
        var response = await httpClient.GetFromJsonAsync<VehicleAvailabilityResponse>(
            $"/api/reservations/availability?pickupDate={pickupDate:yyyy-MM-dd}&returnDate={returnDate:yyyy-MM-dd}",
            cancellationToken);

        return response?.BookedVehicleIds ?? Array.Empty<Guid>();
    }

    private sealed record VehicleAvailabilityResponse(
        IReadOnlyList<Guid> BookedVehicleIds,
        DateOnly PickupDate,
        DateOnly ReturnDate,
        int TotalActiveReservations);
}
```

---

### Step 9: Register HTTP Client in Fleet

**File:** `Services/Fleet/OrangeCarRental.Fleet.Api/Program.cs`

```csharp
// Register typed HTTP client for Reservations service
builder.Services.AddHttpClient<IReservationService, ReservationService>(client =>
{
    // Use service discovery or configuration
    var reservationsUrl = builder.Configuration["Services:Reservations:Url"]
                          ?? "http://localhost:5002"; // Default for local development
    client.BaseAddress = new Uri(reservationsUrl);
    client.DefaultRequestHeaders.Add("User-Agent", "Fleet-Service");
});

// Add resilience policies (optional but recommended)
builder.Services.AddHttpClient<IReservationService, ReservationService>()
    .AddPolicyHandler(GetRetryPolicy())
    .AddPolicyHandler(GetCircuitBreakerPolicy());

static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
}

static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));
}
```

**Configuration:** `appsettings.json`

```json
{
  "Services": {
    "Reservations": {
      "Url": "http://localhost:5002"
    }
  }
}
```

---

### Step 10: Update VehicleRepository in Fleet

**File:** `Services/Fleet/OrangeCarRental.Fleet.Infrastructure/Persistence/VehicleRepository.cs`

Replace the current database access with service call:

```csharp
// BEFORE (WRONG - Direct database access)
// var bookedVehicleIds = await reservationsContext.Reservations
//     .Where(r => ...)
//     .Select(r => r.VehicleId)
//     .ToListAsync(cancellationToken);

// AFTER (CORRECT - Service call)
private readonly IReservationService _reservationService;

public VehicleRepository(
    FleetDbContext context,
    IReservationService reservationService)  // Add dependency
{
    this.context = context;
    _reservationService = reservationService;
}

public async Task<PagedResult<Vehicle>> SearchAsync(
    VehicleSearchParameters parameters,
    CancellationToken cancellationToken = default)
{
    var query = context.Vehicles.AsQueryable();

    // ... existing filters ...

    // Check availability if search period is provided
    if (parameters.PickupDate.HasValue && parameters.ReturnDate.HasValue)
    {
        // Call Reservations service instead of direct DB access
        var bookedVehicleIds = await _reservationService.GetBookedVehicleIdsAsync(
            parameters.PickupDate.Value,
            parameters.ReturnDate.Value,
            cancellationToken);

        query = query.Where(v => !bookedVehicleIds.Contains(v.Id.Value));
    }

    // ... rest of method ...
}
```

---

### Step 11: Remove Cross-Service References

**File:** `Services/Fleet/OrangeCarRental.Fleet.Infrastructure/OrangeCarRental.Fleet.Infrastructure.csproj`

Remove these lines:

```xml
<!-- DELETE THIS LINE -->
<ProjectReference Include="..\..\Reservations\OrangeCarRental.Reservations.Infrastructure\..." />
```

**File:** `Services/Fleet/OrangeCarRental.Fleet.Api/OrangeCarRental.Fleet.Api.csproj`

Remove:

```xml
<!-- DELETE THIS LINE IF IT EXISTS -->
<ProjectReference Include="..\..\Reservations\OrangeCarRental.Reservations.Infrastructure\..." />
```

Remove these using statements from VehicleRepository.cs:

```csharp
// DELETE THESE
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Reservation;
using SmartSolutionsLab.OrangeCarRental.Reservations.Infrastructure.Persistence;
```

---

## Testing the Implementation

### 1. Unit Test the Query Handler

**File:** `Services/Reservations/OrangeCarRental.Reservations.Tests/Application/Queries/GetVehicleAvailabilityQueryHandlerTests.cs`

```csharp
public class GetVehicleAvailabilityQueryHandlerTests
{
    [Fact]
    public async Task HandleAsync_WithBookedVehicles_ShouldReturnIds()
    {
        // Arrange
        var pickupDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));
        var returnDate = pickupDate.AddDays(3);

        var bookedIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };

        var repositoryMock = new Mock<IReservationRepository>();
        repositoryMock
            .Setup(x => x.GetBookedVehicleIdsAsync(pickupDate, returnDate, It.IsAny<CancellationToken>()))
            .ReturnsAsync(bookedIds);

        var handler = new GetVehicleAvailabilityQueryHandler(repositoryMock.Object);
        var query = new GetVehicleAvailabilityQuery(pickupDate, returnDate);

        // Act
        var result = await handler.HandleAsync(query, CancellationToken.None);

        // Assert
        result.BookedVehicleIds.ShouldBe(bookedIds);
        result.TotalActiveReservations.ShouldBe(2);
    }
}
```

### 2. Integration Test the HTTP Communication

```csharp
[Fact]
public async Task SearchVehicles_ShouldCallReservationsService()
{
    // Use WebApplicationFactory to test HTTP communication
    // Verify Fleet service properly calls Reservations API
}
```

### 3. Manual Testing

```bash
# 1. Start Reservations service
cd Services/Reservations/OrangeCarRental.Reservations.Api
dotnet run

# 2. Test availability endpoint
curl "http://localhost:5002/api/reservations/availability?pickupDate=2025-12-01&returnDate=2025-12-05"

# 3. Start Fleet service
cd Services/Fleet/OrangeCarRental.Fleet.Api
dotnet run

# 4. Test vehicle search
curl "http://localhost:5001/api/fleet/search?pickupDate=2025-12-01&returnDate=2025-12-05"
```

---

## Rollout Plan

### Phase 1: Add New Endpoint (Backward Compatible)
1. Deploy new Reservations endpoint
2. Verify endpoint works correctly
3. No changes to Fleet yet

### Phase 2: Deploy Fleet Changes
1. Deploy Fleet with new HTTP client
2. Remove old database access code
3. Remove project references

### Phase 3: Verify and Monitor
1. Monitor HTTP call success rate
2. Check response times
3. Verify no errors in logs

---

## Rollback Plan

If issues occur:

1. Revert Fleet deployment to previous version (still has database access)
2. Keep Reservations endpoint (harmless to have it)
3. Investigate issues
4. Re-deploy when fixed

---

## Performance Considerations

**HTTP Call Overhead:**
- Adds ~50-100ms network latency per request
- Acceptable for vehicle search queries

**Mitigation:**
- Implement caching for frequently searched periods
- Use HTTP/2 for connection reuse
- Consider gRPC for better performance (future optimization)

**Caching Strategy:**

```csharp
// Cache availability results for 30 seconds
services.AddMemoryCache();

public class CachedReservationService : IReservationService
{
    private readonly IReservationService _inner;
    private readonly IMemoryCache _cache;

    public async Task<IReadOnlyList<Guid>> GetBookedVehicleIdsAsync(...)
    {
        var cacheKey = $"availability:{pickupDate}:{returnDate}";

        if (_cache.TryGetValue(cacheKey, out IReadOnlyList<Guid> cached))
            return cached;

        var result = await _inner.GetBookedVehicleIdsAsync(...);
        _cache.Set(cacheKey, result, TimeSpan.FromSeconds(30));
        return result;
    }
}
```

---

## Success Criteria

✅ Fleet service does not reference Reservations.Infrastructure
✅ VehicleRepository uses IReservationService
✅ Reservations API endpoint returns correct booked vehicle IDs
✅ Vehicle search correctly excludes booked vehicles
✅ All unit tests pass
✅ Integration tests verify HTTP communication
✅ No cross-service database dependencies

---

## Estimated Timeline

- **Day 1:** Create Reservations endpoint and query handler (Steps 1-6)
- **Day 2:** Create Fleet service interface and HTTP client (Steps 7-9)
- **Day 3:** Update VehicleRepository, remove references, test (Steps 10-11)

**Total: 2-3 days**

---

## Questions or Issues?

Contact the architecture team or refer to:
- Architecture Review Document: `ARCHITECTURE-REVIEW.md`
- Clean Architecture Guidelines: `docs/clean-architecture.md`
