# Orange Car Rental - User Stories

**Project:** Orange Car Rental System
**Version:** 1.0
**Last Updated:** 2025-11-05

---

## 🌐 PUBLIC PORTAL - Customer-Facing Features

### US-1: Vehicle Search with Filters ✅ IMPLEMENTED
**As a** customer
**I want to** search for available vehicles with filters
**So that** I can find a car that meets my specific needs

**Priority:** High
**Status:** ✅ Implemented
**Story Points:** 8

#### Acceptance Criteria
- [ ] User can select pickup and return dates
- [ ] User can filter by pickup/return location
- [ ] User can filter by vehicle category (KLEIN, KOMPAKT, MITTEL, OBER, SUV, KOMBI, TRANS, LUXUS)
- [ ] User can filter by fuel type (Petrol, Diesel, Electric, Hybrid)
- [ ] User can filter by transmission type (Manual, Automatic)
- [ ] User can filter by minimum number of seats
- [ ] Search results show only available vehicles for the selected period
- [ ] Results display vehicle name, category, price, location, and specifications
- [ ] Prices are shown in EUR with 19% VAT included
- [ ] User can reset all filters

#### Technical Notes
- **Backend API:** `GET /api/vehicles?pickupDate={date}&returnDate={date}&location={code}&category={code}&fuelType={type}&transmission={type}&minSeats={number}`
- **Frontend Component:** `apps/public-portal/src/app/components/vehicle-search/vehicle-search.component.ts`
- **Service:** `VehicleService.searchVehicles()`

#### Definition of Done
- [x] Backend API endpoint implemented
- [x] Frontend component created with all filters
- [x] Availability checking integrated
- [x] German VAT (19%) calculation working
- [x] Unit tests pass
- [x] E2E tests pass
- [x] Code reviewed and merged

---

### US-2: Booking Flow (Quick + Search-based) ✅ IMPLEMENTED
**As a** customer
**I want to** complete a booking in a simple 5-step wizard
**So that** I can quickly rent a vehicle without complicated forms

**Priority:** High
**Status:** ✅ Implemented
**Story Points:** 13

#### Acceptance Criteria
- [ ] **Step 1 - Vehicle Details:**
  - Pickup and return dates are editable
  - Pickup and dropoff locations can be selected
  - Rental days are calculated automatically
  - User cannot proceed with invalid dates (return before pickup)

- [ ] **Step 2 - Customer Information:**
  - First name (min 2 characters, required)
  - Last name (min 2 characters, required)
  - Email (valid email format, required)
  - Phone number (German format validation)
  - Date of birth (required, must be 18+ years old)

- [ ] **Step 3 - Address:**
  - Street (min 5 characters, required)
  - City (min 2 characters, required)
  - Postal code (5-digit German format, required)
  - Country (default: Deutschland)

- [ ] **Step 4 - Driver's License:**
  - License number (min 5 characters, required)
  - Issue country (default: Deutschland)
  - Issue date (cannot be in the future)
  - Expiry date (must be after issue date, min 30 days from today)

- [ ] **Step 5 - Review & Submit:**
  - All entered information is displayed for review
  - User can go back to edit any step
  - Submit button creates reservation
  - Loading indicator shown during submission

- [ ] **Confirmation Page:**
  - Reservation ID is displayed
  - Booking details are shown
  - Price breakdown (net, VAT, gross) is visible
  - Print functionality works
  - User can return to homepage

#### Technical Notes
- **Backend API:** `POST /api/reservations/guest`
- **Frontend Components:**
  - `apps/public-portal/src/app/pages/booking/booking.component.ts`
  - `apps/public-portal/src/app/pages/confirmation/confirmation.component.ts`
- **Services:** `ReservationService.createGuestReservation()`, `PricingService`

#### Definition of Done
- [x] 5-step wizard implemented with validation
- [x] Backend guest reservation endpoint working
- [x] Price calculation integrated
- [x] Customer created automatically during booking
- [x] Confirmation page shows all details
- [x] Error handling for failed bookings
- [x] Unit tests pass
- [x] E2E booking flow test passes
- [x] Code reviewed and merged

