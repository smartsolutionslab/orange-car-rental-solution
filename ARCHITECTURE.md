# Car Rental System - Architecture & Design Document

**Version:** 1.0
**Date:** 2025-10-28
**Status:** Architecture Design Phase

---

## Executive Summary

This document defines the architecture for a modern, cloud-ready Car Rental Software System built with Domain-Driven Design principles, Clean Architecture, and a clear separation between public and call center interfaces.

**Target Market:** German market with full GDPR/DSGVO compliance and German language support

**Key Characteristics:**
- Domain-Driven Design with strict value object usage
- Clean Architecture with layered separation (Domain, Application, Infrastructure, API)
- Multi-location/branch support from the start
- Two separate Angular frontends (Public Portal + Call Center)
- Deployable to Azure, Kubernetes, or on-premise
- Full CI/CD automation with GitHub Actions
- German market compliance (GDPR, VAT, German language, legal requirements)

**See also:** [German Market Requirements](./GERMAN_MARKET_REQUIREMENTS.md) for detailed compliance and localization requirements

---

## System Overview

### Business Domains

The system supports car rental operations across multiple locations with two distinct user interfaces:

1. **Public Portal** - Customer-facing web application
2. **Call Center Portal** - Internal agent interface for managing bookings and customers

### Core Capabilities

- Vehicle search and availability checking
- Reservation management with pricing calculation
- Customer registration and profile management
- Multi-location/station operations
- Booking history and tracking
- Similar vehicle suggestions
- Comprehensive call center operations

---

## Bounded Contexts

Based on DDD principles and the user stories, the system is divided into the following bounded contexts:

### 1. Fleet Management Context

**Responsibility:** Manage vehicles, categories, and stations/locations

**Aggregates:**
- `Vehicle` - Individual vehicle with specifications
- `Station` - Rental location/branch
- `VehicleCategory` - Classification of vehicles

**Key Domain Events:**
- `VehicleAddedToFleet`
- `VehicleRetiredFromFleet`
- `VehicleMaintenanceScheduled`
- `VehicleMovedToStation`
- `StationCreated`
- `StationCapacityChanged`

### 2. Reservations Context

**Responsibility:** Handle availability, bookings, and reservation lifecycle

**Aggregates:**
- `Reservation` - Core booking aggregate
- `AvailabilityCalendar` - Tracks vehicle availability

**Key Domain Events:**
- `ReservationRequested`
- `ReservationConfirmed`
- `ReservationCancelled`
- `ReservationModified`
- `VehiclePickedUp`
- `VehicleReturned`

**Future Enhancement:** Event Sourcing planned for full audit trail and temporal queries.

### 3. Customer Context

**Responsibility:** Customer profiles, registration, and identity

**Aggregates:**
- `Customer` - Customer profile and details

**Key Domain Events:**
- `CustomerRegistered`
- `CustomerProfileUpdated`
- `CustomerVerified`

**Future Enhancement:** Event Sourcing planned for complete customer history and compliance.

### 4. Pricing Context

**Responsibility:** Pricing rules, calculations, and promotions

**Aggregates:**
- `PricingPolicy` - Pricing rules per category/location
- `PriceCalculation` - Immutable price computation

**Key Domain Events:**
- `PricingPolicyUpdated`
- `PriceCalculated`
- `DiscountApplied`

### 5. Payments Context

**Responsibility:** Payment processing and transactions

**Aggregates:**
- `Payment` - Payment transaction

**Key Domain Events:**
- `PaymentInitiated`
- `PaymentCompleted`
- `PaymentFailed`
- `RefundIssued`

### 6. Notifications Context

**Responsibility:** Multi-channel notifications (Email, In-app, Push)

**Key Domain Events (Subscribed):**
- Listens to events from other contexts
- Sends appropriate notifications

---

## Domain Model Design

### Value Objects (Strict Usage)

**CRITICAL RULE:** No primitive types (string, int, DateTime, decimal) are allowed directly in domain models. Everything must be a value object.

#### Common Value Objects

```csharp
// Identity Value Objects
public sealed record VehicleId(Guid Value);
public sealed record StationId(Guid Value);
public sealed record ReservationId(Guid Value);
public sealed record CustomerId(Guid Value);
public sealed record CategoryId(Guid Value);

// Temporal Value Objects
public sealed record ReservationPeriod(StartDate Start, EndDate End)
{
    public int TotalDays => (End.Value - Start.Value).Days;
}
public sealed record StartDate(DateTime Value);
public sealed record EndDate(DateTime Value);

// Money Value Object (with VAT support for German market)
public sealed record Money(decimal NetAmount, decimal VatAmount, Currency Currency)
{
    public decimal GrossAmount => NetAmount + VatAmount;
    public decimal VatRate => NetAmount > 0 ? VatAmount / NetAmount : 0;

    public static Money operator +(Money a, Money b)
    {
        if (a.Currency != b.Currency)
            throw new InvalidOperationException("Cannot add different currencies");
        return new Money(a.NetAmount + b.NetAmount, a.VatAmount + b.VatAmount, a.Currency);
    }

    public static Money operator *(Money money, int multiplier)
        => new Money(money.NetAmount * multiplier, money.VatAmount * multiplier, money.Currency);

    // Factory methods
    public static Money CreateWithVat(decimal netAmount, decimal vatRate, Currency currency)
    {
        var vatAmount = Math.Round(netAmount * vatRate, 2);
        return new Money(netAmount, vatAmount, currency);
    }

    public static Money CreateGross(decimal grossAmount, decimal vatRate, Currency currency)
    {
        var netAmount = Math.Round(grossAmount / (1 + vatRate), 2);
        var vatAmount = grossAmount - netAmount;
        return new Money(netAmount, vatAmount, currency);
    }

    // German market convenience
    public static Money Euro(decimal netAmount) => CreateWithVat(netAmount, 0.19m, new Currency("EUR"));
    public static Money EuroGross(decimal grossAmount) => CreateGross(grossAmount, 0.19m, new Currency("EUR"));
}

public sealed record Currency(string Code) // EUR, USD, etc.
{
    public static readonly Currency EUR = new("EUR");
    public static readonly Currency USD = new("USD");
}

// Personal Information Value Objects
public sealed record PersonName(Title Title, FirstName FirstName, LastName LastName);
public sealed record Title(string Value); // Mr, Ms, Mrs, Dr, etc.
public sealed record FirstName(string Value);
public sealed record LastName(string Value);
public sealed record DateOfBirth(DateTime Value);
public sealed record EmailAddress(string Value);

// Address Value Object
public sealed record Address(
    Street Street,
    HouseNumber HouseNumber,
    PostalCode PostalCode,
    City City,
    Country Country,
    District District);

public sealed record Street(string Value);
public sealed record HouseNumber(string Value);
public sealed record PostalCode(string Value);
public sealed record City(string Value);
public sealed record Country(string Value);
public sealed record District(string Value);

// Vehicle Specifications Value Objects
public sealed record VehicleName(string Value);
public sealed record SeatCount(int Value);
public sealed record FuelType(string Value); // Petrol, Diesel, Electric, Hybrid
public sealed record TransmissionType(string Value); // Manual, Automatic
public sealed record DailyRate(Money Amount);

// Station Value Objects
public sealed record StationName(string Value);
public sealed record AvailableVehicleCount(int Value);

// Contact Information
public sealed record PhoneNumber(string Value);
```

