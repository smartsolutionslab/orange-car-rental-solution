# Testing Guide - Orange Car Rental

## Overview

This document provides a comprehensive guide to the test suite for the Orange Car Rental solution. The test suite includes **275 test methods** across **22 test classes** covering all four microservices.

## Test Coverage Summary

### Total Test Coverage

| Service | Domain Tests | Application Tests | Total Tests | Test Files |
|---------|-------------|-------------------|-------------|------------|
| **Customers** | 66 | 0 (included in domain) | 66 | 6 |
| **Fleet** | 82 | 15 | 97 | 8 |
| **Reservations** | 55 | 7 | 62 | 4 |
| **Pricing** | 40 | 10 | 50 | 4 |
| **TOTAL** | **243** | **32** | **275** | **22** |

## Running Tests

### Prerequisites

- .NET 10.0 SDK
- Docker (for integration tests with Testcontainers - if implemented)

### Run All Tests

```bash
# From solution root
dotnet test

# Run tests for specific service
dotnet test Services/Customers/OrangeCarRental.Customers.Tests
dotnet test Services/Fleet/OrangeCarRental.Fleet.Tests
dotnet test Services/Reservations/OrangeCarRental.Reservations.Tests
dotnet test Services/Pricing/OrangeCarRental.Pricing.Tests
```

### Run Tests with Coverage

```bash
# Install coverage tool
dotnet tool install --global dotnet-coverage

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"

# Generate coverage report
dotnet coverage report coverage.cobertura.xml --output-format html
```

## Customer Service Tests (66 tests)

### Domain Layer - Value Objects (55 tests)

#### EmailTests.cs (14 tests)
- ✅ Valid email formats (RFC 5322 compliance)
- ✅ Case normalization (lowercase)
- ✅ Length validation (max 254 characters)
- ✅ Invalid format rejection (missing @, multiple @, etc.)
- ✅ GDPR anonymization (`anonymized-{guid}@gdpr-deleted.local`)

#### PhoneNumberTests.cs (17 tests)
- ✅ German phone format validation (+49 prefix required)
- ✅ Domestic format conversion (0151 → +491512345678)
- ✅ International format normalization (removes spaces, dashes, parentheses)
- ✅ Double-zero prefix conversion (0049 → +49)
- ✅ Length validation (10-16 digits after +49)
- ✅ Non-German number rejection (+44, +1, +33, etc.)
- ✅ Formatted display output (`+49 151 2345678`)
- ✅ GDPR anonymization (+490000000000)

#### CustomerNameTests.cs (11 tests)
- ✅ First and last name validation
- ✅ German salutation support (Herr, Frau, Divers)
- ✅ Full name generation (`Max Mustermann`)
- ✅ Formal name with salutation (`Herr Max Mustermann`)
- ✅ Anonymization support (`[DELETED-{guid}]`)
- ✅ Value object equality

### Domain Layer - Entities (11 tests)

#### CustomerTests.cs (14 tests)
- ✅ Customer registration with complete profile
- ✅ Minimum age validation (18+ years)
- ✅ Driver's license expiry validation (30+ days remaining)
- ✅ Profile updates with immutability pattern
- ✅ Domain event emission (CustomerRegistered, CustomerProfileUpdated)
- ✅ Age calculation
- ✅ Full name and formal name properties

### Application Layer (0 tests included in domain)

#### RegisterCustomerCommandHandlerTests.cs (7 tests)
- ✅ Valid customer registration
- ✅ Email uniqueness validation
- ✅ Age validation (under 18 rejection)
- ✅ Expired license rejection
- ✅ Repository interaction verification
- ✅ SaveChanges verification
- ✅ Cancellation token handling

#### GetCustomerQueryHandlerTests.cs (3 tests)
- ✅ Retrieve existing customer
- ✅ EntityNotFoundException for non-existent customer
- ✅ Cancellation token handling

## Fleet Service Tests (97 tests)

### Domain Layer - Value Objects (53 tests)

