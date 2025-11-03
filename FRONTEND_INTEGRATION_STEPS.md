# Frontend Guest Booking Integration - Manual Steps

This document outlines the manual steps needed to complete the guest booking feature integration.

## ✅ Completed Components

The following frontend components have been created:

### 1. Services (src/app/services/)
- ✅ `reservation.model.ts` - TypeScript interfaces for reservation data
- ✅ `reservation.service.ts` - Angular service for Reservations API

### 2. Booking Page (src/app/pages/booking/)
- ✅ `booking.component.ts` - Multi-step booking form component
- ✅ `booking.component.html` - Booking form template
- ✅ `booking.component.css` - Booking form styles

### 3. Confirmation Page (src/app/pages/confirmation/)
- ✅ `confirmation.component.ts` - Confirmation page component
- ✅ `confirmation.component.html` - Confirmation page template
- ✅ `confirmation.component.css` - Confirmation page styles

## 📝 Manual Steps Required

### Step 1: Update Routes (app.routes.ts)

**File:** `src/frontend/apps/public-portal/src/app/app.routes.ts`

Replace the entire file content with:

```typescript
import { Routes } from '@angular/router';
import { VehicleListComponent } from './pages/vehicle-list/vehicle-list.component';
import { BookingComponent } from './pages/booking/booking.component';
import { ConfirmationComponent } from './pages/confirmation/confirmation.component';

export const routes: Routes = [
  {
    path: '',
    component: VehicleListComponent
  },
  {
    path: 'booking',
    component: BookingComponent
  },
  {
    path: 'confirmation',
    component: ConfirmationComponent
  }
];
```

### Step 2: Add Navigation to Vehicle List Component

**File:** `src/frontend/apps/public-portal/src/app/pages/vehicle-list/vehicle-list.component.ts`

#### 2a. Add Router Import

Add `Router` to the imports at the top (line 3):

```typescript
import { Router } from '@angular/router';
```

#### 2b. Inject Router

Add the router injection in the component class (around line 19):

```typescript
export class VehicleListComponent implements OnInit {
  private readonly vehicleService = inject(VehicleService);
  private readonly router = inject(Router);  // ADD THIS LINE

  protected readonly vehicles = signal<Vehicle[]>([]);
  // ... rest of the code
```

#### 2c. Store Current Search Query

Add a property to store the current search query (around line 24):

```typescript
  protected readonly totalCount = signal(0);
  protected readonly currentSearchQuery = signal<VehicleSearchQuery>({});  // ADD THIS LINE
```

#### 2d. Update onSearch Method

Update the `onSearch` method to store the query (around line 34):

```typescript
  protected onSearch(query: VehicleSearchQuery) {
    this.currentSearchQuery.set(query);  // ADD THIS LINE
    this.loading.set(true);
    this.error.set(null);

    // ... rest of the method stays the same
  }
```

#### 2e. Add Booking Navigation Method

Add this method at the end of the class (before the closing brace):

```typescript
  /**
   * Navigate to booking page with vehicle and search details
   */
  protected onBookVehicle(vehicle: Vehicle): void {
    const query = this.currentSearchQuery();

    this.router.navigate(['/booking'], {
      queryParams: {
        vehicleId: vehicle.id,
        categoryCode: vehicle.categoryCode,
        pickupDate: query.pickupDate || '',
        returnDate: query.returnDate || '',
        locationCode: query.locationCode || vehicle.locationCode
      }
    });
  }
```

### Step 3: Update Vehicle List Template

**File:** `src/frontend/apps/public-portal/src/app/pages/vehicle-list/vehicle-list.component.html`

Find the "Jetzt buchen" button (around line 88) and update it:

**Before:**
```html
<button class="btn-book">Jetzt buchen</button>
```

**After:**
```html
<button class="btn-book" (click)="onBookVehicle(vehicle)">Jetzt buchen</button>
```

### Step 4: Fix Booking Form - Missing vehicleId and categoryCode