### Example Aggregate: Vehicle

```csharp
public sealed class Vehicle : AggregateRoot<VehicleId>
{
    public VehicleName Name { get; private set; }
    public CategoryId Category { get; private set; }
    public SeatCount Seats { get; private set; }
    public FuelType FuelType { get; private set; }
    public TransmissionType Transmission { get; private set; }
    public StationId CurrentStation { get; private set; }
    public VehicleStatus Status { get; private set; }

    private Vehicle() { } // For EF Core

    public static Vehicle Create(
        VehicleId id,
        VehicleName name,
        CategoryId category,
        SeatCount seats,
        FuelType fuelType,
        TransmissionType transmission,
        StationId station)
    {
        var vehicle = new Vehicle
        {
            Id = id,
            Name = name,
            Category = category,
            Seats = seats,
            FuelType = fuelType,
            Transmission = transmission,
            CurrentStation = station,
            Status = VehicleStatus.Available
        };

        vehicle.AddDomainEvent(new VehicleAddedToFleet(id, name, category, station));
        return vehicle;
    }

    public void MoveToStation(StationId newStation)
    {
        if (CurrentStation == newStation)
            return;

        CurrentStation = newStation;
        AddDomainEvent(new VehicleMovedToStation(Id, newStation));
    }
}

public sealed record VehicleStatus
{
    public static readonly VehicleStatus Available = new("Available");
    public static readonly VehicleStatus Rented = new("Rented");
    public static readonly VehicleStatus Maintenance = new("Maintenance");
    public static readonly VehicleStatus Retired = new("Retired");

    public string Value { get; }
    private VehicleStatus(string value) => Value = value;
}
```

### Example Aggregate: Reservation

```csharp
public sealed class Reservation : AggregateRoot<ReservationId>
{
    public VehicleId Vehicle { get; private set; }
    public CustomerId Customer { get; private set; }
    public StationId PickupStation { get; private set; }
    public StationId ReturnStation { get; private set; }
    public ReservationPeriod Period { get; private set; }
    public RenterInformation Renter { get; private set; }
    public Money TotalPrice { get; private set; }
    public ReservationStatus Status { get; private set; }

    private Reservation() { }

    public static Reservation Create(
        ReservationId id,
        VehicleId vehicle,
        CustomerId customer,
        StationId pickupStation,
        StationId returnStation,
        ReservationPeriod period,
        RenterInformation renter,
        Money totalPrice)
    {
        var reservation = new Reservation
        {
            Id = id,
            Vehicle = vehicle,
            Customer = customer,
            PickupStation = pickupStation,
            ReturnStation = returnStation,
            Period = period,
            Renter = renter,
            TotalPrice = totalPrice,
            Status = ReservationStatus.Pending
        };

        reservation.AddDomainEvent(new ReservationRequested(
            id, vehicle, customer, pickupStation, returnStation, period, totalPrice));

        return reservation;
    }

    public void Confirm()
    {
        if (Status != ReservationStatus.Pending)
            throw new InvalidOperationException("Can only confirm pending reservations");

        Status = ReservationStatus.Confirmed;
        AddDomainEvent(new ReservationConfirmed(Id));
    }

    public void Cancel(CancellationReason reason)
    {
        if (Status == ReservationStatus.Cancelled)
            return;

        Status = ReservationStatus.Cancelled;
        AddDomainEvent(new ReservationCancelled(Id, reason));
    }
}

public sealed record RenterInformation(
    PersonName Name,
    DateOfBirth DateOfBirth,
    Address Address);

public sealed record ReservationStatus
{
    public static readonly ReservationStatus Pending = new("Pending");
    public static readonly ReservationStatus Confirmed = new("Confirmed");
    public static readonly ReservationStatus Active = new("Active");
    public static readonly ReservationStatus Completed = new("Completed");
    public static readonly ReservationStatus Cancelled = new("Cancelled");

    public string Value { get; }
    private ReservationStatus(string value) => Value = value;
}

public sealed record CancellationReason(string Value);
```

---

## Application Architecture

### Command Handlers (Write Operations)

**Purpose:** Handle commands, enforce business rules, persist changes

**Flow:**
1. Command received via Minimal API endpoint
2. Command validated
3. Entity loaded from database via Entity Framework Core
4. Business logic executed in domain layer
5. Changes persisted to SQL Server
6. Response returned to client

**Command Examples:**
```csharp
public sealed record CreateReservationCommand(
    VehicleId VehicleId,
    CustomerId CustomerId,
    StationId PickupStation,
    StationId ReturnStation,
    ReservationPeriod Period,
    RenterInformation Renter);

public sealed record ConfirmReservationCommand(ReservationId ReservationId);
public sealed record CancelReservationCommand(ReservationId ReservationId, CancellationReason Reason);
```

### Query Handlers (Read Operations)

**Purpose:** Optimized queries for read operations

**Read Models (SQL Server Tables):**

```sql
-- Vehicle Search View
CREATE TABLE VehicleSearchView (
    VehicleId UNIQUEIDENTIFIER,
    VehicleName NVARCHAR(200),
    CategoryId UNIQUEIDENTIFIER,
    CategoryName NVARCHAR(100),
    SeatCount INT,
    FuelType NVARCHAR(50),
    TransmissionType NVARCHAR(50),
    CurrentStationId UNIQUEIDENTIFIER,
    CurrentStationName NVARCHAR(200),
    DailyRate DECIMAL(18,2),
    Currency NVARCHAR(3),
    Status NVARCHAR(50)
);

-- Availability View
CREATE TABLE VehicleAvailabilityView (
    VehicleId UNIQUEIDENTIFIER,
    StationId UNIQUEIDENTIFIER,
    Date DATE,
    IsAvailable BIT
);

-- Reservation List View
CREATE TABLE ReservationListView (
    ReservationId UNIQUEIDENTIFIER,
    VehicleName NVARCHAR(200),
    VehicleCategoryName NVARCHAR(100),
    DailyRate DECIMAL(18,2),
    PickupStationName NVARCHAR(200),
    PickupStationAddress NVARCHAR(500),
    ReturnStationName NVARCHAR(200),
    StartDate DATE,
    EndDate DATE,
    RenterFullName NVARCHAR(300),
    RenterDateOfBirth DATE,
    RenterAddress NVARCHAR(500),
    TotalPrice DECIMAL(18,2),
    Currency NVARCHAR(3),
    Status NVARCHAR(50),
    CreatedAt DATETIME2
);

-- Customer Booking History View
CREATE TABLE CustomerBookingHistoryView (
    CustomerId UNIQUEIDENTIFIER,
    ReservationId UNIQUEIDENTIFIER,
    VehicleName NVARCHAR(200),
    PickupStationName NVARCHAR(200),
    ReturnStationName NVARCHAR(200),
    StartDate DATE,
    EndDate DATE,
    TotalPrice DECIMAL(18,2),
    Currency NVARCHAR(3),
    Status NVARCHAR(50)
);

-- Station Overview View
CREATE TABLE StationOverviewView (
    StationId UNIQUEIDENTIFIER,
    StationName NVARCHAR(200),
    Address NVARCHAR(500),
    AvailableVehicleCount INT
);

-- Customer List View
CREATE TABLE CustomerListView (
    CustomerId UNIQUEIDENTIFIER,
    FullName NVARCHAR(300),
    Address NVARCHAR(500),
    Email NVARCHAR(200),
    TotalBookings INT
);
```

