# Orange Car Rental System

A modern, cloud-ready car rental software system built with Domain-Driven Design, CQRS, Event Sourcing, and microservices architecture.

**Target Market:** 🇩🇪 German market with full GDPR/DSGVO compliance, German language support, and VAT handling

[![E2E Tests](https://github.com/YOUR_USERNAME/orange-car-rental/workflows/E2E%20Tests%20-%20Public%20Portal/badge.svg)](https://github.com/YOUR_USERNAME/orange-car-rental/actions)
[![Unit Tests](https://github.com/YOUR_USERNAME/orange-car-rental/workflows/Unit%20Tests/badge.svg)](https://github.com/YOUR_USERNAME/orange-car-rental/actions)
[![PR Checks](https://github.com/YOUR_USERNAME/orange-car-rental/workflows/Pull%20Request%20Checks/badge.svg)](https://github.com/YOUR_USERNAME/orange-car-rental/actions)
[![codecov](https://codecov.io/gh/YOUR_USERNAME/orange-car-rental/branch/main/graph/badge.svg)](https://codecov.io/gh/YOUR_USERNAME/orange-car-rental)

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

## Documentation

Complete guides for operating and developing the Orange Car Rental system:

### Quick Start & Operations
- 📘 [**ASPIRE-QUICKSTART.md**](./ASPIRE-QUICKSTART.md) - Quick start guide for .NET Aspire development
- 📊 [**DEPLOYMENT-STATUS.md**](./DEPLOYMENT-STATUS.md) - Current system status and health report
- 🗺️ [**NEXT-STEPS.md**](./NEXT-STEPS.md) - Recommended development tasks and roadmap

### Architecture & Testing
- 🏗️ [**ARCHITECTURE.md**](./ARCHITECTURE.md) - System architecture and design patterns
- 🎭 [**E2E Testing Guide**](./src/frontend/apps/public-portal/E2E_TESTING.md) - Playwright E2E testing
- ⚡ [**Performance Testing**](./performance-tests/README.md) - k6 load and performance testing
- 📋 [**User Stories**](./USER_STORIES.md) - Feature implementation status
- 📈 [**Performance SLOs**](./PERFORMANCE_SLOS.md) - Service Level Objectives

### Deployment & Operations
- 🚀 [**DEPLOYMENT.md**](./DEPLOYMENT.md) - Production deployment guide
- 🔧 [**MONITORING.md**](./MONITORING.md) - Observability and alerting
- 🔐 [**SECURITY.md**](./SECURITY.md) - Security guidelines
- 🚢 [**CI/CD Pipeline**](./CI-CD-SETUP.md) - GitHub Actions workflows
- 🔍 [**TROUBLESHOOTING.md**](./TROUBLESHOOTING.md) - Common issues and solutions

### Database
- 🗄️ Database migrations are handled automatically by EF Core via .NET Aspire
- 📝 Seed data is applied automatically on service startup

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
├── infrastructure/                  # Azure deployment templates
├── monitoring/                      # Monitoring configuration
├── ARCHITECTURE.md                  # Architecture documentation
└── README.md                        # This file
```

## Development

### Testing Infrastructure

This project has **production-ready test coverage** with **600+ test scenarios** across unit, integration, and E2E tests.

**Quick Start:**
```bash
# Install dependencies
cd src/frontend/apps/public-portal
npm install
npx playwright install

# Run unit tests
npm test

# Run E2E tests (interactive mode)
npm run e2e:ui

# Run all E2E tests
npm run e2e
```

**Test Coverage:**
- **Unit Tests**: 400+ tests (~90% coverage across frontend and backend)
- **Integration Tests**: 50+ tests with real HTTP calls
- **E2E Tests**: 171 tests across 6 user stories and 3 browsers (Chromium, Firefox, WebKit)
  - US-1: Vehicle Search (30 tests)
  - US-2: Booking Flow (48 tests)
  - US-3: Authentication (20 tests)
  - US-4: Booking History (38 tests)
  - US-5: Profile Pre-fill (14 tests)
  - US-6: Similar Vehicles (21 tests)

**Documentation:**
- 🎭 [E2E Testing Guide](./src/frontend/apps/public-portal/E2E_TESTING.md) - Complete Playwright E2E documentation
- ⚡ [Performance Testing Guide](./performance-tests/README.md) - k6 load and performance testing
- 📊 [Performance SLOs](./PERFORMANCE_SLOS.md) - Service Level Objectives and benchmarks
- 🚀 [CI/CD Pipeline](./CI-CD-SETUP.md) - GitHub Actions workflows and deployment
- 📋 [User Stories](./USER_STORIES.md) - Feature implementation status and test coverage

**Backend Tests:**
```bash
# Unit tests
dotnet test --filter "Category=Unit"

# Integration tests
dotnet test --filter "Category=Integration"

# All tests with coverage
dotnet test --collect:"XPlat Code Coverage"
```

**Frontend Tests:**
```bash
# Unit tests
cd src/frontend/apps/public-portal
npm test                    # Watch mode
npm run test:ci             # Headless CI mode
npm run test:coverage       # With coverage report

# E2E tests with Playwright
npm run e2e                 # Run all 171 tests
npm run e2e:ui              # Interactive UI mode (recommended)
npm run e2e:headed          # Run in headed mode
npm run e2e:debug           # Debug mode with Playwright Inspector
npm run e2e:chromium        # Run on Chromium only
npm run e2e:report          # View last test report
```

**Performance Tests:**
```bash
# Load and performance testing with k6
cd performance-tests

# Quick smoke test (verify system is working)
npm run test:smoke

# Load test (test under normal load)
npm run test:load

# Stress test (test beyond normal capacity)
npm run test:stress

# Spike test (sudden traffic spike)
npm run test:spike

# End-to-end booking flow performance
npm run test:booking-flow

# Test against specific environment
ENVIRONMENT=staging npm run test:load
```

**Performance Metrics:**
- **Target Response Time:** P95 < 500ms, P99 < 1000ms
- **Target Error Rate:** < 1% for all endpoints
- **Target Throughput:** 1000+ requests/second (production)
- **6 test scenarios:** Smoke, Load, Stress, Spike, Booking Flow, API-specific
- **Automated weekly tests** on staging via GitHub Actions
- **Comprehensive SLOs** for all endpoints and operations

See [Performance Testing Guide](./performance-tests/README.md) for complete documentation.

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

### Production-Ready Infrastructure

This project includes **complete production-ready deployment infrastructure** for Azure with Kubernetes:

**Infrastructure Components:**
- ✅ **Azure Kubernetes Service (AKS)** - Managed Kubernetes cluster with auto-scaling
- ✅ **Azure Container Registry (ACR)** - Private Docker registry
- ✅ **Azure Database for PostgreSQL** - Managed database with automated backups
- ✅ **Azure Key Vault** - Secure secrets management
- ✅ **Application Insights** - Monitoring and observability
- ✅ **NGINX Ingress Controller** - Load balancing with SSL/TLS
- ✅ **cert-manager** - Automated SSL certificate management with Let's Encrypt

**Deployment Features:**
- Infrastructure-as-Code with Azure Bicep templates
- Kubernetes manifests with Kustomize overlays (dev, staging, production)
- Automated database migrations
- Automated daily backups
- Health checks and liveness probes
- Horizontal Pod Autoscaling (HPA)
- Complete monitoring and alerting

**Documentation:**
- 📘 [Complete Deployment Guide](./DEPLOYMENT.md) - Full deployment instructions
- 📋 [Quick Reference](./infrastructure/QUICK_REFERENCE.md) - Common operations
- 🔧 [Monitoring Guide](./MONITORING.md) - Observability and alerting

### Quick Deploy to Azure

```bash
# 1. Deploy Azure infrastructure (one-time setup)
cd infrastructure/azure
az deployment sub create \
  --name orange-prod-deployment \
  --location westeurope \
  --template-file main.bicep \
  --parameters @parameters.production.json

# 2. Setup secrets and ingress
cd ../scripts
./setup-secrets.sh production orange-production-kv
./setup-ingress.sh production

# 3. Deploy application
./deploy.sh production v1.0.0
```

**Deployment time:** ~30 minutes for complete setup

### Environments

**Development:**
- **Purpose:** Local development and testing
- **Cost:** ~$150/month
- **Resources:** 1 replica per service, Burstable database
- **URL:** https://dev.orange-rental.de

**Staging:**
- **Purpose:** Pre-production testing and QA
- **Cost:** ~$300/month
- **Resources:** 2 replicas per service, Burstable database
- **URL:** https://staging.orange-rental.de

**Production:**
- **Purpose:** Live production environment
- **Cost:** ~$800/month
- **Resources:** 3-5 replicas with auto-scaling, High-availability database
- **URL:** https://orange-rental.de

### CI/CD Pipeline

Automated deployment via GitHub Actions:
- ✅ Build and test on every PR
- ✅ Automated E2E tests (171 tests across 3 browsers)
- ✅ Docker image building and pushing to ACR
- ✅ Automated deployment to staging on merge to main
- ✅ Manual approval for production deployment
- ✅ Automated database migrations
- ✅ Rollback capability

See [CI-CD-SETUP.md](./CI-CD-SETUP.md) for complete CI/CD documentation.

### Monitoring & Observability

Production monitoring stack:
- **Application Insights** - Request tracking, exceptions, custom metrics
- **Serilog** - Structured logging with JSON output
- **Health Checks** - /health/live and /health/ready endpoints
- **Grafana Dashboards** - Real-time metrics visualization
- **Azure Monitor Alerts** - 6 pre-configured alert rules
- **Prometheus Metrics** - Custom business metrics

See [MONITORING.md](./MONITORING.md) for complete monitoring documentation.

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

### DevOps & Testing
- GitHub Actions CI/CD
- Docker & Docker Compose
- .NET Aspire
- Playwright (E2E Testing)
- Codecov (Coverage Reporting)
- Automated PR Checks
- Multi-browser Testing (Chromium, Firefox, WebKit)

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