**File:** `src/frontend/apps/public-portal/src/app/pages/booking/booking.component.ts`

The booking form needs to extract categoryCode from the vehicle. Since we don't have a `getVehicleById` method yet, we need to pass it via query params.

Update the `ngOnInit` method (around line 98):

**Find this section:**
```typescript
    this.route.queryParams.subscribe(params => {
      const vehicleId = params['vehicleId'];
      const pickupDate = params['pickupDate'];
      const returnDate = params['returnDate'];
      const locationCode = params['locationCode'];

      if (vehicleId) {
        this.loadVehicle(vehicleId);
        this.bookingForm.patchValue({
          vehicleId,
          pickupDate: pickupDate || '',
          returnDate: returnDate || '',
          pickupLocationCode: locationCode || '',
          dropoffLocationCode: locationCode || ''
        });
      }
    });
```

**Replace with:**
```typescript
    this.route.queryParams.subscribe(params => {
      const vehicleId = params['vehicleId'];
      const categoryCode = params['categoryCode'];
      const pickupDate = params['pickupDate'];
      const returnDate = params['returnDate'];
      const locationCode = params['locationCode'];

      if (vehicleId && categoryCode) {
        this.bookingForm.patchValue({
          vehicleId,
          categoryCode,
          pickupDate: pickupDate || '',
          returnDate: returnDate || '',
          pickupLocationCode: locationCode || '',
          dropoffLocationCode: locationCode || ''
        });
      }
    });
```

**Also remove or comment out the `loadVehicle` method** (around line 119) since we're not using it:

```typescript
  /**
   * Load vehicle details
   */
  // private loadVehicle(vehicleId: string): void {
  //   // Note: This would need a getVehicleById method in VehicleService
  //   // For now, we'll extract the category code from the URL or use a placeholder
  //   // In production, you'd want to fetch the full vehicle details
  // }
```

## 🧪 Testing the Complete Flow

After completing all manual steps:

1. **Start the application:**
   - Backend: Run all services (Fleet, Pricing, Reservations, Customers)
   - Frontend: `ng serve` in the public-portal directory

2. **Test the booking flow:**
   - Navigate to the vehicle list page
   - Use the search filters (optional)
   - Click "Jetzt buchen" on any vehicle
   - Fill out the 5-step booking form:
     - Step 1: Verify booking details (dates, locations)
     - Step 2: Enter customer information
     - Step 3: Enter address
     - Step 4: Enter driver's license details
     - Step 5: Review and submit
   - Verify navigation to confirmation page
   - Check that reservation details are displayed correctly

3. **Verify backend integration:**
   - Check that the customer is created in Customers service
   - Check that the reservation is created in Reservations service
   - Verify pricing was calculated by Pricing service
   - Check database for new records

## 🔗 API Endpoints Used

- **POST** `/api/reservations/guest` - Create guest reservation
- **GET** `/api/reservations/{id}` - Get reservation details
- **GET** `/api/fleet/search` - Search vehicles
- **GET** `/api/locations` - Get available locations

## 📊 Backend Status

✅ All backend services are implemented and tested:
- 78 Reservations tests passing (18 new CreateGuestReservationCommandHandler tests)
- CreateGuestReservationCommand and Handler implemented
- CustomersService integration complete
- PricingService integration complete
- POST /api/reservations/guest endpoint added

## 🎨 UI Features

The booking form includes:
- 5-step wizard with progress indicator
- Form validation at each step
- Date validation (pickup/return)
- Location selection
- Automatic price calculation (backend)
- Responsive design (mobile-friendly)
- Error handling and display
- Loading states
- Success confirmation page with:
  - Reservation number
  - Booking details
  - Price breakdown
  - Print functionality

## 💡 Next Enhancements (Optional)

Consider these future improvements:
1. Add vehicle details display on booking page
2. Implement real-time price preview on form
3. Add booking cancellation functionality
4. Email confirmation integration
5. Payment processing integration
6. Booking history for guests (via email lookup)
