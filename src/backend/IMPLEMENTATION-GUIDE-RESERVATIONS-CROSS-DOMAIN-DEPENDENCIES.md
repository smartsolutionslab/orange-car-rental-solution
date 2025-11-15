# Implementation Guide: Removing Cross-Domain Dependencies in Reservations Service

**Priority:** 🔴 CRITICAL
**Estimated Effort:** 1-2 days
**Impact:** Enables independent versioning and deployment of microservices

---

## Problem Statement

The Reservations.Application layer currently references domain layers from other bounded contexts (Customers and Fleet), creating compile-time coupling between microservices.

**Current Dependencies:**

```xml
<!-- Services/Reservations/OrangeCarRental.Reservations.Application/OrangeCarRental.Reservations.Application.csproj -->
<ProjectReference Include="..\..\Customers\OrangeCarRental.Customers.Domain\..." />
<ProjectReference Include="..\..\Fleet\OrangeCarRental.Fleet.Domain\..." />
```

**Current Usage:**

```csharp
// CreateReservationCommand.cs
using SmartSolutionsLab.OrangeCarRental.Customers.Domain.Customer;  // ❌
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Vehicle;        // ❌

public sealed record CreateReservationCommand(
    VehicleIdentifier VehicleId,      // From Fleet.Domain
    CustomerIdentifier CustomerId,     // From Customers.Domain
    VehicleCategory CategoryCode,     // From Fleet.Domain
    ...
)
```

**Issues:**
- Services cannot be versioned independently
- Changes to Customer or Fleet domain models require recompiling Reservations
- Violates DDD bounded context isolation
- Prevents independent deployment cycles
- Creates tight coupling at compile time

---

## Solution: Internal Value Objects (Anti-Corruption Layer)

Each bounded context should own its own value objects, even if they represent similar concepts from other contexts.

### DDD Principle: Bounded Context Autonomy

> "Each bounded context should have its own ubiquitous language and domain model. References to other contexts should be by ID only, with the context defining its own value objects to represent those IDs."
> — Domain-Driven Design, Eric Evans

---

## Implementation Steps

### Step 1: Create Internal Identifiers in Reservations.Domain

**File:** `Services/Reservations/OrangeCarRental.Reservations.Domain/Shared/ReservationVehicleId.cs`

```csharp
namespace SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Shared;

/// <summary>
///     Value object representing a vehicle ID within the Reservations bounded context.
///     This is an internal representation, not a reference to Fleet.Domain.Vehicle.VehicleIdentifier.
/// </summary>
public readonly record struct ReservationVehicleId
{
    private ReservationVehicleId(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("Vehicle ID cannot be empty", nameof(value));

        Value = value;
    }

    public Guid Value { get; }

    /// <summary>
    ///     Creates a new vehicle ID from a GUID.
    /// </summary>
    public static ReservationVehicleId From(Guid value) => new(value);

    /// <summary>
    ///     Implicit conversion to Guid for database mapping.
    /// </summary>
    public static implicit operator Guid(ReservationVehicleId id) => id.Value;

    public override string ToString() => Value.ToString();
}
```

**File:** `Services/Reservations/OrangeCarRental.Reservations.Domain/Shared/ReservationCustomerId.cs`

```csharp
namespace SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Shared;

/// <summary>
///     Value object representing a customer ID within the Reservations bounded context.
///     This is an internal representation, not a reference to Customers.Domain.Customer.CustomerIdentifier.
/// </summary>
public readonly record struct ReservationCustomerId
{
    private ReservationCustomerId(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("Customer ID cannot be empty", nameof(value));

        Value = value;
    }

    public Guid Value { get; }

    public static ReservationCustomerId From(Guid value) => new(value);

    public static implicit operator Guid(ReservationCustomerId id) => id.Value;

    public override string ToString() => Value.ToString();
}
```

---

### Step 2: Create Internal Vehicle Category in Reservations.Domain

**File:** `Services/Reservations/OrangeCarRental.Reservations.Domain/Shared/ReservationVehicleCategory.cs`