**Query Examples:**
```csharp
public sealed record SearchVehiclesQuery(
    ReservationPeriod Period,
    StationId? Station,
    CategoryId? Category);

public sealed record GetReservationListQuery(
    StationId? StationFilter,
    DateOnly? DateFilter,
    string? CustomerSearchTerm);

public sealed record GetCustomerBookingHistoryQuery(CustomerId CustomerId);
```

### Event Projections

Event handlers update read models:

```csharp
public sealed class ReservationProjectionHandler :
    IEventHandler<ReservationConfirmed>,
    IEventHandler<ReservationCancelled>
{
    private readonly IReadModelRepository _repository;

    public async Task Handle(ReservationConfirmed @event)
    {
        // Update ReservationListView
        // Update CustomerBookingHistoryView
        // Update VehicleAvailabilityView
    }

    public async Task Handle(ReservationCancelled @event)
    {
        // Update status in read models
        // Restore vehicle availability
    }
}
```

---

## Technical Stack

### Backend (.NET)

| Component | Technology | Version | Purpose |
|-----------|-----------|---------|---------|
| Framework | .NET | 10.0 | Backend platform |
| API Style | Minimal API | .NET 10 | RESTful endpoints |
| Orchestration | .NET Aspire | 9.1 | Local dev & orchestration |
| Database | SQL Server | 2022 | Data persistence |
| Authentication | Keycloak | Latest | Identity & SSO (OpenID Connect) |
| ORM | Entity Framework Core | 10.0 | Data access |
| Testing | xUnit + FluentAssertions | Latest | Unit & integration tests |
| API Docs | Scalar (OpenAPI) | Latest | API documentation |

**Key Libraries:**
- FluentValidation - Command validation
- Polly - Resilience policies
- Serilog - Structured logging
- MassTransit - Message bus (in-memory for local, Azure Service Bus for production)

### Frontend (Angular)

| Component | Technology | Version | Purpose |
|-----------|-----------|---------|---------|
| Framework | Angular | 21 | Frontend framework |
| Monorepo | Nx | Latest | Workspace management |
| Styling | Tailwind CSS | 3.x | Utility-first CSS |
| UI Components | Custom + Headless UI | - | Shared component library |
| State Management | NgRx Signal Store | Latest | State management |
| HTTP Client | Angular HttpClient | - | API communication |
| Forms | Reactive Forms | - | Type-safe forms |
| Testing | Jest + Testing Library | Latest | Unit & component tests |
| E2E Testing | Playwright | Latest | End-to-end tests |

### Infrastructure

| Component | Technology | Purpose |
|-----------|-----------|---------|
| CI/CD | GitHub Actions | Pipeline automation |
| Container Registry | GitHub Container Registry | Docker image storage |
| Cloud Platform | Azure / On-Premise | Deployment target |
| Container Orchestration | Azure Container Apps / Kubernetes | Production deployment |
| Message Bus | Azure Service Bus / RabbitMQ | Event distribution |
| Email Service | Azure Communication Services | Email notifications |
| Push Notifications | Azure Notification Hubs / Web Push | Push notifications |

---

## Project Structure

### Repository Organization