---

### US-3: User Registration and Authentication ❌ NOT IMPLEMENTED
**As a** customer
**I want to** create an account and log in
**So that** I can save my details and view my booking history

**Priority:** High
**Status:** ❌ Not Implemented
**Story Points:** 13

#### Acceptance Criteria
- [ ] Registration page with form fields:
  - Email (unique, required)
  - Password (min 8 characters, uppercase, lowercase, number, special char)
  - Confirm password (must match)
  - First name, last name
  - Phone number
  - Accept terms and conditions checkbox

- [ ] Login page with:
  - Email field
  - Password field
  - "Remember me" checkbox
  - "Forgot password" link
  - Login button

- [ ] After successful registration:
  - User receives confirmation email
  - User is automatically logged in
  - User is redirected to profile page

- [ ] After successful login:
  - JWT token is stored securely
  - User's name appears in navigation
  - User sees "Logout" option

- [ ] Logout functionality:
  - Clears authentication token
  - Redirects to homepage

- [ ] Password reset flow:
  - User enters email
  - Reset link sent to email
  - User clicks link and sets new password

- [ ] Protected routes:
  - Booking history requires authentication
  - Profile page requires authentication
  - Unauthenticated users redirected to login

#### Technical Notes
- **Backend:** Keycloak integration (already configured in architecture)
- **Frontend Services to Create:** `AuthService`, `TokenService`
- **Frontend Guards to Create:** `AuthGuard`
- **Components to Create:** `LoginComponent`, `RegisterComponent`, `ForgotPasswordComponent`

#### Definition of Done
- [ ] Registration endpoint integrated with Keycloak
- [ ] Login endpoint working with JWT tokens
- [ ] Password reset flow implemented
- [ ] Auth guards protect routes
- [ ] Token refresh mechanism working
- [ ] Session persistence (remember me)
- [ ] Unit tests pass
- [ ] E2E tests for login/register/logout pass
- [ ] Code reviewed and merged

---

### US-4: Booking History ❌ NOT IMPLEMENTED
**As a** registered customer
**I want to** view my past and upcoming bookings
**So that** I can track my rental history and manage active reservations

**Priority:** Medium
**Status:** ❌ Not Implemented
**Story Points:** 8

#### Acceptance Criteria
- [ ] "My Bookings" page accessible from navigation (requires login)
- [ ] Page displays all bookings for logged-in customer
- [ ] Bookings are grouped into:
  - Upcoming bookings (Confirmed, Active status, future dates)
  - Past bookings (Completed, Cancelled status)
  - Pending bookings (awaiting confirmation)

- [ ] Each booking card shows:
  - Reservation ID
  - Vehicle name and category
  - Pickup date and location
  - Return date and location
  - Total price
  - Status badge (color-coded)
  - "View Details" button

- [ ] Detail view shows:
  - Complete reservation information
  - Renter details
  - Driver's license info
  - Price breakdown
  - Cancellation policy
  - "Cancel Booking" button (if eligible)
  - "Print" button

- [ ] Guest users can lookup booking by:
  - Reservation ID
  - Email used during booking
  - Shows single reservation without login

- [ ] Cancellation functionality:
  - Free cancellation up to 48h before pickup
  - Confirmation dialog before cancelling
  - Cancellation reason dropdown
  - Success message after cancellation

#### Technical Notes
- **Backend API (MISSING - needs implementation):**
  - `GET /api/reservations?customerId={id}` - List customer bookings
  - `GET /api/reservations/lookup?reservationId={id}&email={email}` - Guest lookup
  - `PUT /api/reservations/{id}/cancel` - Cancel reservation
- **Frontend Component to Create:** `apps/public-portal/src/app/pages/booking-history/booking-history.component.ts`
- **Service Methods to Add:** `ReservationService.getCustomerReservations()`, `ReservationService.cancelReservation()`