```csharp
namespace SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Shared;

/// <summary>
///     Value object representing a vehicle category within the Reservations bounded context.
///     Maps to categories from the Fleet service but is defined locally for context autonomy.
/// </summary>
public readonly record struct ReservationVehicleCategory
{
    private ReservationVehicleCategory(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Category code cannot be empty", nameof(code));

        if (code.Length > 20)
            throw new ArgumentException("Category code cannot exceed 20 characters", nameof(code));

        Code = code.ToUpperInvariant();
    }

    public string Code { get; }

    public static ReservationVehicleCategory From(string code) => new(code);

    // Predefined categories (matching Fleet service but owned by Reservations context)
    public static readonly ReservationVehicleCategory Kleinwagen = From("KLEIN");
    public static readonly ReservationVehicleCategory Kompaktklasse = From("KOMPAKT");
    public static readonly ReservationVehicleCategory Mittelklasse = From("MITTEL");
    public static readonly ReservationVehicleCategory Oberklasse = From("OBER");
    public static readonly ReservationVehicleCategory SUV = From("SUV");
    public static readonly ReservationVehicleCategory Kombi = From("KOMBI");
    public static readonly ReservationVehicleCategory Transporter = From("TRANS");
    public static readonly ReservationVehicleCategory Luxus = From("LUXUS");

    public static implicit operator string(ReservationVehicleCategory category) => category.Code;

    public override string ToString() => Code;
}
```

---

### Step 3: Update Reservation Entity

**File:** `Services/Reservations/OrangeCarRental.Reservations.Domain/Reservation/Reservation.cs`

Replace references to external domain types:

```csharp
// BEFORE (WRONG - References other domains)
using SmartSolutionsLab.OrangeCarRental.Customers.Domain.Customer;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Vehicle;

public sealed class Reservation : AggregateRoot<ReservationIdentifier>
{
    public VehicleIdentifier VehicleId { get; init; }      // ❌ From Fleet.Domain
    public CustomerIdentifier CustomerId { get; init; }    // ❌ From Customers.Domain
    ...
}

// AFTER (CORRECT - Uses internal value objects)
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Shared;

public sealed class Reservation : AggregateRoot<ReservationIdentifier>
{
    public ReservationVehicleId VehicleId { get; init; }      // ✅ Internal
    public ReservationCustomerId CustomerId { get; init; }    // ✅ Internal
    ...
}
```

Update the factory method:

```csharp
public static Reservation Create(
    ReservationVehicleId vehicleId,        // Changed
    ReservationCustomerId customerId,      // Changed
    ReservationVehicleCategory category,   // Changed (if stored)
    BookingPeriod period,
    LocationCode pickupLocationCode,
    LocationCode dropoffLocationCode,
    Money totalPrice)
{
    var reservation = new Reservation
    {
        Id = ReservationIdentifier.New(),
        VehicleId = vehicleId,
        CustomerId = customerId,
        Period = period,
        PickupLocationCode = pickupLocationCode,
        DropoffLocationCode = dropoffLocationCode,
        TotalPrice = totalPrice,
        Status = ReservationStatus.Pending,
        CreatedAtUtc = DateTime.UtcNow
    };

    reservation.AddDomainEvent(new ReservationCreated(
        reservation.Id.Value,
        vehicleId.Value,
        customerId.Value,
        period.PickupDate,
        period.ReturnDate));

    return reservation;
}
```

---

### Step 4: Update Commands in Reservations.Application

**File:** `Services/Reservations/OrangeCarRental.Reservations.Application/Commands/CreateReservation/CreateReservationCommand.cs`

```csharp
// BEFORE (WRONG)
using SmartSolutionsLab.OrangeCarRental.Customers.Domain.Customer;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Vehicle;

public sealed record CreateReservationCommand(
    VehicleIdentifier VehicleId,
    CustomerIdentifier CustomerId,
    VehicleCategory CategoryCode,
    ...
)

// AFTER (CORRECT)
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Shared;

public sealed record CreateReservationCommand(
    ReservationVehicleId VehicleId,
    ReservationCustomerId CustomerId,
    ReservationVehicleCategory CategoryCode,
    BookingPeriod Period,
    LocationCode PickupLocationCode,
    LocationCode DropoffLocationCode,
    Money? TotalPrice = null
) : ICommand<CreateReservationResult>;
```