```
orange-car-rental/
├── .github/
│   └── workflows/              # GitHub Actions CI/CD
│       ├── backend-ci.yml
│       ├── frontend-ci.yml
│       └── deploy-azure.yml
├── src/
│   ├── backend/
│   │   ├── OrangeCarRental.sln
│   │   ├── Directory.Build.props
│   │   ├── Directory.Packages.props
│   │   │
│   │   ├── BuildingBlocks/     # Shared infrastructure
│   │   │   ├── OrangeCarRental.BuildingBlocks.Domain/
│   │   │   │   ├── AggregateRoot.cs
│   │   │   │   ├── IDomainEvent.cs
│   │   │   │   ├── ValueObject.cs
│   │   │   │   └── IRepository.cs
│   │   │   │
│   │   │   ├── OrangeCarRental.BuildingBlocks.Infrastructure/
│   │   │   │   ├── Messaging/
│   │   │   │   ├── Authentication/
│   │   │   │   └── Logging/
│   │   │   │
│   │   │   └── OrangeCarRental.BuildingBlocks.Testing/
│   │   │       ├── TestBase.cs
│   │   │       └── Fixtures/
│   │   │
│   │   ├── Services/           # Bounded Context Services
│   │   │   │
│   │   │   ├── Fleet/
│   │   │   │   ├── OrangeCarRental.Fleet.Domain/
│   │   │   │   │   ├── Vehicles/
│   │   │   │   │   │   ├── Vehicle.cs (Aggregate)
│   │   │   │   │   │   ├── VehicleId.cs (Value Object)
│   │   │   │   │   │   ├── VehicleName.cs
│   │   │   │   │   │   ├── SeatCount.cs
│   │   │   │   │   │   └── Events/
│   │   │   │   │   │       └── VehicleAddedToFleet.cs
│   │   │   │   │   ├── Stations/
│   │   │   │   │   │   ├── Station.cs (Aggregate)
│   │   │   │   │   │   └── Events/
│   │   │   │   │   └── Categories/
│   │   │   │   │
│   │   │   │   ├── OrangeCarRental.Fleet.Application/
│   │   │   │   │   ├── Commands/
│   │   │   │   │   │   ├── AddVehicleCommand.cs
│   │   │   │   │   │   └── AddVehicleCommandHandler.cs
│   │   │   │   │   ├── Queries/
│   │   │   │   │   │   ├── SearchVehiclesQuery.cs
│   │   │   │   │   │   └── SearchVehiclesQueryHandler.cs
│   │   │   │   │   └── Projections/
│   │   │   │   │       └── VehicleProjectionHandler.cs
│   │   │   │   │
│   │   │   │   ├── OrangeCarRental.Fleet.Infrastructure/
│   │   │   │   │   ├── Repositories/
│   │   │   │   │   │   └── VehicleRepository.cs
│   │   │   │   │   ├── Persistence/
│   │   │   │   │   │   ├── FleetReadDbContext.cs
│   │   │   │   │   │   └── Configurations/
│   │   │   │   │   └── EventHandlers/
│   │   │   │   │
│   │   │   │   ├── OrangeCarRental.Fleet.Api/
│   │   │   │   │   ├── Program.cs
│   │   │   │   │   ├── Endpoints/
│   │   │   │   │   │   ├── VehicleEndpoints.cs
│   │   │   │   │   │   └── StationEndpoints.cs
│   │   │   │   │   └── appsettings.json
│   │   │   │   │
│   │   │   │   └── OrangeCarRental.Fleet.Tests/
│   │   │   │       ├── Unit/
│   │   │   │       ├── Integration/
│   │   │   │       └── Acceptance/
│   │   │   │
│   │   │   ├── Reservations/
│   │   │   │   ├── OrangeCarRental.Reservations.Domain/
│   │   │   │   ├── OrangeCarRental.Reservations.Application/
│   │   │   │   ├── OrangeCarRental.Reservations.Infrastructure/
│   │   │   │   ├── OrangeCarRental.Reservations.Api/
│   │   │   │   └── OrangeCarRental.Reservations.Tests/
│   │   │   │
│   │   │   ├── Customers/
│   │   │   │   ├── OrangeCarRental.Customers.Domain/
│   │   │   │   ├── OrangeCarRental.Customers.Application/
│   │   │   │   ├── OrangeCarRental.Customers.Infrastructure/
│   │   │   │   ├── OrangeCarRental.Customers.Api/
│   │   │   │   └── OrangeCarRental.Customers.Tests/
│   │   │   │
│   │   │   ├── Pricing/
│   │   │   │   └── (same structure)
│   │   │   │
│   │   │   ├── Payments/
│   │   │   │   └── (same structure)
│   │   │   │
│   │   │   └── Notifications/
│   │   │       └── (same structure)
│   │   │
│   │   ├── ApiGateway/
│   │   │   └── OrangeCarRental.ApiGateway/    # YARP or Ocelot
│   │   │
│   │   └── AppHost/
│   │       └── OrangeCarRental.AppHost/        # .NET Aspire orchestration
│   │
│   └── frontend/
│       ├── apps/
│       │   ├── public-portal/
│       │   │   ├── src/
│       │   │   │   ├── app/
│       │   │   │   │   ├── features/
│       │   │   │   │   │   ├── vehicle-search/
│       │   │   │   │   │   ├── booking/
│       │   │   │   │   │   ├── customer-profile/
│       │   │   │   │   │   └── booking-history/
│       │   │   │   │   ├── core/
│       │   │   │   │   │   ├── auth/
│       │   │   │   │   │   ├── services/
│       │   │   │   │   │   └── guards/
│       │   │   │   │   └── shared/
│       │   │   │   │       └── (app-specific components)
│       │   │   │   └── assets/
│       │   │   ├── project.json
│       │   │   └── tailwind.config.js
│       │   │
│       │   └── call-center-portal/
│       │       ├── src/
│       │       │   ├── app/
│       │       │   │   ├── features/
│       │       │   │   │   ├── dashboard/
│       │       │   │   │   ├── booking-management/
│       │       │   │   │   ├── customer-management/
│       │       │   │   │   └── station-overview/
│       │       │   │   ├── core/
│       │       │   │   └── shared/
│       │       │   └── assets/
│       │       ├── project.json
│       │       └── tailwind.config.js
│       │
│       ├── libs/
│       │   ├── shared-ui/              # Shared UI component library
│       │   │   ├── src/
│       │   │   │   ├── lib/
│       │   │   │   │   ├── components/
│       │   │   │   │   │   ├── button/
│       │   │   │   │   │   ├── input/
│       │   │   │   │   │   ├── card/
│       │   │   │   │   │   ├── modal/
│       │   │   │   │   │   ├── date-picker/
│       │   │   │   │   │   ├── vehicle-card/
│       │   │   │   │   │   └── booking-card/
│       │   │   │   │   └── index.ts
│       │   │   └── project.json
│       │   │
│       │   ├── data-access/            # API clients & state management
│       │   │   ├── src/
│       │   │   │   ├── lib/
│       │   │   │   │   ├── services/
│       │   │   │   │   │   ├── vehicle.service.ts
│       │   │   │   │   │   ├── reservation.service.ts
│       │   │   │   │   │   └── customer.service.ts
│       │   │   │   │   ├── models/
│       │   │   │   │   └── stores/
│       │   │   └── project.json
│       │   │
│       │   └── util/                   # Utility functions
│       │       └── src/lib/
│       │           ├── date-utils.ts
│       │           ├── format-utils.ts
│       │           └── validation.ts
│       │
│       ├── nx.json
│       ├── package.json
│       ├── tsconfig.base.json
│       └── tailwind.config.base.js     # Shared Tailwind config
│
├── docs/
│   ├── architecture/
│   ├── api/
│   └── user-guides/
│
├── scripts/
│   ├── local-pipeline.sh              # Run CI/CD locally
│   └── setup-dev-environment.sh
│
├── .editorconfig
├── .gitignore
├── README.md
└── ARCHITECTURE.md                    # This document
```

---

## Development Guidelines

### 1. Coding Conventions

#### C# Backend Standards

**Naming Conventions:**
- Classes: `PascalCase`
- Interfaces: `IPascalCase`
- Methods: `PascalCase`
- Parameters/Variables: `camelCase`
- Private fields: `_camelCase`
- Constants: `PascalCase`

**File Organization:**
- One class per file
- File name matches class name
- Value objects in separate files
- Events grouped in `/Events` folder

**Code Style:**
```csharp
// ✅ GOOD: Value object usage
public sealed record CreateVehicleCommand(
    VehicleName Name,
    CategoryId Category,
    SeatCount Seats,
    FuelType FuelType,
    TransmissionType Transmission);

// ❌ BAD: Primitive obsession
public sealed record CreateVehicleCommand(
    string Name,
    Guid CategoryId,
    int Seats,
    string FuelType,
    string Transmission);
```

**Immutability:**
- Use `sealed record` for value objects
- Use `private set` for aggregate properties
- No public setters on domain models

**Domain Event Naming:**
- Past tense: `ReservationConfirmed`, not `ConfirmReservation`
- Describes what happened, not what to do

#### Angular/TypeScript Standards

**Naming Conventions:**
- Components: `kebab-case` files, `PascalCase` classes
- Services: `camelCase.service.ts`
- Models: `PascalCase` interfaces

**Component Structure:**
```typescript
// ✅ GOOD: Smart/Dumb component pattern
@Component({
  selector: 'app-vehicle-search',
  standalone: true,
  imports: [VehicleCardComponent, DateRangePickerComponent],
  template: `...`
})
export class VehicleSearchComponent {
  private vehicleService = inject(VehicleService);

  vehicles = signal<Vehicle[]>([]);

  async search(criteria: SearchCriteria): Promise<void> {
    this.vehicles.set(await this.vehicleService.search(criteria));
  }
}
```