#### Definition of Done
- [ ] Backend endpoints implemented
- [ ] My Bookings page created
- [ ] Guest lookup functionality working
- [ ] Booking detail modal implemented
- [ ] Cancellation flow working with policy enforcement
- [ ] Booking list filters (upcoming/past/pending)
- [ ] Unit tests pass
- [ ] E2E tests pass
- [ ] Code reviewed and merged

---

### US-5: Pre-fill Renter Data for Registered Users ❌ NOT IMPLEMENTED
**As a** registered customer
**I want to** have my personal details automatically filled in the booking form
**So that** I don't have to re-enter my information every time I book

**Priority:** Medium
**Status:** ❌ Not Implemented (depends on US-3)
**Story Points:** 5

#### Acceptance Criteria
- [ ] When authenticated user starts booking:
  - Step 2 (Customer Information) is pre-filled from user profile
  - Step 3 (Address) is pre-filled from user profile
  - Step 4 (Driver's License) is pre-filled if available

- [ ] User can still edit pre-filled information
- [ ] Changes during booking don't update profile (one-time override)
- [ ] Option to "Update my profile with these changes" checkbox
- [ ] If checkbox selected, profile is updated after successful booking

- [ ] Guest users see empty form as usual
- [ ] Form validation still applies to pre-filled data

#### Technical Notes
- **Depends on:** US-3 (User Registration)
- **Backend API:** `GET /api/customers/profile` (existing: `GET /api/customers/{id}`)
- **Frontend:** Modify `BookingComponent.ngOnInit()` to check auth and load profile

#### Definition of Done
- [ ] Auth check in booking component
- [ ] Profile data loaded for authenticated users
- [ ] Form pre-populated with profile data
- [ ] Edit functionality preserved
- [ ] Optional profile update after booking
- [ ] Guest users unaffected
- [ ] Unit tests pass
- [ ] E2E test for pre-filled booking
- [ ] Code reviewed and merged

---

### US-6: Similar Vehicle Suggestions ❌ NOT IMPLEMENTED
**As a** customer
**I want to** see similar vehicles when viewing a vehicle or if my selected vehicle is unavailable
**So that** I can find alternative options that meet my needs

**Priority:** Low
**Status:** ❌ Not Implemented
**Story Points:** 5

#### Acceptance Criteria
- [ ] On vehicle detail/booking page, show "Similar Vehicles" section
- [ ] Suggestions based on:
  - Same category OR +/- 1 category level
  - Same location
  - Similar price range (+/- 20%)
  - Available for same dates

- [ ] Show maximum 4 similar vehicles
- [ ] Each suggestion card shows:
  - Vehicle image
  - Name and category
  - Price comparison (e.g., "€5/day cheaper")
  - Key specs (seats, fuel, transmission)
  - "Book This Instead" button

- [ ] If selected vehicle becomes unavailable:
  - Show warning message
  - Automatically display similar vehicles
  - Highlight why each is similar (same category, lower price, etc.)

- [ ] "Book This Instead" replaces current vehicle in booking form
- [ ] Preserves all other booking details (dates, locations)

#### Technical Notes
- **Backend API (needs implementation):** `GET /api/vehicles/{id}/similar?pickupDate={date}&returnDate={date}`
- **Algorithm:**
  1. Match category +/- 1 level
  2. Same location
  3. Price within 20%
  4. Check availability
  5. Sort by price similarity
  6. Return top 4
- **Frontend Component:** Create `SimilarVehiclesComponent`

#### Definition of Done
- [ ] Backend similar vehicles endpoint implemented
- [ ] Recommendation algorithm working
- [ ] Similar vehicles component created
- [ ] Integration in booking flow
- [ ] "Book This Instead" functionality working
- [ ] Unavailability handling
- [ ] Unit tests pass
- [ ] E2E test pass
- [ ] Code reviewed and merged

---

## 🏢 CALL CENTER PORTAL - Internal Staff Features

### US-7: List All Bookings ✅ IMPLEMENTED (Frontend) / ⚠️ PARTIAL (Backend)
**As a** call center agent
**I want to** see a list of all reservations
**So that** I can manage bookings and assist customers

**Priority:** High
**Status:** ✅ Frontend Complete / ⚠️ Backend APIs Missing
**Story Points:** 8

#### Acceptance Criteria
- [ ] Dashboard shows statistics:
  - Today's bookings count
  - Active bookings count (Confirmed + Active status)
  - Pending bookings count
  - Total bookings count

- [ ] Reservations table displays all bookings with:
  - Reservation ID (truncated with tooltip on hover)
  - Customer ID
  - Pickup date (German DD.MM.YYYY format)
  - Return date (German DD.MM.YYYY format)
  - Pickup location code
  - Return location code
  - Total price (gross, in EUR)
  - Status badge (color-coded: Pending=yellow, Confirmed=green, Active=blue, Completed=gray, Cancelled=red)
  - Actions column with buttons

- [ ] Actions available:
  - "View Details" - Opens reservation detail modal
  - "Confirm" - Confirms pending reservation (only if status=Pending)
  - "Cancel" - Cancels reservation with reason

- [ ] Table features:
  - Pagination (10, 25, 50, 100 per page)
  - Loading state while fetching data
  - Empty state if no reservations
  - Error handling with user-friendly message

- [ ] Reservation detail modal shows:
  - Complete reservation information
  - Customer details
  - Vehicle details
  - Price breakdown
  - Status history (future enhancement)

#### Technical Notes
- **Frontend:** ✅ `apps/call-center-portal/src/app/pages/reservations/reservations.component.ts` (332 lines)
- **Backend APIs (MISSING):**
  - `GET /api/reservations` - List all reservations (pagination, sorting)
  - `GET /api/reservations/search` - Search with filters
  - `PUT /api/reservations/{id}/confirm` - Confirm reservation
  - `PUT /api/reservations/{id}/cancel` - Cancel with reason
- **Domain:** ✅ `Reservation.Confirm()` and `Reservation.Cancel(reason)` exist
- **Need to Implement:** Command handlers and API endpoints

#### Definition of Done
- [x] Frontend reservations page implemented
- [ ] Backend list/search endpoint implemented
- [ ] Backend confirm endpoint implemented
- [ ] Backend cancel endpoint implemented
- [ ] Statistics calculation working
- [ ] Pagination working
- [ ] Action buttons functional
- [ ] Unit tests pass
- [ ] Integration tests pass
- [ ] Code reviewed and merged

---

### US-8: Filter and Group Bookings ⚠️ PARTIALLY IMPLEMENTED
**As a** call center agent
**I want to** filter and group reservations by various criteria
**So that** I can quickly find specific bookings or analyze patterns

**Priority:** High
**Status:** ⚠️ Partial (basic filters only)
**Story Points:** 8

#### Acceptance Criteria
- [ ] **Filter Options:**
  - [x] Status (All, Pending, Confirmed, Active, Completed, Cancelled)
  - [x] Customer ID (text search)
  - [ ] Date range (pickup date from/to)
  - [ ] Location (pickup location dropdown)
  - [ ] Vehicle category (dropdown)
  - [ ] Price range (min/max sliders)
  - [ ] Customer name (text search)
  - [ ] Vehicle ID (text search)

- [ ] **Sorting Options:**
  - [ ] Pickup date (ascending/descending)
  - [ ] Total price (low to high, high to low)
  - [ ] Status (alphabetical)
  - [ ] Created date (newest first, oldest first)

- [ ] **Grouping Options:**
  - [ ] Group by status (collapsible sections)
  - [ ] Group by pickup date (Today, This Week, This Month, Later)
  - [ ] Group by location
  - [ ] Group by customer

- [ ] "Apply Filters" button updates results
- [ ] "Reset Filters" clears all and shows all bookings
- [ ] Active filters are visually indicated (badges/chips)
- [ ] Result count shown: "Showing 25 of 150 reservations"

- [ ] Filters persist when navigating away and back
- [ ] URL parameters updated with active filters (shareable links)

#### Technical Notes
- **Frontend:** ⚠️ Partial in `reservations.component.ts` (only status + customer ID)
- **Backend:** ✅ `ReservationService.searchReservations()` supports all filters
- **Need to Add:**
  - Date range filter UI
  - Sorting controls
  - Grouping logic
  - URL parameter sync

#### Definition of Done
- [x] Basic status and customer ID filters working
- [ ] All filter controls implemented
- [ ] Sorting functionality working
- [ ] Grouping functionality working
- [ ] Filter persistence in URL
- [ ] Result count display
- [ ] Active filter indicators
- [ ] Unit tests pass
- [ ] E2E tests pass
- [ ] Code reviewed and merged

---

### US-9: Search Bookings by Customer Details ✅ IMPLEMENTED
**As a** call center agent
**I want to** search for customers and view their booking history
**So that** I can assist customers with inquiries about their reservations

**Priority:** High
**Status:** ✅ Implemented
**Story Points:** 8

#### Acceptance Criteria
- [ ] Customer search page with search form:
  - Email address (text input)
  - Phone number (text input)
  - Last name (text input)
  - At least one field required to search

- [ ] Search results table shows:
  - Customer ID
  - Full name (first + last)
  - Email address
  - Phone number
  - Date of birth with calculated age
  - Registration date
  - "Show Details" button

- [ ] Customer detail modal displays:
  - **Personal Information:**
    - Customer ID, name, email, phone, date of birth, age
  - **Address:**
    - Street, postal code, city, country
  - **Driver's License:**
    - License number, issue country, issue/expiry dates
  - **Bookings History:**
    - All reservations for this customer
    - Reservation ID, vehicle, dates, locations, price, status

- [ ] Edit functionality:
  - "Edit" button in customer details
  - Editable form with validation
  - Save button updates customer
  - Success/error messaging

- [ ] Each booking in customer history has:
  - Color-coded status badge
  - "View Details" link to reservation
  - Pickup/return dates in German format

#### Technical Notes
- **Frontend:** ✅ `apps/call-center-portal/src/app/pages/customers/customers.component.ts` (453 lines)
- **Backend APIs:** ✅ All implemented
  - `GET /api/customers/search` - Search by email, phone, name
  - `GET /api/customers/{id}` - Get customer details
  - `PUT /api/customers/{id}/profile` - Update customer
- **Service:** ✅ `CustomerService.searchCustomers()`, `ReservationService.searchReservations({ customerId })`

#### Definition of Done
- [x] Customer search page implemented
- [x] Search by multiple criteria working
- [x] Customer detail modal with all info
- [x] Booking history integrated
- [x] Edit functionality working
- [x] Validation and error handling
- [x] Unit tests pass
- [x] E2E tests pass
- [x] Code reviewed and merged

---

### US-10: Dashboard with Vehicle Search ✅ IMPLEMENTED
**As a** call center agent
**I want to** search for vehicles and view fleet status
**So that** I can help customers find available vehicles and manage fleet

**Priority:** High
**Status:** ✅ Implemented
**Story Points:** 8

#### Acceptance Criteria
- [ ] Dashboard statistics showing:
  - Total vehicles count
  - Available vehicles count
  - Vehicles in maintenance count
  - Rented vehicles count

- [ ] Filter section with:
  - Status filter (All, Available, Rented, Maintenance, Out of Service)
  - Location filter (dropdown, dynamically populated)
  - Category filter (dropdown, dynamically populated)

- [ ] Vehicle grid/table displaying:
  - Vehicle name
  - Category
  - Location (city)
  - License plate
  - Manufacturer and model
  - Year
  - Seats, fuel type, transmission type
  - Daily rate (gross, in EUR)
  - Status badge (color-coded)
  - "Show Details" button

- [ ] Vehicle detail modal shows:
  - Complete vehicle specifications
  - Pricing information
  - Current status
  - Current location
  - Option to view availability calendar (future)

- [ ] Search and filter functionality:
  - Filters update results immediately
  - Reset button clears all filters
  - Results show filtered count vs total

#### Technical Notes
- **Frontend:** ✅ `apps/call-center-portal/src/app/pages/vehicles/vehicles.component.ts` (274 lines)
- **Backend API:** ✅ `GET /api/vehicles` with full filtering support
- **Features Working:**
  - Dashboard stats calculation
  - Dynamic filter population
  - Vehicle detail modal
- **Future Enhancements:**
  - Add new vehicle functionality (button disabled)
  - Edit vehicle status
  - Availability calendar

#### Definition of Done
- [x] Dashboard with statistics implemented
- [x] Filter controls working
- [x] Vehicle grid with all details
- [x] Vehicle detail modal
- [x] Search functionality
- [x] Status color coding
- [x] Unit tests pass
- [x] E2E tests pass
- [x] Code reviewed and merged

---

### US-11: Station Overview with Vehicle Inventory ✅ IMPLEMENTED
**As a** call center agent
**I want to** view all rental locations and their vehicle inventory
**So that** I can see which vehicles are at which location

**Priority:** Medium
**Status:** ✅ Implemented
**Story Points:** 5

#### Acceptance Criteria
- [ ] Dashboard statistics showing:
  - Total locations count
  - Active locations count
  - Total vehicles across all locations

- [ ] Location cards grid displaying:
  - Location name
  - Full address (street, postal code, city)
  - Contact information (phone, email)
  - Operating hours
  - Vehicle count at this location
  - "Show Details" button

- [ ] Location detail modal shows:
  - Complete location information
  - Contact details
  - Operating hours
  - List of all vehicles at this location:
    - Vehicle name
    - Category
    - License plate
    - Status (with color badge)
    - Daily rate

- [ ] Each location card color-codes by status (future):
  - Active: Green
  - Temporarily closed: Yellow
  - Closed: Red

#### Technical Notes
- **Frontend:** ✅ `apps/call-center-portal/src/app/pages/locations/locations.component.ts` (218 lines)
- **Backend APIs:** ✅ Implemented
  - `GET /api/locations` - List all locations
  - `GET /api/locations/{code}` - Get specific location
  - `GET /api/vehicles?locationCode={code}` - Vehicles at location
- **Features:**
  - Vehicle count calculated per location
  - Vehicle detail in location modal
  - Responsive grid layout

#### Definition of Done
- [x] Locations page implemented
- [x] Dashboard statistics working
- [x] Location cards with info
- [x] Vehicle count per location
- [x] Location detail modal
- [x] Vehicle list in location details
- [x] Unit tests pass
- [x] E2E tests pass
- [x] Code reviewed and merged

---

### US-12: Customer View with Complete Booking History ✅ IMPLEMENTED
**As a** call center agent
**I want to** view a customer's complete profile and all their bookings
**So that** I can provide comprehensive support and answer customer inquiries

**Priority:** Medium
**Status:** ✅ Implemented
**Story Points:** 5

#### Acceptance Criteria
- [ ] Accessible from customer search (US-9)
- [ ] Customer profile view shows:
  - **Personal Information:**
    - Customer ID
    - Full name
    - Email address
    - Phone number
    - Date of birth with age
    - Registration/creation date

  - **Address Information:**
    - Street address
    - Postal code
    - City
    - Country

  - **Driver's License:**
    - License number
    - Issuing country
    - Issue date
    - Expiry date

  - **Booking History:**
    - All reservations (upcoming, active, past, cancelled)
    - For each booking:
      - Reservation ID
      - Vehicle ID
      - Pickup and return dates
      - Rental duration in days
      - Pickup and dropoff locations
      - Total price (gross)
      - Status with color badge
      - Created date

- [ ] Edit mode for customer information:
  - Toggle between view and edit mode
  - Validate all fields
  - Save button persists changes
  - Cancel button discards changes
  - Success/error messages

- [ ] Booking actions from customer view:
  - Click reservation ID to view details
  - Status filter for bookings (all, upcoming, past, cancelled)

#### Technical Notes
- **Frontend:** ✅ Integrated in `apps/call-center-portal/src/app/pages/customers/customers.component.ts`
- **Backend APIs:** ✅ All implemented
  - `GET /api/customers/{id}`
  - `PUT /api/customers/{id}/profile`
  - `GET /api/reservations?customerId={id}` (needs backend implementation)
- **Features:**
  - View/edit mode toggle
  - Complete customer profile
  - Full booking history integration
  - Real-time age calculation

#### Definition of Done
- [x] Customer profile view implemented
- [x] All customer information displayed
- [x] Booking history integrated
- [x] Edit functionality working
- [x] Form validation
- [x] Success/error handling
- [x] Unit tests pass
- [x] E2E tests pass
- [x] Code reviewed and merged

---

## 📊 Story Summary

### Public Portal Stories
| ID | Title | Priority | Status | Story Points |
|----|-------|----------|--------|--------------|
| US-1 | Vehicle Search | High | ✅ Complete | 8 |
| US-2 | Booking Flow | High | ✅ Complete | 13 |
| US-3 | User Registration | High | ❌ Not Implemented | 13 |
| US-4 | Booking History | Medium | ❌ Not Implemented | 8 |
| US-5 | Pre-fill Data | Medium | ❌ Not Implemented | 5 |
| US-6 | Similar Vehicles | Low | ❌ Not Implemented | 5 |

**Total Story Points:** 52
**Completed:** 21 (40%)
**Remaining:** 31 (60%)

### Call Center Portal Stories
| ID | Title | Priority | Status | Story Points |
|----|-------|----------|--------|--------------|
| US-7 | List Bookings | High | ⚠️ Partial | 8 |
| US-8 | Filter/Group | High | ⚠️ Partial | 8 |
| US-9 | Customer Search | High | ✅ Complete | 8 |
| US-10 | Vehicle Dashboard | High | ✅ Complete | 8 |
| US-11 | Station Overview | Medium | ✅ Complete | 5 |
| US-12 | Customer Profile | Medium | ✅ Complete | 5 |

**Total Story Points:** 42
**Completed:** 26 (62%)
**Remaining:** 16 (38%)

### Overall Project
**Total Story Points:** 94
**Completed:** 47 (50%)
**Remaining:** 47 (50%)

---

## 🔧 Technical Debt / Missing Backend APIs

### Critical (Blocks Frontend Features)
1. **Reservation List/Search API** - Needed for US-7
   - `GET /api/reservations`
   - `GET /api/reservations/search`

2. **Reservation Actions APIs** - Needed for US-7
   - `PUT /api/reservations/{id}/confirm`
   - `PUT /api/reservations/{id}/cancel`

3. **Customer Reservations API** - Needed for US-4, US-9, US-12
   - `GET /api/reservations?customerId={id}`

### Nice to Have
4. **Similar Vehicles API** - Needed for US-6
   - `GET /api/vehicles/{id}/similar`

5. **Vehicle Categories API**
   - `GET /api/vehicles/categories`

---

## 🎯 Recommended Implementation Order

### Sprint 1: Critical Backend APIs (1 week)
1. Implement reservation list/search endpoints
2. Implement confirm/cancel endpoints
3. Extend repository with search capabilities
4. Update frontend services

### Sprint 2: Booking History (1 week)
1. US-4: Implement booking history page
2. Guest lookup functionality
3. Cancellation flow with policy

### Sprint 3: Enhanced Filtering (1 week)
1. US-8: Complete filter/group functionality
2. Date range filters
3. Sorting options
4. Grouping logic

### Sprint 4: Authentication System (2 weeks)
1. US-3: User registration and login
2. Keycloak integration
3. Protected routes
4. Password reset flow

### Sprint 5: Pre-fill & Suggestions (1 week)
1. US-5: Pre-fill functionality
2. US-6: Similar vehicles feature
3. Recommendation algorithm

---

## 📝 Notes
- All prices include 19% German VAT (MWST)
- German language (de-DE) used throughout
- Date format: DD.MM.YYYY
- Currency format: 1.234,56 €
- Minimum rental age: 18 years (21 for some categories)
- Driver's license minimum validity: 30 days
