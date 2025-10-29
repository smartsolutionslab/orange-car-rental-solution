# Orange Car Rental - Frontend

**Status:** ✅ Complete and Running!

## Quick Start

Both Angular applications are successfully set up and running:

### Public Portal (Customer-facing)
```bash
cd apps/public-portal
npm start
```
- **URL:** http://localhost:4200
- **Purpose:** Customer portal for browsing vehicles, making reservations, viewing bookings

### Call Center Portal
```bash
cd apps/call-center-portal
npm start -- --port 4201
```
- **URL:** http://localhost:4201
- **Purpose:** Internal portal for call center agents to manage bookings and customers

## Architecture

### Directory Structure
```
src/frontend/
├── apps/
│   ├── public-portal/          ✅ Running on :4200
│   │   ├── src/
│   │   │   ├── app/           # Application components
│   │   │   ├── styles.css     # Tailwind CSS with Orange brand colors
│   │   │   └── index.html
│   │   ├── angular.json
│   │   └── package.json
│   │
│   └── call-center-portal/     ✅ Running on :4201
│       ├── src/
│       │   ├── app/           # Application components
│       │   ├── styles.css     # Tailwind CSS with dashboard styles
│       │   └── index.html
│       ├── angular.json
│       └── package.json
│
├── libs/
│   ├── shared-ui/             # Reusable UI components
│   │   └── src/lib/components/
│   │
│   ├── data-access/           # API services and models
│   │   └── src/lib/
│   │       ├── services/
│   │       └── models/
│   │
│   └── util/                  # Utility functions
│       └── src/lib/
│           ├── formatters/    ✅ German currency & date formatters
│           └── validators/
│
├── tailwind.config.js         ✅ Orange brand colors configured
├── package.json
└── README.md                  # This file
```

## Technology Stack

- **Angular:** 18+ with standalone components
- **Styling:** Tailwind CSS 3.x
- **Build Tool:** Angular CLI
- **Package Manager:** npm
- **TypeScript:** Latest

## Tailwind Configuration

### Orange Brand Colors
The Orange Car Rental brand colors are configured in `tailwind.config.js`:

- **Primary Orange:** `#ef6c1b` (primary-500)
- Full color palette from 50-950 for all shades
- Additional UI colors: secondary, success, warning, error

### Custom CSS Classes

#### Buttons
- `.btn-primary` - Orange brand button
- `.btn-secondary` - Gray button
- `.btn-outline` - Outlined button

#### Cards
- `.card` - Standard card with shadow
- `.card-compact` - Compact card (call center)
- `.widget` - Dashboard widget

#### Forms
- `.input` - Standard input with focus ring

#### Tables (Call Center)
- `.table` - Responsive table with hover states

#### Status Badges (Call Center)
- `.status-active` - Green success badge
- `.status-pending` - Yellow warning badge
- `.status-inactive` - Red error badge

## Shared Libraries

### Utility Functions (German Market)

**Currency Formatter** (`libs/util/src/lib/formatters/currency-formatter.ts`):
```typescript
import { CurrencyFormatter } from '@orange-car-rental/util';

// Format German currency
const price = CurrencyFormatter.formatGerman(100); // "100,00 €"

// Format with VAT (19%)
const priceWithVat = CurrencyFormatter.formatWithVat(100);
// Returns: { net: "100,00 €", vat: "19,00 €", gross: "119,00 €" }

// Parse German formatted string
const amount = CurrencyFormatter.parseGerman("1.234,56 €"); // 1234.56
```

**Date Formatter** (`libs/util/src/lib/formatters/date-formatter.ts`):
```typescript
import { DateFormatter } from '@orange-car-rental/util';

// Format German short date
const date = DateFormatter.formatGermanShort(new Date()); // "28.10.2025"

// Format German long date
const dateLong = DateFormatter.formatGermanLong(new Date()); // "28. Oktober 2025"

// Format date-time
const datetime = DateFormatter.formatGermanDateTime(new Date()); // "28.10.2025, 19:16"

// Parse German date
const parsed = DateFormatter.parseGermanDate("28.10.2025"); // Date object

// Calculate rental days
const days = DateFormatter.calculateRentalDays(startDate, endDate); // 5
```