**Tailwind Usage:**
```html
<!-- ✅ GOOD: Utility classes, extract to component for reuse -->
<button class="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700
               transition-colors duration-200 font-medium">
  Book Now
</button>

<!-- ❌ BAD: Custom CSS when Tailwind provides it -->
<button class="custom-button">Book Now</button>
```

### 2. Value Object Guidelines

**Creation:**
- Always validate in constructor
- Throw descriptive exceptions for invalid values
- Implement equality by value

```csharp
public sealed record EmailAddress
{
    public string Value { get; }

    public EmailAddress(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Email cannot be empty", nameof(value));

        if (!IsValidEmail(value))
            throw new ArgumentException($"Invalid email format: {value}", nameof(value));

        Value = value.ToLowerInvariant();
    }

    private static bool IsValidEmail(string email)
        => email.Contains('@') && email.Contains('.');
}
```

### 3. Testing Strategy

#### Unit Tests (Domain Layer)

```csharp
public class ReservationTests
{
    [Fact]
    public void Create_ShouldEmitReservationRequestedEvent()
    {
        // Arrange
        var reservationId = new ReservationId(Guid.NewGuid());
        var vehicleId = new VehicleId(Guid.NewGuid());
        // ... setup value objects

        // Act
        var reservation = Reservation.Create(
            reservationId, vehicleId, customerId,
            pickupStation, returnStation, period, renter, price);

        // Assert
        var events = reservation.GetUncommittedEvents();
        events.Should().ContainSingle()
            .Which.Should().BeOfType<ReservationRequested>();
    }

    [Fact]
    public void Confirm_WhenAlreadyConfirmed_ShouldThrow()
    {
        // Arrange
        var reservation = CreateConfirmedReservation();

        // Act
        var act = () => reservation.Confirm();

        // Assert
        act.Should().Throw<InvalidOperationException>();
    }
}
```

#### Integration Tests (API + Database)

```csharp
public class ReservationApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    [Fact]
    public async Task CreateReservation_ShouldReturn201_AndStoreEvents()
    {
        // Arrange
        var client = _factory.CreateClient();
        var command = new CreateReservationCommand(...);

        // Act
        var response = await client.PostAsJsonAsync("/api/reservations", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        // Verify events in event store
        var eventStore = _factory.Services.GetRequiredService<IEventStore>();
        var events = await eventStore.GetEventsAsync(reservationId);
        events.Should().ContainSingle(e => e is ReservationRequested);
    }
}
```

#### E2E Tests (Playwright)

```typescript
test('user can search and book a vehicle', async ({ page }) => {
  await page.goto('/');

  // Search
  await page.fill('[data-testid="pickup-date"]', '2025-11-01');
  await page.fill('[data-testid="return-date"]', '2025-11-05');
  await page.selectOption('[data-testid="station-select"]', 'station-1');
  await page.click('[data-testid="search-button"]');

  // Select vehicle
  await expect(page.locator('[data-testid="vehicle-card"]')).toHaveCount(5);
  await page.click('[data-testid="book-button"]').first();

  // Fill booking form
  await page.fill('[data-testid="first-name"]', 'John');
  await page.fill('[data-testid="last-name"]', 'Doe');
  // ... fill other fields

  await page.click('[data-testid="confirm-booking"]');

  // Verify confirmation
  await expect(page.locator('[data-testid="confirmation-message"]'))
    .toContainText('Booking confirmed');
});
```

### 4. Git Workflow

**Branch Strategy:**
```
main (production)
└── develop (integration)
    ├── feature/US-1-vehicle-search
    ├── feature/US-2-booking-flow
    └── feature/US-3-user-registration
```

**Branch Naming:**
- Feature: `feature/US-{number}-{short-description}`
- Bugfix: `bugfix/{issue-number}-{description}`
- Hotfix: `hotfix/{description}`

**Commit Message Format:**
```
<type>(<scope>): <subject>

<body>

<footer>
```

**Types:** `feat`, `fix`, `docs`, `style`, `refactor`, `test`, `chore`

**Example:**
```
feat(reservations): implement vehicle booking flow

- Add CreateReservationCommand and handler
- Implement price calculation logic
- Add booking form validation
- Update read models with reservation projections

Implements: US-2
```

**Pull Request Process:**
1. Create feature branch from `develop`
2. Implement user story with tests
3. Run local pipeline: `./scripts/local-pipeline.sh`
4. Create PR with description and user story reference
5. Require 1+ approvals
6. Squash merge to `develop`

### 5. API Design Guidelines

**Minimal API Endpoint Pattern:**
```csharp
public static class VehicleEndpoints
{
    public static RouteGroupBuilder MapVehicleEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("/", SearchVehicles)
            .WithName("SearchVehicles")
            .WithOpenApi()
            .Produces<SearchVehiclesResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest);

        group.MapPost("/", AddVehicle)
            .WithName("AddVehicle")
            .RequireAuthorization("FleetManager")
            .WithOpenApi();

        return group;
    }

    private static async Task<IResult> SearchVehicles(
        [AsParameters] SearchVehiclesQuery query,
        IQueryHandler<SearchVehiclesQuery, SearchVehiclesResponse> handler)
    {
        var result = await handler.HandleAsync(query);
        return Results.Ok(result);
    }

    private static async Task<IResult> AddVehicle(
        AddVehicleCommand command,
        ICommandHandler<AddVehicleCommand> handler)
    {
        await handler.HandleAsync(command);
        return Results.Created($"/api/vehicles/{command.VehicleId}", null);
    }
}
```

**REST Conventions:**
- `GET /api/vehicles` - Search vehicles
- `POST /api/vehicles` - Add vehicle
- `GET /api/vehicles/{id}` - Get vehicle details
- `PUT /api/vehicles/{id}` - Update vehicle
- `DELETE /api/vehicles/{id}` - Remove vehicle

- `GET /api/reservations` - List reservations
- `POST /api/reservations` - Create reservation
- `POST /api/reservations/{id}/confirm` - Confirm reservation
- `POST /api/reservations/{id}/cancel` - Cancel reservation

### 6. Error Handling

**Domain Exceptions:**
```csharp
public sealed class ReservationException : Exception
{
    public ReservationException(string message) : base(message) { }
}

public sealed class VehicleNotAvailableException : ReservationException
{
    public VehicleNotAvailableException(VehicleId vehicleId, ReservationPeriod period)
        : base($"Vehicle {vehicleId.Value} is not available for period {period}") { }
}
```