#### VehicleNameTests.cs (10 tests)
- ✅ Valid name validation
- ✅ Whitespace trimming
- ✅ Max length validation (100 characters)
- ✅ Empty/null rejection

#### ManufacturerTests.cs (10 tests)
- ✅ German manufacturer validation (BMW, Volkswagen, Mercedes-Benz, Audi, Porsche, Opel)
- ✅ Whitespace trimming
- ✅ Max length validation (100 characters)

#### SeatingCapacityTests.cs (11 tests)
- ✅ Valid range validation (2-9 seats)
- ✅ Comparison operators (<, <=, >, >=)
- ✅ Edge case validation (exactly 2, exactly 9)

#### VehicleCategoryTests.cs (11 tests)
- ✅ German category validation (Kleinwagen, Kompaktklasse, Mittelklasse, Oberklasse, SUV, Kombi, Transporter, Luxus)
- ✅ Case-insensitive lookup
- ✅ Category code mapping
- ✅ All categories collection

#### LocationTests.cs (11 tests)
- ✅ Predefined German locations (Berlin Hauptbahnhof, München Flughafen, Frankfurt Flughafen, Hamburg Hauptbahnhof, Köln Hauptbahnhof)
- ✅ Location code lookup
- ✅ Custom location creation
- ✅ Address details validation

### Domain Layer - Entities (29 tests)

#### VehicleTests.cs (29 tests)
- ✅ Vehicle creation with all required fields
- ✅ Daily rate updates with immutability
- ✅ Location changes with business rules (cannot move rented vehicle)
- ✅ Status transitions (Available → Rented → Available)
- ✅ Maintenance status with validation (cannot put rented vehicle under maintenance)
- ✅ License plate setting with normalization (uppercase)
- ✅ Optional details (manufacturer, model, year, imageUrl)
- ✅ Domain event emission (VehicleAddedToFleet, VehicleDailyRateChanged, VehicleLocationChanged, VehicleStatusChanged)
- ✅ Availability checking

### Application Layer - Commands (8 tests)

#### AddVehicleToFleetCommandHandlerTests.cs (8 tests)
- ✅ Valid vehicle creation
- ✅ Optional details handling (manufacturer, model, year, imageUrl, licensePlate)
- ✅ All vehicle categories (8 categories)
- ✅ All fuel types (6 types)
- ✅ Repository interaction verification
- ✅ Cancellation token handling

### Application Layer - Queries (7 tests)

#### SearchVehiclesQueryHandlerTests.cs (7 tests)
- ✅ Pagination support
- ✅ Empty results handling
- ✅ Location filtering
- ✅ Category filtering
- ✅ Multiple filter combinations
- ✅ Cancellation token handling

## Reservations Service Tests (62 tests)

### Domain Layer - Value Objects (32 tests)

#### BookingPeriodTests.cs (19 tests)
- ✅ Valid date range validation
- ✅ Past date rejection
- ✅ Return before pickup rejection
- ✅ Max rental period (90 days)
- ✅ Total days calculation
- ✅ Overlap detection
- ✅ DateTime to DateOnly conversion

#### LocationCodeTests.cs (13 tests)
- ✅ Valid code validation (3-20 characters)
- ✅ Uppercase normalization
- ✅ Whitespace trimming
- ✅ German location codes (BER-HBF, MUC-FLG, FRA-FLG, HAM-HBF, CGN-HBF)

### Domain Layer - Entities (23 tests)

#### ReservationTests.cs (23 tests)
- ✅ Reservation creation
- ✅ Status lifecycle (Pending → Confirmed → Active → Completed)
- ✅ Cancellation with business rules
- ✅ No-show marking
- ✅ Pickup date validation (must be today or past)
- ✅ Completed reservation protection (cannot cancel)
- ✅ Domain event emission (ReservationCreated, ReservationConfirmed, ReservationCancelled, ReservationCompleted)
- ✅ Period overlap checking

### Application Layer - Commands (7 tests)