## Development

### Run Both Apps Simultaneously

**Terminal 1 - Public Portal:**
```bash
cd src/frontend/apps/public-portal
npm start
```

**Terminal 2 - Call Center Portal:**
```bash
cd src/frontend/apps/call-center-portal
npm start -- --port 4201
```

### Build for Production

**Public Portal:**
```bash
cd apps/public-portal
npm run build
```

**Call Center Portal:**
```bash
cd apps/call-center-portal
npm run build
```

### Run Tests

**Public Portal:**
```bash
cd apps/public-portal
npm test
```

**Call Center Portal:**
```bash
cd apps/call-center-portal
npm test
```

## Next Steps

### 1. Implement User Story 1: Vehicle Search

**Backend** (Fleet service):
- Create Vehicle aggregate with value objects
- Implement SearchVehiclesQuery handler
- Add GET /api/vehicles endpoint

**Frontend** (Public Portal):
- Create vehicle search component
- Add date pickers for rental period
- Add location/station dropdown
- Add vehicle category filter
- Display vehicle cards with pricing
- Show VAT-inclusive prices using German formatter

**Example Component:**
```typescript
import { Component } from '@angular/core';
import { CurrencyFormatter, DateFormatter } from '@orange-car-rental/util';

@Component({
  selector: 'app-vehicle-search',
  template: `
    <div class="card">
      <h2 class="text-2xl font-bold text-primary-500">Fahrzeug suchen</h2>
      <!-- Search form here -->
    </div>

    <div class="grid grid-cols-3 gap-4 mt-6">
      <div class="card cursor-pointer" *ngFor="let vehicle of vehicles">
        <h3 class="font-bold">{{ vehicle.name }}</h3>
        <p class="text-gray-600">{{ vehicle.category }}</p>
        <p class="text-2xl font-bold text-primary-500 mt-4 currency-de">
          {{ formatPrice(vehicle.dailyRate) }}
        </p>
        <p class="text-sm text-gray-500">pro Tag (inkl. MwSt.)</p>
        <button class="btn-primary w-full mt-4">Jetzt buchen</button>
      </div>
    </div>
  `
})
export class VehicleSearchComponent {
  formatPrice(amount: number): string {
    return CurrencyFormatter.formatWithVat(amount).gross;
  }
}
```

### 2. Create Shared UI Components

In `libs/shared-ui/src/lib/components/`:
- Button component
- Input component
- Card component
- DatePicker component (with German locale)
- Vehicle card component
- Loading spinner
- Modal/Dialog

### 3. Set Up API Services

In `libs/data-access/src/lib/services/`:
- VehicleService
- ReservationService
- CustomerService
- PricingService

### 4. State Management

Consider adding:
- NgRx Signal Store for global state
- Or simple services with BehaviorSubjects for now

### 5. Frontend CI/CD Pipeline

Create `.github/workflows/frontend-ci.yml`:
- Build both apps
- Run tests
- Build Docker images
- Deploy to Azure Static Web Apps

## German Market Compliance

✅ **Currency Formatting:** German locale (1.234,56 €)
✅ **Date Formatting:** German format (DD.MM.YYYY)
✅ **VAT Calculation:** Automatic 19% VAT included
✅ **Language:** Ready for German i18n
✅ **GDPR:** Data handling ready (implement consent forms)

## Resources

- [Angular Documentation](https://angular.dev)
- [Tailwind CSS Documentation](https://tailwindcss.com)
- Backend API: http://localhost:5000 (once implemented)
- [Project Architecture](../../ARCHITECTURE.md)
- [German Market Requirements](../../GERMAN_MARKET_REQUIREMENTS.md)

## Success! 🎉

Both frontend applications are successfully set up and running! The foundation is complete, and you're ready to start implementing user stories.

**What's working:**
- ✅ Two Angular apps (public-portal, call-center-portal)
- ✅ Tailwind CSS with Orange brand colors
- ✅ Shared libraries structure
- ✅ German market formatters (currency, dates)
- ✅ Development servers running

**URLs:**
- Public Portal: http://localhost:4200
- Call Center Portal: http://localhost:4201