**API Error Responses (RFC 7807 Problem Details):**
```csharp
app.UseExceptionHandler(builder =>
{
    builder.Run(async context =>
    {
        var exceptionHandler = context.Features.Get<IExceptionHandlerFeature>();
        var exception = exceptionHandler?.Error;

        var problemDetails = exception switch
        {
            ValidationException ve => new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Validation Error",
                Detail = ve.Message
            },
            VehicleNotAvailableException vnae => new ProblemDetails
            {
                Status = StatusCodes.Status409Conflict,
                Title = "Vehicle Not Available",
                Detail = vnae.Message
            },
            _ => new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Internal Server Error"
            }
        };

        context.Response.StatusCode = problemDetails.Status ?? 500;
        await context.Response.WriteAsJsonAsync(problemDetails);
    });
});
```

---

## CI/CD Pipeline Strategy

### GitHub Actions Workflows

#### 1. Backend CI Pipeline

**File:** `.github/workflows/backend-ci.yml`

```yaml
name: Backend CI

on:
  push:
    branches: [main, develop]
    paths:
      - 'src/backend/**'
  pull_request:
    branches: [main, develop]
    paths:
      - 'src/backend/**'

jobs:
  build-and-test:
    runs-on: ubuntu-latest

    steps:
    - uses: actions/checkout@v4

    - name: Setup .NET
      uses: actions/setup-dotnet@v4
      with:
        dotnet-version: '9.0.x'

    - name: Restore dependencies
      run: dotnet restore src/backend/OrangeCarRental.sln

    - name: Build
      run: dotnet build src/backend/OrangeCarRental.sln --no-restore --configuration Release

    - name: Run unit tests
      run: dotnet test src/backend/OrangeCarRental.sln --no-build --configuration Release
           --filter "Category=Unit" --logger "trx;LogFileName=unit-tests.trx"

    - name: Run integration tests
      run: dotnet test src/backend/OrangeCarRental.sln --no-build --configuration Release
           --filter "Category=Integration" --logger "trx;LogFileName=integration-tests.trx"

    - name: Code coverage
      run: dotnet test src/backend/OrangeCarRental.sln --no-build --configuration Release
           --collect:"XPlat Code Coverage"

    - name: Upload coverage to Codecov
      uses: codecov/codecov-action@v4
      with:
        files: '**/coverage.cobertura.xml'

    - name: Build Docker images
      if: github.event_name == 'push'
      run: |
        docker build -f src/backend/Services/Fleet/OrangeCarRental.Fleet.Api/Dockerfile
                     -t ghcr.io/${{ github.repository }}/fleet-api:${{ github.sha }} .
        docker build -f src/backend/Services/Reservations/OrangeCarRental.Reservations.Api/Dockerfile
                     -t ghcr.io/${{ github.repository }}/reservations-api:${{ github.sha }} .

    - name: Push Docker images
      if: github.event_name == 'push' && github.ref == 'refs/heads/main'
      run: |
        echo ${{ secrets.GITHUB_TOKEN }} | docker login ghcr.io -u ${{ github.actor }} --password-stdin
        docker push ghcr.io/${{ github.repository }}/fleet-api:${{ github.sha }}
        docker push ghcr.io/${{ github.repository }}/reservations-api:${{ github.sha }}
```

#### 2. Frontend CI Pipeline

**File:** `.github/workflows/frontend-ci.yml`

```yaml
name: Frontend CI

on:
  push:
    branches: [main, develop]
    paths:
      - 'src/frontend/**'
  pull_request:
    branches: [main, develop]
    paths:
      - 'src/frontend/**'

jobs:
  build-and-test:
    runs-on: ubuntu-latest

    steps:
    - uses: actions/checkout@v4
      with:
        fetch-depth: 0  # For Nx affected commands

    - name: Setup Node.js
      uses: actions/setup-node@v4
      with:
        node-version: '20'
        cache: 'npm'
        cache-dependency-path: src/frontend/package-lock.json

    - name: Install dependencies
      working-directory: src/frontend
      run: npm ci

    - name: Lint
      working-directory: src/frontend
      run: npx nx affected --target=lint --base=origin/main

    - name: Test
      working-directory: src/frontend
      run: npx nx affected --target=test --base=origin/main --code-coverage

    - name: Build
      working-directory: src/frontend
      run: npx nx affected --target=build --base=origin/main --configuration=production

    - name: E2E tests
      working-directory: src/frontend
      run: npx nx affected --target=e2e --base=origin/main

    - name: Upload coverage
      uses: codecov/codecov-action@v4
      with:
        directory: src/frontend/coverage

    - name: Build Docker images
      if: github.event_name == 'push'
      run: |
        docker build -f src/frontend/apps/public-portal/Dockerfile
                     -t ghcr.io/${{ github.repository }}/public-portal:${{ github.sha }} .
        docker build -f src/frontend/apps/call-center-portal/Dockerfile
                     -t ghcr.io/${{ github.repository }}/call-center-portal:${{ github.sha }} .
```

#### 3. Local Pipeline Execution

**File:** `scripts/local-pipeline.sh`

```bash
#!/bin/bash
set -e

echo "🚀 Running local CI/CD pipeline..."

# Backend
echo "📦 Building backend..."
cd src/backend
dotnet restore
dotnet build --no-restore --configuration Release

echo "🧪 Running backend tests..."
dotnet test --no-build --configuration Release --filter "Category=Unit"
dotnet test --no-build --configuration Release --filter "Category=Integration"

cd ../..

# Frontend
echo "📦 Building frontend..."
cd src/frontend
npm ci
npx nx run-many --target=lint --all
npx nx run-many --target=test --all --code-coverage
npx nx run-many --target=build --all --configuration=production

cd ../..

echo "✅ Local pipeline completed successfully!"
```

### Deployment Strategy

#### Azure Deployment

**Option 1: Azure Container Apps (Recommended)**
- Managed container hosting
- Auto-scaling
- Integrated with Azure services
- Simpler than AKS

**Option 2: Azure Kubernetes Service (AKS)**
- Full Kubernetes control
- More complex, more flexibility
- Better for large scale

**Infrastructure Components:**
- Azure Container Apps / AKS - Application hosting
- Azure SQL Database - Event store & read models
- Azure Service Bus - Event distribution
- Azure Key Vault - Secrets management
- Azure Application Insights - Monitoring & logging
- Azure Container Registry - Private image registry (if not using GitHub)
- Azure Front Door - CDN & global routing

#### On-Premise Deployment

**Docker Compose for small deployments:**
```yaml
services:
  fleet-api:
    image: orange-car-rental/fleet-api:latest
    environment:
      - ConnectionStrings__EventStore=...
      - ConnectionStrings__ReadDb=...

  reservations-api:
    image: orange-car-rental/reservations-api:latest

  public-portal:
    image: orange-car-rental/public-portal:latest

  call-center-portal:
    image: orange-car-rental/call-center-portal:latest

  sqlserver:
    image: mcr.microsoft.com/mssql/server:2022-latest

  keycloak:
    image: quay.io/keycloak/keycloak:latest

  rabbitmq:
    image: rabbitmq:3-management
```

**Kubernetes for larger deployments:**
- Helm charts for deployment
- Ingress for routing
- Persistent volumes for databases
- Secrets management

---

## .NET Aspire Integration