---

### Step 5: Update Command Handlers

**File:** `Services/Reservations/OrangeCarRental.Reservations.Application/Commands/CreateReservation/CreateReservationCommandHandler.cs`

The handler remains largely unchanged since it already uses the command properties:

```csharp
public async Task<CreateReservationResult> HandleAsync(
    CreateReservationCommand command,
    CancellationToken cancellationToken = default)
{
    // Calculate price if not provided
    Money totalPrice;
    if (command.TotalPrice.HasValue)
    {
        totalPrice = command.TotalPrice.Value;
    }
    else
    {
        var priceCalculation = await pricingService.CalculatePriceAsync(
            command.CategoryCode.Code,  // Pass string code, not domain object
            command.Period,
            command.PickupLocationCode,
            cancellationToken);

        totalPrice = Money.Of(
            priceCalculation.TotalPriceNet,
            priceCalculation.VatRate,
            Currency.Of(priceCalculation.Currency));
    }

    // Create reservation (now uses internal value objects)
    var reservation = Reservation.Create(
        command.VehicleId,
        command.CustomerId,
        command.CategoryCode,
        command.Period,
        command.PickupLocationCode,
        command.DropoffLocationCode,
        totalPrice);

    await reservations.AddAsync(reservation, cancellationToken);
    await reservations.SaveChangesAsync(cancellationToken);

    return new CreateReservationResult
    {
        ReservationId = reservation.Id.Value,
        VehicleId = reservation.VehicleId.Value,
        CustomerId = reservation.CustomerId.Value,
        Status = reservation.Status.ToString(),
        NetAmount = reservation.TotalPrice.NetAmount,
        VatAmount = reservation.TotalPrice.VatAmount,
        GrossAmount = reservation.TotalPrice.GrossAmount,
        PickupDate = reservation.Period.PickupDate,
        ReturnDate = reservation.Period.ReturnDate
    };
}
```

---

### Step 6: Update CreateGuestReservationCommand

**File:** `Services/Reservations/OrangeCarRental.Reservations.Application/Commands/CreateGuestReservation/CreateGuestReservationCommand.cs`

```csharp
public sealed record CreateGuestReservationCommand(
    // Guest customer details (unchanged)
    string FirstName,
    string LastName,
    string Salutation,
    string Email,
    string PhoneNumber,
    DateOnly DateOfBirth,
    string Street,
    string City,
    string PostalCode,
    string Country,
    string DriversLicenseNumber,
    string DriversLicenseIssueCountry,
    DateOnly DriversLicenseIssueDate,
    DateOnly DriversLicenseExpiryDate,

    // Reservation details (changed)
    ReservationVehicleId VehicleId,           // Changed
    ReservationVehicleCategory CategoryCode,  // Changed
    BookingPeriod Period,
    LocationCode PickupLocationCode,
    LocationCode DropoffLocationCode
) : ICommand<CreateGuestReservationResult>;
```

**Handler Update:**

```csharp
public async Task<CreateGuestReservationResult> HandleAsync(
    CreateGuestReservationCommand command,
    CancellationToken cancellationToken = default)
{
    // 1. Register customer via Customers service
    var customerId = await customersService.RegisterCustomerAsync(
        command.FirstName,
        command.LastName,
        // ... other customer details
        cancellationToken);

    // 2. Calculate price via Pricing service
    var price = await pricingService.CalculatePriceAsync(
        command.CategoryCode.Code,
        command.Period,
        command.PickupLocationCode,
        cancellationToken);

    // 3. Create reservation
    var reservation = Reservation.Create(
        command.VehicleId,
        ReservationCustomerId.From(customerId),  // Convert external ID to internal type
        command.CategoryCode,
        command.Period,
        command.PickupLocationCode,
        command.DropoffLocationCode,
        Money.Of(price.TotalPriceNet, price.VatRate, Currency.Of(price.Currency)));

    await reservations.AddAsync(reservation, cancellationToken);
    await reservations.SaveChangesAsync(cancellationToken);

    return new CreateGuestReservationResult
    {
        ReservationId = reservation.Id.Value,
        CustomerId = customerId,
        VehicleId = command.VehicleId.Value,
        // ... rest of response
    };
}
```

