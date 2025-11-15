# Developer Onboarding Guide
## Orange Car Rental Backend

Welcome to the Orange Car Rental backend development team! This guide will help you get up to speed quickly.

---

## 🎯 Quick Start (5 Minutes)

### Prerequisites

- **.NET 9.0 SDK** - [Download](https://dotnet.microsoft.com/download/dotnet/9.0)
- **IDE:** Visual Studio 2022, Rider, or VS Code
- **Git** for version control
- **Docker Desktop** (for integration tests and local development)

### Clone and Run

```bash
# 1. Clone repository
git clone https://github.com/your-org/orange-car-rental.git
cd orange-car-rental/src/backend

# 2. Restore dependencies
dotnet restore

# 3. Build solution
dotnet build

# 4. Run tests
dotnet test --filter "FullyQualifiedName!~IntegrationTests"

# 5. Run a service
cd Services/Customers/OrangeCarRental.Customers.Api
dotnet run
# Navigate to: http://localhost:5000/swagger
```

---

## 📚 Architecture Overview

### Solution Structure

```
src/backend/
├── BuildingBlocks/              # Shared kernel (domain primitives)
│   ├── BuildingBlocks.Domain    # CQRS interfaces, base classes, value objects
│   ├── BuildingBlocks.Infrastructure
│   ├── BuildingBlocks.EventStore
│   └── BuildingBlocks.Testing
│
├── Services/                    # Microservices (bounded contexts)
│   ├── Customers/               # Customer management
│   │   ├── Domain               # Business logic, aggregates, value objects
│   │   ├── Application          # Commands, queries, handlers
│   │   ├── Infrastructure       # EF Core, repositories
│   │   ├── Api                  # REST endpoints
│   │   └── Tests                # Unit tests
│   │
│   ├── Fleet/                   # Vehicle fleet management
│   ├── Reservations/            # Booking management
│   ├── Pricing/                 # Pricing calculations
│   ├── Payments/                # Payment processing (placeholder)
│   └── Notifications/           # Email/SMS (placeholder)
│
├── Tests/                       # Integration tests
│   └── IntegrationTests         # Cross-service tests
│
└── Gateway/                     # API Gateway (YARP)
```

### Architectural Patterns

We use a combination of proven patterns:

1. **Clean Architecture** (Onion Architecture)
2. **Domain-Driven Design** (DDD)
3. **CQRS** (Command Query Responsibility Segregation)
4. **Microservices**

---

## 🏗️ Clean Architecture Layers

### Dependency Direction: Inward Only

```
    API Layer
       ↓
Infrastructure Layer
       ↓
Application Layer
       ↓
   Domain Layer  ← No dependencies
```

### Layer Responsibilities

#### 1. Domain Layer (Core Business Logic)
**Location:** `Services/{Service}/OrangeCarRental.{Service}.Domain`

**Contains:**
- Aggregates (Customer, Vehicle, Reservation, PricingPolicy)
- Value Objects (Email, PhoneNumber, Money, etc.)
- Domain Events
- Repository Interfaces
- Business Rules

**Rules:**
- ✅ No external dependencies (only BuildingBlocks.Domain)
- ✅ Pure business logic
- ❌ No references to Application, Infrastructure, or API
- ❌ No database, HTTP, or framework code

**Example:**
```csharp
// Services/Customers/Domain/Customer/Customer.cs
public sealed class Customer : AggregateRoot<CustomerIdentifier>
{
    public CustomerName Name { get; init; }
    public Email Email { get; init; }

    public Customer UpdateProfile(CustomerName name, PhoneNumber phone, Address address)
    {
        // Business logic here
        var updated = CreateMutatedCopy(name, phoneNumber: phone, address: address);
        updated.AddDomainEvent(new CustomerProfileUpdated(...));
        return updated;
    }
}
```

#### 2. Application Layer (Use Cases)
**Location:** `Services/{Service}/OrangeCarRental.{Service}.Application`

**Contains:**
- Commands & Command Handlers (write operations)
- Queries & Query Handlers (read operations)
- DTOs (Data Transfer Objects)
- Application Services (cross-context integration)

**Rules:**
- ✅ Orchestrates domain objects
- ✅ References Domain layer
- ✅ References BuildingBlocks.Domain
- ❌ No database implementation details
- ❌ No HTTP/framework code

**Example:**
```csharp
// Commands/RegisterCustomer/RegisterCustomerCommandHandler.cs
public sealed class RegisterCustomerCommandHandler(ICustomerRepository customers)
    : ICommandHandler<RegisterCustomerCommand, RegisterCustomerResult>
{
    public async Task<RegisterCustomerResult> HandleAsync(
        RegisterCustomerCommand command,
        CancellationToken cancellationToken = default)
    {
        // 1. Check uniqueness
        if (await customers.ExistsWithEmailAsync(command.Email, cancellationToken))
            throw new InvalidOperationException($"Customer with email {command.Email} already exists");

        // 2. Create aggregate (domain method)
        var customer = Customer.Register(
            command.Name,
            command.Email,
            command.PhoneNumber,
            command.DateOfBirth,
            command.Address,
            command.DriversLicense);

        // 3. Persist
        await customers.AddAsync(customer, cancellationToken);
        await customers.SaveChangesAsync(cancellationToken);

        // 4. Return result
        return new RegisterCustomerResult
        {
            CustomerIdentifier = customer.Id.Value,
            Email = customer.Email.Value,
            Status = customer.Status.ToString()
        };
    }
}
```

#### 3. Infrastructure Layer (Technical Implementation)
**Location:** `Services/{Service}/OrangeCarRental.{Service}.Infrastructure`

**Contains:**
- EF Core DbContext
- Repository Implementations
- External Service Adapters
- Database Migrations

**Rules:**
- ✅ References Application and Domain
- ✅ Implements repository interfaces from Domain
- ❌ No business logic

#### 4. API Layer (Presentation)
**Location:** `Services/{Service}/OrangeCarRental.{Service}.Api`

**Contains:**
- Minimal API endpoints
- Request/Response DTOs
- Program.cs (DI configuration)

**Rules:**
- ✅ References Application and Infrastructure
- ✅ Maps HTTP requests to commands/queries
- ❌ No business logic

---

## 🎨 CQRS Pattern

### Commands (Write Operations)

Commands modify state:

```csharp
// 1. Define command (record)
public sealed record RegisterCustomerCommand(
    CustomerName Name,
    Email Email,
    PhoneNumber PhoneNumber,
    DateOnly DateOfBirth,
    Address Address,
    DriversLicense DriversLicense
) : ICommand<RegisterCustomerResult>;

// 2. Define result
public sealed record RegisterCustomerResult
{
    public required Guid CustomerIdentifier { get; init; }
    public required string Email { get; init; }
    public required string Status { get; init; }
}

// 3. Create handler
public sealed class RegisterCustomerCommandHandler(ICustomerRepository customers)
    : ICommandHandler<RegisterCustomerCommand, RegisterCustomerResult>
{
    public async Task<RegisterCustomerResult> HandleAsync(
        RegisterCustomerCommand command,
        CancellationToken cancellationToken = default)
    {
        // Orchestrate domain logic
        var customer = Customer.Register(...);
        await customers.AddAsync(customer, cancellationToken);
        await customers.SaveChangesAsync(cancellationToken);
        return new RegisterCustomerResult { ... };
    }
}

// 4. Register in DI
builder.Services.AddScoped<RegisterCustomerCommandHandler>();

// 5. Use in endpoint
customers.MapPost("/", async (
    RegisterCustomerRequest request,
    RegisterCustomerCommandHandler handler,  // Direct injection
    CancellationToken cancellationToken) =>
{
    var command = new RegisterCustomerCommand(...);
    var result = await handler.HandleAsync(command, cancellationToken);
    return Results.Created($"/api/customers/{result.CustomerIdentifier}", result);
});
```

### Queries (Read Operations)

Queries return data (no state changes):

```csharp
// 1. Define query
public sealed record GetCustomerQuery(CustomerIdentifier Id) : IQuery<CustomerDto>;

// 2. Create handler
public sealed class GetCustomerQueryHandler(ICustomerRepository customers)
    : IQueryHandler<GetCustomerQuery, CustomerDto>
{
    public async Task<CustomerDto> HandleAsync(
        GetCustomerQuery query,
        CancellationToken cancellationToken = default)
    {
        var customer = await customers.GetByIdAsync(query.Id, cancellationToken);
        if (customer is null)
            throw new EntityNotFoundException($"Customer {query.Id} not found");

        return customer.ToDto();  // Map to DTO
    }
}

// 3. Use in endpoint
customers.MapGet("/{id:guid}", async (
    Guid id,
    GetCustomerQueryHandler handler,
    CancellationToken cancellationToken) =>
{
    var query = new GetCustomerQuery(CustomerIdentifier.From(id));
    var customer = await handler.HandleAsync(query, cancellationToken);
    return Results.Ok(customer);
});
```

**Key Rules:**
- ✅ Commands can change state, Queries cannot
- ✅ Commands return operation results, Queries return DTOs
- ✅ Never call `SaveChangesAsync()` in a query handler
- ✅ Queries return read-only data

---

## 🎯 Domain-Driven Design

### Aggregates

Aggregates are consistency boundaries:

```csharp
public sealed class Customer : AggregateRoot<CustomerIdentifier>
{
    // Invariants (rules that must always be true)
    private const int MinimumAgeYears = 18;
    private const int MinimumLicenseValidityDays = 30;

    // State (init-only properties)
    public CustomerName Name { get; init; }
    public Email Email { get; init; }
    public PhoneNumber PhoneNumber { get; init; }
    public CustomerStatus Status { get; init; }

    // Factory method (enforces invariants)
    public static Customer Register(CustomerName name, Email email, ...)
    {
        // Validate business rules
        if (age < MinimumAgeYears)
            throw new ArgumentException($"Customer must be at least {MinimumAgeYears}");

        // Create instance
        var customer = new Customer
        {
            Id = CustomerIdentifier.New(),
            Name = name,
            Email = email,
            Status = CustomerStatus.Active,
            CreatedAtUtc = DateTime.UtcNow
        };

        // Raise domain event
        customer.AddDomainEvent(new CustomerRegistered(customer.Id.Value, email.Value));

        return customer;
    }

    // Domain methods (business operations)
    public Customer UpdateProfile(CustomerName name, PhoneNumber phone, Address address)
    {
        var updated = CreateMutatedCopy(name, phoneNumber: phone, address: address);
        updated.AddDomainEvent(new CustomerProfileUpdated(Id.Value));
        return updated;  // Returns new instance (immutable pattern)
    }

    public bool CanMakeReservation()
    {
        if (Status != CustomerStatus.Active) return false;
        if (!DriversLicense.IsValid()) return false;
        if (DriversLicense.DaysUntilExpiry() < MinimumLicenseValidityDays) return false;
        return true;
    }
}
```

**Aggregate Design Rules:**
- ✅ Small, focused aggregates
- ✅ Enforce invariants
- ✅ One aggregate root per aggregate
- ✅ Reference other aggregates by ID only
- ✅ Raise domain events for state changes

### Value Objects

Value objects are immutable, validated data:

```csharp
public readonly record struct Email(string Value)
{
    public static Email Of(string value)
    {
        // Normalize
        var normalized = value?.Trim().ToLowerInvariant() ?? string.Empty;

        // Validate
        Ensure.That(normalized, nameof(value))
            .IsNotNullOrWhiteSpace()
            .AndHasMaxLength(254)
            .AndIsValidEmail();

        return new Email(normalized);
    }

    // GDPR anonymization
    public static Email Anonymized() => Of($"anonymized-{Guid.NewGuid()}@gdpr-deleted.local");
}
```

**Value Object Rules:**
- ✅ Immutable (readonly record struct)
- ✅ Equality by value
- ✅ No identity
- ✅ Validation in factory method
- ✅ Business logic encapsulated

---

## 🧪 Testing

### Test Organization

```
OrangeCarRental.{Service}.Tests/
├── Domain/
│   ├── ValueObjects/       # Test value object validation
│   │   ├── EmailTests.cs
│   │   └── PhoneNumberTests.cs
│   └── Entities/           # Test aggregate business rules
│       └── CustomerTests.cs
└── Application/
    ├── Commands/           # Test command handlers
    └── Queries/            # Test query handlers
```

### Writing Tests (AAA Pattern)

```csharp
public class CustomerTests
{
    [Fact]
    public void Register_WithCustomerUnder18_ShouldThrowArgumentException()
    {
        // Arrange
        var tooYoung = BirthDate.Of(DateOnly.FromDateTime(DateTime.Now.AddYears(-17)));
        var name = CustomerName.Of("John", "Doe");
        var email = Email.Of("john.doe@example.com");
        // ... other test data

        // Act
        var ex = Should.Throw<ArgumentException>(() =>
            Customer.Register(name, email, phoneNumber, tooYoung, address, license));

        // Assert
        ex.Message.ShouldContain("18");
    }
}
```

**Test Guidelines:**
- ✅ Use AAA pattern (Arrange-Act-Assert)
- ✅ Test business rules, not implementation
- ✅ One assertion per test (generally)
- ✅ Descriptive test names: `MethodName_Scenario_ExpectedResult`
- ✅ Use Shouldly for fluent assertions
- ✅ Use Moq for mocking dependencies

### Running Tests

```bash
# All unit tests (fast)
dotnet test --filter "FullyQualifiedName!~IntegrationTests"

# Specific service
dotnet test Services/Customers/OrangeCarRental.Customers.Tests

# With coverage
dotnet test --collect:"XPlat Code Coverage"

# Watch mode (auto-run on file changes)
dotnet watch test --project Services/Customers/OrangeCarRental.Customers.Tests
```

---

## 🔧 Development Workflow

### 1. Pick a Task
- Check project board / Jira
- Assign task to yourself
- Create feature branch: `feature/add-customer-validation`

### 2. Implement Feature (TDD Approach)

#### Step 1: Write Failing Test

```csharp
[Fact]
public void UpdateProfile_WithInvalidPhone_ShouldThrowException()
{
    // Arrange
    var customer = CreateValidCustomer();
    var invalidPhone = PhoneNumber.Of("+1234567890");  // Not German format

    // Act & Assert
    Should.Throw<ArgumentException>(() =>
        customer.UpdateProfile(customer.Name, invalidPhone, customer.Address));
}
```

#### Step 2: Implement Domain Logic

```csharp
public Customer UpdateProfile(CustomerName name, PhoneNumber phone, Address address)
{
    // Business rule: Phone must be German
    if (!phone.Value.StartsWith("+49"))
        throw new ArgumentException("Phone must be German format (+49)");

    var updated = CreateMutatedCopy(name, phoneNumber: phone, address: address);
    updated.AddDomainEvent(new CustomerProfileUpdated(Id.Value));
    return updated;
}
```

#### Step 3: Create Command/Handler

```csharp
public sealed record UpdateCustomerProfileCommand(
    CustomerIdentifier CustomerId,
    CustomerName Name,
    PhoneNumber PhoneNumber,
    Address Address
) : ICommand<UpdateCustomerProfileResult>;

public sealed class UpdateCustomerProfileCommandHandler(ICustomerRepository customers)
    : ICommandHandler<UpdateCustomerProfileCommand, UpdateCustomerProfileResult>
{
    public async Task<UpdateCustomerProfileResult> HandleAsync(...)
    {
        var customer = await customers.GetByIdAsync(command.CustomerId, ...);
        var updated = customer.UpdateProfile(command.Name, command.PhoneNumber, command.Address);
        await customers.UpdateAsync(updated, ...);
        await customers.SaveChangesAsync(...);
        return new UpdateCustomerProfileResult { ... };
    }
}
```

#### Step 4: Add API Endpoint

```csharp
customers.MapPut("/{id:guid}/profile", async (
    Guid id,
    UpdateCustomerProfileRequest request,
    UpdateCustomerProfileCommandHandler handler,
    CancellationToken cancellationToken) =>
{
    var command = new UpdateCustomerProfileCommand(...);
    var result = await handler.HandleAsync(command, cancellationToken);
    return Results.Ok(result);
});
```

#### Step 5: Test Endpoint

```bash
curl -X PUT http://localhost:5000/api/customers/123/profile \
  -H "Content-Type: application/json" \
  -d '{
    "firstName": "John",
    "lastName": "Doe",
    "phoneNumber": "+4915123456789",
    ...
  }'
```

### 3. Commit Changes

```bash
git add .
git commit -m "feat(customers): add phone number validation to profile update"
git push origin feature/add-customer-validation
```

### 4. Create Pull Request
- Include tests
- Update documentation if needed
- Request code review

---

## 🎓 German Market Compliance

Our system is designed for the German car rental market:

### Customer Requirements
- **Minimum Age:** 18 years
- **Phone Format:** +49 (German prefix required)
- **Salutations:** Herr, Frau, Divers (German gender options)
- **Driver's License:** Minimum 30 days validity

### Pricing
- **VAT:** 19% (German standard rate)
- **Currency:** EUR
- **Price Display:** Net + VAT = Gross

### Vehicle Categories
German rental categories:
- Kleinwagen (Small)
- Kompaktklasse (Compact)
- Mittelklasse (Mid-size)
- Oberklasse (Premium)
- SUV
- Kombi (Estate)
- Transporter (Van)
- Luxus (Luxury)

### Locations
Major German cities:
- Berlin Hauptbahnhof
- München Flughafen
- Frankfurt Flughafen
- Hamburg Hauptbahnhof
- Köln Hauptbahnhof

---

## 🚀 Common Tasks

### Add New Command

1. Create command record in `Application/Commands/{Feature}/`
2. Create command handler
3. Create result record
4. Register handler in DI
5. Add API endpoint
6. Write tests

### Add New Query

1. Create query record in `Application/Queries/{Feature}/`
2. Create query handler
3. Create DTO
4. Register handler in DI
5. Add API endpoint
6. Write tests

### Add New Value Object

1. Create in `Domain/` (appropriate namespace)
2. Make it immutable (readonly record struct)
3. Add validation in factory method
4. Write unit tests

### Add Database Migration

```bash
cd Services/Customers/OrangeCarRental.Customers.Infrastructure

# Add migration
dotnet ef migrations add AddCustomerPreferences

# Update database
dotnet ef database update

# Review migration
# Check Migrations/ folder
```

---

## 📖 Further Reading

### Architecture
- [Clean Architecture Review](ARCHITECTURE-REVIEW.md)
- [ADR-001: Immutable Aggregates](docs/ADR-001-IMMUTABLE-AGGREGATES.md)
- [ADR-002: No MediatR](docs/ADR-002-NO-MEDIATR.md)

### Implementation Guides
- [Fleet-Reservations Decoupling](IMPLEMENTATION-GUIDE-FLEET-RESERVATIONS-DECOUPLING.md)
- [Reservations Cross-Domain Dependencies](IMPLEMENTATION-GUIDE-RESERVATIONS-CROSS-DOMAIN-DEPENDENCIES.md)

### Testing
- [Testing Guide](TESTING-GUIDE.md)
- [Test Summary](TEST-SUMMARY.md)

---

## 🆘 Getting Help

- **Architecture Questions:** Review ADRs in `docs/` folder
- **Code Examples:** Check existing commands/queries in other services
- **Testing:** See `TESTING-GUIDE.md`
- **Git Issues:** Ask team lead

---

## ✅ Checklist for New Developers

### Week 1
- [ ] Clone repository and run solution
- [ ] Run all tests successfully
- [ ] Read this onboarding guide
- [ ] Review Clean Architecture layers
- [ ] Understand CQRS pattern
- [ ] Read ADR-001 and ADR-002

### Week 2
- [ ] Implement a simple command (guided)
- [ ] Implement a simple query (guided)
- [ ] Write unit tests for your code
- [ ] Create a pull request
- [ ] Code review with team

### Week 3
- [ ] Implement a feature independently
- [ ] Add API endpoint
- [ ] Write integration test
- [ ] Review German market compliance rules

### Month 1
- [ ] Understand all 4 microservices
- [ ] Know when to use commands vs queries
- [ ] Understand aggregate design
- [ ] Contribute to architecture discussions

---

**Welcome to the team! Happy coding!** 🎉