#### CreateReservationCommandHandlerTests.cs (7 tests)
- ✅ Valid reservation creation
- ✅ Price calculation via pricing service
- ✅ Provided price bypass (backward compatibility)
- ✅ Different pickup/dropoff locations
- ✅ Repository and pricing service interaction
- ✅ Cancellation token handling

## Pricing Service Tests (50 tests)

### Domain Layer - Value Objects (21 tests)

#### RentalPeriodTests.cs (11 tests)
- ✅ Valid date range validation
- ✅ Past date rejection
- ✅ Total days calculation
- ✅ DateTime to DateOnly conversion

#### CategoryCodeTests.cs (10 tests)
- ✅ Valid category codes (KLEIN, KOMPAKT, MITTEL, OBER, SUV, KOMBI, TRANS, LUXUS)
- ✅ Uppercase normalization
- ✅ Max length validation (20 characters)

### Domain Layer - Entities (19 tests)

#### PricingPolicyTests.cs (19 tests)
- ✅ Policy creation with effective dates
- ✅ Daily rate updates with immutability
- ✅ Policy deactivation
- ✅ Validity checking (date range and active status)
- ✅ Price calculation (daily rate × total days)
- ✅ Location-specific pricing support
- ✅ Domain event emission (PricingPolicyCreated, PricingPolicyUpdated, PricingPolicyDeactivated)

### Application Layer - Queries (10 tests)

#### CalculatePriceQueryHandlerTests.cs (10 tests)
- ✅ Price calculation for different categories
- ✅ Location-specific pricing with fallback
- ✅ VAT calculation (19% German rate)
- ✅ Multi-day rental calculations
- ✅ No active policy error handling
- ✅ Cancellation token handling

## Test Patterns and Best Practices

### AAA Pattern (Arrange-Act-Assert)

All tests follow the AAA pattern for clarity:

```csharp
[Fact]
public async Task HandleAsync_WithValidCommand_ShouldCreateReservation()
{
    // Arrange
    var command = CreateValidCommand();
    _pricingServiceMock.Setup(...).ReturnsAsync(...);

    // Act
    var result = await _handler.HandleAsync(command, CancellationToken.None);

    // Assert
    result.ShouldNotBeNull();
    result.Status.ShouldBe("Pending");
    _repositoryMock.Verify(..., Times.Once);
}
```

### Mocking with Moq

Repository and service dependencies are mocked:

```csharp
private readonly Mock<IVehicleRepository> _vehicleRepositoryMock;
private readonly Mock<IPricingService> _pricingServiceMock;

_vehicleRepositoryMock
    .Setup(x => x.AddAsync(It.IsAny<Vehicle>(), It.IsAny<CancellationToken>()))
    .Returns(Task.CompletedTask);
```

### Fluent Assertions with Shouldly

All assertions use Shouldly for readability:

```csharp
result.ShouldNotBeNull();
result.Id.ShouldNotBe(Guid.Empty);
result.Status.ShouldBe(VehicleStatus.Available);
customer.Age.ShouldBeGreaterThanOrEqualTo(18);
```

### Test Data Builders

Helper methods create valid test data:

```csharp
private static AddVehicleToFleetCommand CreateValidCommand()
{
    return new AddVehicleToFleetCommand(
        VehicleName.Of("BMW X5"),
        VehicleCategory.SUV,
        Location.BerlinHauptbahnhof,
        Money.Of(89.99m, Currency.EUR),
        SeatingCapacity.Of(5),
        FuelType.Diesel,
        TransmissionType.Automatic,
        null, null, null, null, null
    );
}
```

## German Market Validations

### Customer Service
- ✅ Minimum age: 18 years
- ✅ German phone format: +49 prefix required
- ✅ Driver's license: minimum 30 days validity
- ✅ Salutations: Herr, Frau, Divers
- ✅ German postal codes and cities

### Fleet Service
- ✅ German vehicle categories (Kleinwagen to Luxusklasse)
- ✅ German rental locations (Berlin, München, Frankfurt, Hamburg, Köln)
- ✅ Seating capacity: 2-9 (typical for German rental market)
- ✅ Fuel types including Hydrogen (German green energy initiative)