---

### Step 7: Update API Layer Mapping

**File:** `Services/Reservations/OrangeCarRental.Reservations.Api/Extensions/ReservationEndpoints.cs`

Update request mapping to use internal types:

```csharp
// Request DTO
public sealed record CreateReservationRequest
{
    public required Guid VehicleId { get; init; }         // Still Guid from API
    public required Guid CustomerId { get; init; }        // Still Guid from API
    public required string CategoryCode { get; init; }    // String from API
    public required DateOnly PickupDate { get; init; }
    public required DateOnly ReturnDate { get; init; }
    public required string PickupLocationCode { get; init; }
    public required string DropoffLocationCode { get; init; }
}

// Endpoint mapping
reservations.MapPost("/", async (
    CreateReservationRequest request,
    CreateReservationCommandHandler handler,
    CancellationToken cancellationToken) =>
{
    var command = new CreateReservationCommand(
        ReservationVehicleId.From(request.VehicleId),        // Convert at API boundary
        ReservationCustomerId.From(request.CustomerId),       // Convert at API boundary
        ReservationVehicleCategory.From(request.CategoryCode), // Convert at API boundary
        BookingPeriod.Of(request.PickupDate, request.ReturnDate),
        LocationCode.Of(request.PickupLocationCode),
        LocationCode.Of(request.DropoffLocationCode),
        null);

    var result = await handler.HandleAsync(command, cancellationToken);

    return Results.Created($"/api/reservations/{result.ReservationId}", result);
})
.WithName("CreateReservation")
.Produces<CreateReservationResult>(StatusCodes.Status201Created)
.ProducesValidationProblem();
```

---

### Step 8: Update Database Entity Configuration (if needed)

**File:** `Services/Reservations/OrangeCarRental.Reservations.Infrastructure/Persistence/Configurations/ReservationConfiguration.cs`

Update value object conversions:

```csharp
public void Configure(EntityTypeBuilder<Reservation> builder)
{
    builder.HasKey(r => r.Id);

    builder.Property(r => r.Id)
        .HasConversion(
            id => id.Value,
            value => ReservationIdentifier.From(value));

    builder.Property(r => r.VehicleId)
        .HasConversion(
            id => id.Value,                        // Convert to Guid for DB
            value => ReservationVehicleId.From(value))  // Convert from DB to value object
        .IsRequired();

    builder.Property(r => r.CustomerId)
        .HasConversion(
            id => id.Value,
            value => ReservationCustomerId.From(value))
        .IsRequired();

    // ... rest of configuration
}
```

---

### Step 9: Update Tests

**File:** `Services/Reservations/OrangeCarRental.Reservations.Tests/Domain/Entities/ReservationTests.cs`

```csharp
// Update test data setup
private readonly ReservationVehicleId _validVehicleId = ReservationVehicleId.From(Guid.NewGuid());
private readonly ReservationCustomerId _validCustomerId = ReservationCustomerId.From(Guid.NewGuid());

[Fact]
public void Create_WithValidData_ShouldCreateReservation()
{
    // Arrange
    var vehicleId = ReservationVehicleId.From(Guid.NewGuid());
    var customerId = ReservationCustomerId.From(Guid.NewGuid());
    var category = ReservationVehicleCategory.SUV;
    // ...

    // Act
    var reservation = Reservation.Create(
        vehicleId,
        customerId,
        category,
        _validPeriod,
        _validPickupLocation,
        _validDropoffLocation,
        _validTotalPrice);

    // Assert
    reservation.VehicleId.ShouldBe(vehicleId);
    reservation.CustomerId.ShouldBe(customerId);
    // ...
}
```

