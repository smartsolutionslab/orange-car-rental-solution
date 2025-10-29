# Orange Car Rental System

A modern, cloud-ready car rental software system built with Domain-Driven Design, CQRS, Event Sourcing, and microservices architecture.

**Target Market:** 🇩🇪 German market with full GDPR/DSGVO compliance, German language support, and VAT handling

[![Backend CI](https://github.com/YOUR_USERNAME/orange-car-rental/workflows/Backend%20CI/badge.svg)](https://github.com/YOUR_USERNAME/orange-car-rental/actions)
[![Frontend CI](https://github.com/YOUR_USERNAME/orange-car-rental/workflows/Frontend%20CI/badge.svg)](https://github.com/YOUR_USERNAME/orange-car-rental/actions)

## Features

### Public Portal
- Vehicle search with advanced filtering (date, location, category)
- Quick booking and search-based booking flows
- User registration and authentication
- Booking history and management
- Pre-filled booking forms for registered users
- Similar vehicle suggestions

### Call Center Portal
- Comprehensive booking overview with filtering
- Customer search and management
- Station dashboard with real-time availability
- Create bookings on behalf of customers
- Advanced search and filtering capabilities

## Architecture

### Backend (.NET)
- **Framework:** .NET 9 with Minimal API
- **Pattern:** Domain-Driven Design with CQRS and Event Sourcing
- **Event Store:** Custom SQL Server implementation
- **Read Models:** SQL Server with optimized projections
- **Authentication:** Keycloak integration
- **Orchestration:** .NET Aspire for local development
- **Testing:** xUnit, FluentAssertions, integration tests

### Frontend (Angular)
- **Framework:** Angular 18+ with standalone components
- **Monorepo:** Nx workspace with two applications
- **Styling:** Tailwind CSS with minimalist design
- **State:** NgRx Signal Store
- **Testing:** Jest, Testing Library, Playwright E2E

### Bounded Contexts
1. **Fleet Management** - Vehicles, stations, categories
2. **Reservations** - Booking lifecycle and availability
3. **Customers** - User profiles and registration
4. **Pricing** - Pricing policies and calculations
5. **Payments** - Payment processing (mock for MVP)
6. **Notifications** - Multi-channel notifications (Email, In-app, Push)

## Getting Started

### Prerequisites

**Backend:**
- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [SQL Server 2022](https://www.microsoft.com/sql-server) or [Docker](https://www.docker.com/)
- [Keycloak](https://www.keycloak.org/) or Docker

**Frontend:**
- [Node.js 20+](https://nodejs.org/)
- [npm](https://www.npmjs.com/)

**Optional:**
- [Docker Desktop](https://www.docker.com/products/docker-desktop)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/)
- [Rider](https://www.jetbrains.com/rider/)

### Quick Start with .NET Aspire

The easiest way to run the entire system locally:

```bash
# Clone the repository
git clone https://github.com/YOUR_USERNAME/orange-car-rental.git
cd orange-car-rental

# Start all services with .NET Aspire
cd src/backend/AppHost
dotnet run
```

This single command will:
- Start SQL Server (via Docker)
- Start Keycloak (via Docker)
- Start RabbitMQ (via Docker)
- Launch all backend microservices
- Launch both Angular frontends
- Open the Aspire dashboard at https://localhost:15888

### Manual Setup

#### Backend

```bash
# Navigate to backend
cd src/backend

# Restore dependencies
dotnet restore

# Run database migrations
dotnet ef database update --project Services/Fleet/OrangeCarRental.Fleet.Infrastructure

# Start a specific service
cd Services/Fleet/OrangeCarRental.Fleet.Api
dotnet run
```

#### Frontend

```bash
# Navigate to frontend
cd src/frontend

# Install dependencies
npm install

# Start public portal
npx nx serve public-portal

# Start call center portal (in another terminal)
npx nx serve call-center-portal
```

## Project Structure

```
orange-car-rental/
├── src/
│   ├── backend/
│   │   ├── BuildingBlocks/          # Shared infrastructure
│   │   ├── Services/                # Bounded context services
│   │   │   ├── Fleet/
│   │   │   ├── Reservations/
│   │   │   ├── Customers/
│   │   │   ├── Pricing/
│   │   │   ├── Payments/
│   │   │   └── Notifications/
│   │   ├── ApiGateway/
│   │   └── AppHost/                 # .NET Aspire orchestration
│   │
│   └── frontend/
│       ├── apps/
│       │   ├── public-portal/       # Customer-facing app
│       │   └── call-center-portal/  # Internal agent app
│       └── libs/
│           ├── shared-ui/           # Shared components
│           ├── data-access/         # API clients
│           └── util/                # Utilities
│
├── docs/                            # Documentation
├── scripts/                         # Build and deployment scripts
├── ARCHITECTURE.md                  # Architecture documentation
└── README.md                        # This file
```

## Development

### Running Tests

**Backend:**
```bash
# Unit tests
dotnet test --filter "Category=Unit"

# Integration tests
dotnet test --filter "Category=Integration"

# All tests with coverage
dotnet test --collect:"XPlat Code Coverage"
```

**Frontend:**
```bash
# Unit tests
npx nx test public-portal

# E2E tests
npx nx e2e public-portal-e2e

# Test all projects
npx nx run-many --target=test --all
```

### Running Local CI/CD Pipeline

Run the full CI/CD pipeline locally before pushing:

```bash
./scripts/local-pipeline.sh
```

This will:
- Build all backend services
- Run all backend tests
- Build all frontend apps
- Run all frontend tests
- Verify linting and code style

### Git Workflow

We use feature branches for all development:

```bash
# Create feature branch
git checkout -b feature/US-1-vehicle-search

# Make changes and commit
git add .
git commit -m "feat(fleet): implement vehicle search endpoint"

# Push and create PR
git push origin feature/US-1-vehicle-search
```

**Branch naming:**
- `feature/US-{number}-{description}` - New features
- `bugfix/{issue-number}-{description}` - Bug fixes
- `hotfix/{description}` - Production hotfixes

### Coding Standards

#### Backend (C#)
- No primitive types in domain models - use value objects
- Sealed records for value objects
- Past tense for domain events (`ReservationConfirmed`)
- One class per file
- Private fields with `_camelCase`

#### Frontend (TypeScript/Angular)
- Standalone components
- Smart/Dumb component pattern
- Signal-based state management
- Tailwind utility classes
- Reactive forms for inputs

See [ARCHITECTURE.md](./ARCHITECTURE.md) for complete guidelines.

## Deployment

### Azure Deployment

The system supports deployment to Azure using:
- **Azure Container Apps** or **Azure Kubernetes Service (AKS)**
- **Azure SQL Database** for event store and read models
- **Azure Service Bus** for event distribution
- **Azure Key Vault** for secrets
- **Azure Application Insights** for monitoring

```bash
# Deploy via GitHub Actions
git push origin main
```

### On-Premise Deployment

For on-premise deployments, we provide:
- Docker Compose configuration
- Kubernetes Helm charts
- Setup scripts

```bash
# Docker Compose
cd deployment/docker-compose
docker-compose up -d

# Kubernetes
cd deployment/kubernetes
helm install orange-car-rental ./charts
```

## API Documentation

Once running, API documentation is available at:
- Fleet API: http://localhost:5000/scalar/v1
- Reservations API: http://localhost:5001/scalar/v1
- Customers API: http://localhost:5002/scalar/v1
- Pricing API: http://localhost:5003/scalar/v1

## Technology Stack

### Backend
- .NET 9
- Entity Framework Core 9
- SQL Server 2022
- Keycloak
- RabbitMQ / Azure Service Bus
- Serilog
- FluentValidation
- Polly

### Frontend
- Angular 18+
- Nx
- Tailwind CSS
- NgRx Signal Store
- TypeScript
- Jest & Playwright

### DevOps
- GitHub Actions
- Docker
- .NET Aspire
- Codecov

## User Stories

### Implemented
- [x] US-1: Vehicle search with filters
- [x] US-2: Booking flow (quick + search)
- [x] US-3: User registration
- [x] US-4: Booking history
- [x] US-5: Pre-fill renter data
- [x] US-6: Similar vehicle suggestions
- [x] US-7: Call center booking list
- [x] US-8: Filter and group bookings
- [x] US-9: Customer/date search
- [x] US-10: Call center dashboard
- [x] US-11: Station overview
- [x] US-12: Customer management view

### Planned
- [ ] Payment gateway integration (Stripe/PayPal)
- [ ] Reservation modifications
- [ ] Vehicle maintenance tracking
- [ ] Advanced reporting
- [ ] Mobile responsive improvements
- [ ] Multi-language support

## German Market Compliance

This system is specifically designed for the German car rental market with the following compliance features:

### Legal & Regulatory
- ✅ **GDPR/DSGVO Compliance** - Full data protection with right to erasure
- ✅ **Impressum** - Legal notice page with all required information
- ✅ **Datenschutzerklärung** - Privacy policy page
- ✅ **Cookie Consent** - GDPR-compliant cookie banner
- ✅ **VAT Handling** - 19% German VAT with separate net/gross amounts

### Localization
- ✅ **German Language** - Primary UI language (English as secondary)
- ✅ **German Date Format** - DD.MM.YYYY format
- ✅ **German Currency Format** - 1.234,56 € format
- ✅ **German Postal Codes** - 5-digit validation
- ✅ **CET/CEST Time Zone** - Central European Time

### Payment Methods
- ⚠️ **SEPA Direct Debit** - Planned for production
- ✅ **Credit/Debit Cards** - Visa, Mastercard, EC-Karte
- ✅ **PayPal** - Popular in German market

### Business Features
- ✅ **Age Validation** - Minimum 21 years for renters
- ⚠️ **Business Customers** - B2B support with VAT ID validation (planned)
- ⚠️ **Kilometer Packages** - 100km/day, unlimited options (planned)
- ⚠️ **Vehicle Extras** - GPS, child seats, winter tires (planned)

See [GERMAN_MARKET_REQUIREMENTS.md](./GERMAN_MARKET_REQUIREMENTS.md) for complete compliance documentation.

## Business Rules

### Reservations
- **Minimum rental:** 1 day
- **Maximum rental:** 30 days
- **Minimum age:** 21 years
- **Cancellation:** Free up to 48h before pickup

### Pricing
- Daily rate model
- Category-based pricing
- Location-specific rates

### Availability
- Real-time availability checking
- Automatic reservation timeout (15 minutes)
- Multi-location support

See [ARCHITECTURE.md](./ARCHITECTURE.md) for complete business rules.

## Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/US-X-description`)
3. Commit your changes (`git commit -m 'feat(scope): description'`)
4. Push to the branch (`git push origin feature/US-X-description`)
5. Open a Pull Request

### Commit Message Format

```
<type>(<scope>): <subject>

<body>

Implements: US-X
```

**Types:** `feat`, `fix`, `docs`, `style`, `refactor`, `test`, `chore`

## License

This project is proprietary software. All rights reserved.

## Support

For issues and questions:
- Create an issue on GitHub
- Check the [documentation](./docs)
- Review [ARCHITECTURE.md](./ARCHITECTURE.md)

## Authors

- **Heiko** - Architecture & Development

## Acknowledgments

- Built with Domain-Driven Design principles
- Inspired by modern microservices patterns
- Following CQRS and Event Sourcing best practices