### AppHost Configuration

```csharp
var builder = DistributedApplication.CreateBuilder(args);

// Infrastructure
var sqlserver = builder.AddSqlServer("sqlserver")
    .WithDataVolume()
    .AddDatabase("eventstore")
    .AddDatabase("readdb");

var keycloak = builder.AddKeycloak("keycloak", 8080)
    .WithDataVolume();

var rabbitmq = builder.AddRabbitMQ("messaging")
    .WithManagementPlugin();

// Backend Services
var fleetApi = builder.AddProject<Fleet_Api>("fleet-api")
    .WithReference(sqlserver)
    .WithReference(rabbitmq);

var reservationsApi = builder.AddProject<Reservations_Api>("reservations-api")
    .WithReference(sqlserver)
    .WithReference(rabbitmq);

var customersApi = builder.AddProject<Customers_Api>("customers-api")
    .WithReference(sqlserver)
    .WithReference(rabbitmq);

var pricingApi = builder.AddProject<Pricing_Api>("pricing-api")
    .WithReference(sqlserver)
    .WithReference(rabbitmq);

var paymentsApi = builder.AddProject<Payments_Api>("payments-api")
    .WithReference(sqlserver)
    .WithReference(rabbitmq);

var notificationsApi = builder.AddProject<Notifications_Api>("notifications-api")
    .WithReference(sqlserver)
    .WithReference(rabbitmq);

// API Gateway
var gateway = builder.AddProject<ApiGateway>("api-gateway")
    .WithReference(fleetApi)
    .WithReference(reservationsApi)
    .WithReference(customersApi)
    .WithReference(pricingApi)
    .WithReference(paymentsApi)
    .WithReference(notificationsApi);

// Frontend
builder.AddNpmApp("public-portal", "../frontend", "start:public")
    .WithReference(gateway)
    .WithHttpEndpoint(port: 4200, env: "PORT")
    .WithExternalHttpEndpoints();

builder.AddNpmApp("call-center-portal", "../frontend", "start:callcenter")
    .WithReference(gateway)
    .WithHttpEndpoint(port: 4201, env: "PORT")
    .WithExternalHttpEndpoints();

builder.Build().Run();
```

**Benefits:**
- Single command to start entire system: `dotnet run --project src/backend/AppHost`
- Automatic service discovery
- Dashboard for monitoring all services
- Easy local development
- Service-to-service communication configuration

---

## Security Considerations

### Authentication & Authorization

**Keycloak Setup:**
- Realm: `orange-car-rental`
- Clients:
  - `public-portal` (Public, PKCE flow)
  - `call-center-portal` (Confidential, Authorization Code flow)
  - `backend-services` (Service accounts)

**Roles:**
- `customer` - Public portal users
- `call-center-agent` - Call center staff
- `fleet-manager` - Vehicle/station management
- `administrator` - Full system access

**JWT Token Validation:**
```csharp
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = "https://keycloak.local/realms/orange-car-rental";
        options.Audience = "backend-services";
        options.RequireHttpsMetadata = true;
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Customer", policy =>
        policy.RequireRole("customer"));

    options.AddPolicy("CallCenterAgent", policy =>
        policy.RequireRole("call-center-agent"));

    options.AddPolicy("FleetManager", policy =>
        policy.RequireRole("fleet-manager"));
});
```

### Data Protection

- Encrypt sensitive data at rest (PII in read models)
- Use HTTPS everywhere
- Secure connection strings in Azure Key Vault / environment variables
- GDPR compliance: Right to erasure (anonymize in event store)

---

## Monitoring & Observability

### Logging

**Serilog Configuration:**
```csharp
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "OrangeCarRental")
    .Enrich.WithProperty("Service", "Fleet.Api")
    .WriteTo.Console()
    .WriteTo.ApplicationInsights(TelemetryConfiguration.Active, TelemetryConverter.Traces)
    .CreateLogger();
```

### Metrics

**Key Metrics to Track:**
- Request rate, duration, errors (per endpoint)
- Event store write latency
- Projection lag (time between event and read model update)
- Queue depths (message bus)
- Database connection pool usage

### Tracing

- Use OpenTelemetry for distributed tracing
- Correlation ID propagation across services
- Track request flow: API → Command Handler → Aggregate → Event Store → Event Bus → Projection

---

## Performance Optimization

### Event Store Optimization

- **Snapshots:** Create snapshots every 50-100 events to speed up aggregate loading
- **Indexing:** Index on `AggregateId`, `Timestamp`, `EventType`
- **Partitioning:** Consider partitioning by date for large event volumes

### Read Model Optimization

- **Indexes:** Create indexes on frequently queried columns
- **Caching:** Use Redis for hot data (vehicle availability, pricing)
- **Denormalization:** Fully denormalize read models (no joins)

### API Performance

- **Response Caching:** Cache vehicle search results
- **Compression:** Enable gzip compression
- **Pagination:** Paginate large result sets
- **Rate Limiting:** Protect against abuse

---

## User Story to Feature Mapping

### Public Portal

| User Story | Feature | Bounded Context | Priority |
|------------|---------|----------------|----------|
| US-1 | Vehicle search with filters | Fleet, Pricing | High |
| US-2 | Booking flow (quick + search) | Reservations, Customers | High |
| US-3 | User registration | Customers | High |
| US-4 | Booking history | Reservations | Medium |
| US-5 | Pre-fill renter data | Customers, Reservations | Medium |
| US-6 | Similar vehicle suggestions | Fleet | Low |

### Call Center Portal

| User Story | Feature | Bounded Context | Priority |
|------------|---------|----------------|----------|
| US-7 | List all bookings | Reservations | High |
| US-8 | Filter/group bookings | Reservations | High |
| US-9 | Search bookings by customer | Reservations, Customers | High |
| US-10 | Dashboard with vehicle search | Fleet | High |
| US-11 | Station overview | Fleet | Medium |
| US-12 | Customer view with bookings | Customers, Reservations | Medium |

---

## Implementation Roadmap

### Phase 1: Foundation (Weeks 1-2)
- [ ] Repository setup
- [ ] Project structure creation
- [ ] BuildingBlocks libraries (Domain, EventStore, Infrastructure)
- [ ] .NET Aspire AppHost setup
- [ ] Keycloak integration
- [ ] CI/CD pipeline setup
- [ ] Nx workspace setup
- [ ] Shared UI component library foundation

### Phase 2: Fleet Management (Weeks 3-4)
- [ ] Vehicle aggregate & value objects
- [ ] Station aggregate
- [ ] Category aggregate
- [ ] Fleet API endpoints
- [ ] Vehicle search projections
- [ ] Basic admin UI for vehicle management

### Phase 3: Customer Management (Week 5)
- [ ] Customer aggregate
- [ ] Registration flow
- [ ] Keycloak integration
- [ ] Customer profile UI
- [ ] Authentication guards