---

### Step 10: Remove Cross-Domain References

**File:** `Services/Reservations/OrangeCarRental.Reservations.Application/OrangeCarRental.Reservations.Application.csproj`

Remove these lines:

```xml
<!-- DELETE THESE LINES -->
<ProjectReference Include="..\..\Customers\OrangeCarRental.Customers.Domain\..." />
<ProjectReference Include="..\..\Fleet\OrangeCarRental.Fleet.Domain\..." />
```

Remove using statements from all files in Reservations.Application:

```csharp
// DELETE THESE
using SmartSolutionsLab.OrangeCarRental.Customers.Domain.Customer;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Vehicle;
```

---

## Migration Strategy

### Phase 1: Add New Value Objects (Backward Compatible)
1. Create `ReservationVehicleId`, `ReservationCustomerId`, `ReservationVehicleCategory`
2. Deploy to non-production environment
3. Run tests to verify no regressions

### Phase 2: Dual Support (Temporary)
1. Add new properties alongside old ones (if needed for gradual migration)
2. Update API layer to map to new types
3. Keep old references temporarily

### Phase 3: Switch to New Types
1. Update Reservation entity to use new types
2. Update all commands/queries
3. Update handlers
4. Update database configuration
5. Deploy and test

### Phase 4: Remove Old Dependencies
1. Remove project references to Customers.Domain and Fleet.Domain
2. Remove using statements
3. Clean up any temporary dual support code
4. Final deployment

---

## Database Migration

If VehicleId or CustomerId columns need to change:

```csharp
// Usually NOT needed - GUID stays GUID in database
// Value object conversion is code-level only
// Database schema remains unchanged
```

---

## Testing Checklist

- [ ] Unit tests for new value objects pass
- [ ] Reservation entity tests pass
- [ ] Command tests pass
- [ ] Handler tests pass
- [ ] API integration tests pass
- [ ] Database entity configuration works correctly
- [ ] No compilation errors after removing references
- [ ] All 62 Reservations service tests pass

---

## Benefits of This Approach

✅ **Independent Versioning:** Customers and Fleet can change their domain models without affecting Reservations

✅ **Reduced Coupling:** Compile-time dependencies eliminated

✅ **Bounded Context Integrity:** Each context owns its own concepts

✅ **Easier Testing:** Fewer dependencies to mock

✅ **Independent Deployment:** Services can be deployed separately

✅ **Team Autonomy:** Teams can work on different services without coordination

---

## Trade-offs

⚠️ **Code Duplication:** Similar value objects exist in multiple contexts (intentional!)

⚠️ **Conversion Overhead:** API layer must convert between types (minimal performance impact)

⚠️ **Synchronization:** Category codes must stay aligned (use integration tests)

---

## Success Criteria

✅ Reservations.Application has no references to Customers.Domain or Fleet.Domain

✅ All value objects are internal to Reservations bounded context

✅ API layer handles type conversions

✅ Database schema unchanged (backward compatible)

✅ All tests pass

✅ Services can be deployed independently

---

## Estimated Timeline

- **Day 1 Morning:** Create new value objects (Steps 1-2)
- **Day 1 Afternoon:** Update Reservation entity and database config (Step 3 + 8)
- **Day 2 Morning:** Update commands, handlers, API (Steps 4-7)
- **Day 2 Afternoon:** Update tests, remove references (Steps 9-10)

**Total: 1-2 days**

---

## Related Patterns

This implementation follows the **Anti-Corruption Layer** pattern from Domain-Driven Design, which protects a bounded context from external influences by translating external concepts into internal ones.

**Further Reading:**
- Domain-Driven Design by Eric Evans (Chapter on Bounded Contexts)
- Implementing Domain-Driven Design by Vaughn Vernon (Chapter on Context Mapping)

---

## Questions or Issues?

Contact the architecture team or refer to:
- Architecture Review Document: `ARCHITECTURE-REVIEW.md`
- DDD Principles Guide: `docs/domain-driven-design.md`