### Reservations Service
- ✅ Maximum rental period: 90 days
- ✅ German location codes
- ✅ Booking period validation

### Pricing Service
- ✅ VAT rate: 19% (German standard rate)
- ✅ Net/Gross price calculations
- ✅ Location-specific pricing for German cities

## Domain Events Tested

All domain events are verified in tests:

### Customer Service
- `CustomerRegistered`
- `CustomerProfileUpdated`

### Fleet Service
- `VehicleAddedToFleet`
- `VehicleDailyRateChanged`
- `VehicleLocationChanged`
- `VehicleStatusChanged`

### Reservations Service
- `ReservationCreated`
- `ReservationConfirmed`
- `ReservationCancelled`
- `ReservationCompleted`

### Pricing Service
- `PricingPolicyCreated`
- `PricingPolicyUpdated`
- `PricingPolicyDeactivated`

## Test Execution Recommendations

### Local Development

```bash
# Run tests before committing
dotnet test --logger "console;verbosity=detailed"

# Run specific test class
dotnet test --filter "FullyQualifiedName~VehicleTests"

# Run specific test method
dotnet test --filter "FullyQualifiedName~VehicleTests.From_WithValidData_ShouldCreateVehicle"
```

### CI/CD Pipeline

```bash
# Run all tests with coverage in CI
dotnet test --collect:"XPlat Code Coverage" --results-directory ./coverage

# Generate coverage report
dotnet coverage report ./coverage/coverage.cobertura.xml --output-format cobertura
```

### Watch Mode (Development)

```bash
# Auto-run tests on file changes
dotnet watch test --project Services/Fleet/OrangeCarRental.Fleet.Tests
```

## Coverage Goals

Target coverage metrics:

- **Domain Layer**: 90%+ (business logic critical)
- **Application Layer**: 85%+ (handler orchestration)
- **Infrastructure Layer**: 70%+ (integration points)
- **API Layer**: 60%+ (endpoint mapping)

Current domain and application layer coverage: **~85%** (estimated based on test count)

## Future Test Improvements

### Recommended Additions

1. **Integration Tests**
   - Database integration with Testcontainers
   - Message broker integration (if event bus added)
   - External API integration (pricing service calls)

2. **Performance Tests**
   - Search query performance
   - Concurrent reservation creation
   - Pricing calculation under load

3. **End-to-End Tests**
   - Complete reservation flow
   - Customer registration → vehicle search → booking → payment
   - Multi-service orchestration

4. **Contract Tests**
   - API contract verification
   - Service-to-service communication
   - DTO validation

## Troubleshooting

### Common Issues

**Issue**: Test fails with `NullReferenceException`
**Solution**: Ensure all mocks are properly set up in test constructor

**Issue**: `EntityNotFoundException` in tests
**Solution**: Verify repository mock returns expected data

**Issue**: Date-based tests fail intermittently
**Solution**: Use fixed dates instead of `DateTime.Now` in assertions

**Issue**: Async test hangs
**Solution**: Ensure all async operations use `await` and `CancellationToken`

## Test Maintenance

### Adding New Tests

1. Create test class in appropriate folder (`Domain/`, `Application/`)
2. Follow AAA pattern
3. Use descriptive test names (`MethodName_Scenario_ExpectedResult`)
4. Add test data builders for complex objects
5. Verify domain events if applicable
6. Test edge cases and error conditions

### Updating Existing Tests

1. Run affected tests after domain changes
2. Update test data if value object constraints change
3. Add new test cases for new business rules
4. Maintain backward compatibility tests

## Summary

This comprehensive test suite provides:
- ✅ **275 tests** covering critical business logic
- ✅ **Domain-driven design** validation
- ✅ **CQRS pattern** verification
- ✅ **German market compliance** testing
- ✅ **Immutability pattern** enforcement
- ✅ **Domain events** validation
- ✅ **Clean Architecture** layer isolation

All tests are maintained and updated as part of the development workflow to ensure high-quality, reliable code.