### Phase 4: Reservations Core (Weeks 6-8)
- [ ] Reservation aggregate
- [ ] Availability checking logic
- [ ] CreateReservation command flow
- [ ] Reservation projections
- [ ] Public portal vehicle search UI (US-1)
- [ ] Public portal booking flow UI (US-2, US-5)

### Phase 5: Pricing (Week 9)
- [ ] Pricing policy aggregate
- [ ] Price calculation logic
- [ ] Pricing projections
- [ ] Integration with reservation flow

### Phase 6: Call Center Portal (Weeks 10-11)
- [ ] Call center authentication
- [ ] Booking list UI (US-7, US-8, US-9)
- [ ] Dashboard with search (US-10)
- [ ] Station overview (US-11)
- [ ] Customer view (US-12)

### Phase 7: Additional Features (Weeks 12-13)
- [ ] Similar vehicle suggestions (US-6)
- [ ] Booking history (US-4)
- [ ] Notifications (Email, In-app, Push)
- [ ] Payment mock implementation

### Phase 8: Deployment & Polish (Week 14)
- [ ] Azure deployment configuration
- [ ] Production monitoring setup
- [ ] Performance testing
- [ ] Security audit
- [ ] Documentation completion

---

## Open Questions & Decisions Needed

### Questions Answered:
✅ Event Store: SQL Server-based custom implementation
✅ Authentication: Keycloak
✅ Read Database: SQL Server
✅ MVP Features: All four (Vehicles, Reservations, Customers, Payments-mock)
✅ Multi-location: Yes, multiple locations from start
✅ Notifications: Email, In-app, Push
✅ Payment Gateway: Mock/placeholder initially

### Business Decisions (Reasonable Defaults):

#### 1. Business Rules

**Cancellation Policy:**
- Free cancellation up to 48 hours before pickup
- 50% refund for cancellations 24-48 hours before pickup
- No refund for cancellations less than 24 hours before pickup

**Pricing:**
- Simple daily rate model initially (no weekend/seasonal pricing in MVP)
- Pricing can be extended later via PricingPolicy aggregate

**One-Way Rentals:**
- Not supported in MVP (pickup station = return station)
- Design allows for future extension

**Rental Duration:**
- Minimum: 1 day
- Maximum: 30 days

**Age Restrictions:**
- Minimum renter age: 21 years
- Young driver surcharge (21-24 years) can be added later

**Deposit:**
- Pre-authorization mock in payment flow
- Actual payment processing deferred to production payment gateway integration

#### 2. Vehicle Management

**Maintenance Scheduling:**
- Manual process for MVP
- Fleet managers can mark vehicles as "Maintenance" status
- Future: Automatic scheduling based on mileage/time

**Vehicle Unavailability Triggers:**
- Manual status change to "Maintenance" or "Retired"
- Active reservation makes vehicle unavailable for that period

**Damage Tracking:**
- Not in MVP scope
- Can be added as separate bounded context later

#### 3. Reservations

**Reservation Status Flow:**
```
Pending (initial) → Confirmed (after payment mock) → Active (pickup) → Completed (return)
                 ↘ Cancelled (any time before Active)
```

**Pending Duration:**
- 15 minutes timeout for unpaid reservations
- After timeout, reservation auto-cancels and vehicle becomes available

**Confirmation Process:**
- Automatic confirmation after successful mock payment
- No manual review needed for MVP

#### 4. Call Center Capabilities

**Reservation Management:**
- ✅ View all reservations (US-7, US-8, US-9)
- ✅ Search and filter reservations
- ✅ Create bookings on behalf of customers
- ❌ Modify existing reservations (not in MVP - can be added later)
- ❌ Pricing overrides (not in MVP)

**Workflow:**
- Call center can create reservations using same flow as public portal
- Payment collected separately (mock payment always succeeds for call center)

#### 5. Notifications

**Notification Triggers:**
- Reservation confirmed (Email + In-app)
- 24 hours before pickup reminder (Email + Push)
- Pickup due today (Push)
- Return due tomorrow (Email + Push)
- Reservation cancelled (Email + In-app)

**Notification Timing:**
- Immediate: Confirmation, cancellation
- Scheduled: Reminders (24h before, day of)

**Channels:**
- Email: Transactional (confirmation, cancellation)
- In-app: All notifications appear in notification center
- Push: Time-sensitive reminders only

#### 6. Reporting & Analytics

**MVP Scope:**
- Basic dashboards only (station overview US-11, customer booking counts US-12)
- No dedicated reporting module

**Future Enhancements:**
- Revenue reports
- Vehicle utilization reports
- Customer analytics
- Booking trends

---

## Additional Business Rules

### Value Object Constraints

**EmailAddress:**
- Must be valid email format
- Stored lowercase

**PhoneNumber:**
- Format: International format preferred
- Validation: Basic pattern matching

**DateOfBirth:**
- Must be at least 21 years ago (minimum renter age)

**PostalCode:**
- Format validation based on country

**Money:**
- Always positive amounts
- Currency must match (no cross-currency operations without explicit conversion)

**ReservationPeriod:**
- End date must be after start date
- Minimum duration: 1 day
- Maximum duration: 30 days
- No historical dates (start date >= today)

**VehicleName:**
- Maximum 200 characters
- Cannot be empty

**SeatCount:**
- Range: 2-9 seats

### Aggregate Invariants

**Vehicle:**
- Must belong to exactly one station at a time
- Cannot be deleted if active reservations exist
- Status transitions: Available ↔ Rented ↔ Maintenance → Retired (one-way to Retired)

**Reservation:**
- Vehicle must be available for the entire requested period
- Cannot modify after status is Active or Completed
- Cancellation only allowed for Pending or Confirmed status

**Station:**
- Must have valid address
- Name must be unique
- Cannot be deleted if vehicles are assigned or active reservations exist

**Customer:**
- Email must be unique
- Date of birth must satisfy minimum age requirement

---

## Next Steps

1. **Review this architecture document** and provide feedback
2. **Answer remaining open questions** to finalize business rules
3. **Approve the architecture** to proceed with implementation
4. **Initialize Git repository** on GitHub
5. **Begin Phase 1 implementation** - Foundation setup

Once approved, I'll begin creating the project structure, setting up the solution, and implementing the foundational components.

---

## Glossary

- **Aggregate:** A cluster of domain objects treated as a single unit (e.g., Reservation)
- **Bounded Context:** A logical boundary within which a domain model applies
- **Clean Architecture:** Layered architecture separating concerns (Domain, Application, Infrastructure, API)
- **Domain Event:** Something that happened in the domain (past tense)
- **Value Object:** Immutable object defined by its attributes, not identity
- **Aggregate Root:** The entry point to an aggregate, responsible for enforcing invariants
- **Repository:** Abstraction for data access, encapsulating persistence logic

---

**Document Status:** Approved - Ready for Implementation
**Version History:**
- v1.0 (2025-10-28): Initial architecture design
- v1.1 (2025-10-28): Business decisions finalized with reasonable defaults
