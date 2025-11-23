# End-to-End Testing Guide

## Overview

This document describes the end-to-end (E2E) testing setup for the Orange Car Rental Public Portal. E2E tests verify complete user flows and interactions across the application using [Playwright](https://playwright.dev/).

## Prerequisites

- Node.js 18+ installed
- Application dependencies installed (`npm install`)
- Local development server running or configured in `playwright.config.ts`

## Quick Start

### Running E2E Tests

```bash
# Run all E2E tests in headless mode
npm run e2e

# Run tests with interactive UI
npm run e2e:ui

# Run tests in headed mode (see browser)
npm run e2e:headed

# Run tests in debug mode
npm run e2e:debug

# Run tests in specific browser
npm run e2e:chromium
npm run e2e:firefox
npm run e2e:webkit

# View last test report
npm run e2e:report
```

### First Time Setup

Playwright browsers need to be installed before running tests:

```bash
npx playwright install
```

## Test Structure

### Directory Layout

```
e2e/
├── fixtures/
│   └── test-data.ts                 # Test data and mock objects
├── helpers/
│   ├── auth.helper.ts               # Authentication helper functions
│   └── booking.helper.ts            # Booking flow helper functions
├── us-1-vehicle-search.spec.ts      # US-1: Vehicle search tests
├── us-2-booking-flow.spec.ts        # US-2: Complete booking flow tests
├── us-3-authentication.spec.ts      # US-3: Authentication tests
├── us-4-booking-history.spec.ts     # US-4: Booking history tests
├── us-5-profile-prefill.spec.ts     # US-5: Profile pre-fill tests
└── us-6-similar-vehicles.spec.ts    # US-6: Similar vehicles tests
```

### Test Organization

Tests are organized by user story:

- **US-1 Vehicle Search**: Search filters, location, category, fuel type, transmission, seats, price display
- **US-2 Booking Flow**: 5-step wizard, form validation, navigation, confirmation
- **US-3 Authentication**: Login, registration, password reset flows
- **US-4 Booking History**: View bookings, details, guest lookup, cancellation
- **US-5 Profile Pre-fill**: Automatic form population for registered users
- **US-6 Similar Vehicles**: Alternative vehicle suggestions and booking

Each test file contains multiple test suites covering different aspects of the feature.

## Test Coverage

### US-1: Vehicle Search with Filters (30 tests)

**Basic Search Functionality** (4 tests)
- ✅ Display vehicle search form
- ✅ Perform basic search with dates
- ✅ Display vehicle information in results
- ✅ Validate pickup date before return date

**Location Filter** (2 tests)
- ✅ Filter by location
- ✅ Support different pickup/dropoff locations

**Category Filter** (2 tests)
- ✅ Filter by vehicle category
- ✅ Display all categories (KLEIN, KOMPAKT, MITTEL, etc.)

**Fuel Type Filter** (2 tests)
- ✅ Filter by fuel type
- ✅ Display fuel options (Petrol, Diesel, Electric, Hybrid)

**Transmission Type Filter** (1 test)
- ✅ Filter by transmission (Manual, Automatic)

**Seat Count Filter** (1 test)
- ✅ Filter by minimum seat count

**Price Display** (2 tests)
- ✅ Display prices in EUR with VAT
- ✅ Display daily rate

**Filter Combinations** (2 tests)
- ✅ Apply multiple filters simultaneously
- ✅ Reset all filters

**User Interactions** (3 tests)
- ✅ Navigate to booking page
- ✅ Display loading state
- ✅ Display empty state

**Responsive Design** (2 tests)
- ✅ Search form on mobile
- ✅ Responsive vehicle results grid

### US-2: Complete Booking Flow (5-Step Wizard) (48 tests)

**Step 1: Vehicle Details** (6 tests)
- ✅ Display vehicle and booking dates
- ✅ Display location selects
- ✅ Calculate rental days automatically
- ✅ Validate return after pickup date
- ✅ Allow editing dates
- ✅ Display progress indicator

**Step 2: Customer Information** (6 tests)
- ✅ Navigate to customer info step
- ✅ Validate required fields
- ✅ Validate first name min length
- ✅ Validate email format
- ✅ Validate phone format
- ✅ Validate age requirement (18+)
- ✅ Accept valid customer info

**Step 3: Address Information** (4 tests)
- ✅ Display address form fields
- ✅ Validate street min length
- ✅ Validate postal code (5 digits)
- ✅ Accept valid address
- ✅ Default country to Deutschland

**Step 4: Driver's License** (5 tests)
- ✅ Display license form fields
- ✅ Validate license number min length
- ✅ Validate issue date not in future
- ✅ Validate expiry after issue date
- ✅ Accept valid license info

**Step 5: Review & Submit** (6 tests)
- ✅ Display all entered information
- ✅ Display booking summary
- ✅ Show submit button
- ✅ Allow going back to edit
- ✅ Show loading indicator
- ✅ Display confirmation page

**Confirmation Page** (4 tests)
- ✅ Navigate to confirmation after booking
- ✅ Display reservation ID
- ✅ Display booking details
- ✅ Option to return to homepage

**Navigation & Progress** (5 tests)
- ✅ Show progress indicator
- ✅ Update progress through steps
- ✅ Allow back navigation
- ✅ Preserve data between steps
- ✅ Disable next when step invalid

**Error Handling** (2 tests)
- ✅ Display error on submission failure
- ✅ Allow retry after failure

**Responsive Design** (2 tests)
- ✅ Display form on mobile
- ✅ Stack fields vertically on mobile

### US-3: User Registration and Authentication (20 tests)

**Login Flow** (8 tests)
- ✅ Successful login with valid credentials
- ✅ Error handling for invalid credentials
- ✅ Email format validation
- ✅ Password visibility toggle
- ✅ "Remember Me" functionality
- ✅ Navigation to forgot password
- ✅ Navigation to registration

**Registration Flow** (6 tests)
- ✅ Successful user registration
- ✅ Password strength validation
- ✅ Password confirmation matching
- ✅ Minimum age validation (18+)
- ✅ Terms acceptance requirement
- ✅ Phone number format validation

**Forgot Password Flow** (3 tests)
- ✅ Forgot password request submission
- ✅ Email validation
- ✅ Navigation back to login

**Logout Flow** (1 test)
- ✅ Successful logout

### US-4: Booking History (38 tests)

**Authenticated User - Booking History** (13 tests)
- ✅ Navigate to booking history page
- ✅ Display page title
- ✅ Display list of bookings
- ✅ Display booking cards with info
- ✅ Group bookings by status
- ✅ Display status badges with colors
- ✅ Show "View Details" button
- ✅ Open booking detail view
- ✅ Display complete booking info
- ✅ Display vehicle information
- ✅ Display price breakdown
- ✅ Show cancel button for eligible bookings
- ✅ Show confirmation dialog for cancellation
- ✅ Display empty state when no bookings
- ✅ Show loading state
- ✅ Display dates in German format

**Guest Booking Lookup** (5 tests)
- ✅ Have guest booking lookup option
- ✅ Display guest lookup form
- ✅ Require reservation ID and email
- ✅ Lookup booking with valid data
- ✅ Show error for invalid lookup

**Booking Cancellation** (4 tests)
- ✅ Show cancellation policy info
- ✅ Require cancellation reason
- ✅ Show success message after cancellation
- ✅ Update booking status after cancellation

**Responsive Design** (2 tests)
- ✅ Display history on mobile
- ✅ Stack booking cards vertically on mobile

### US-5: Pre-fill Renter Data for Registered Users (14 tests)

**Authenticated User Booking** (8 tests)
- ✅ Pre-fill customer information
- ✅ Pre-fill address information
- ✅ Pre-fill driver's license information
- ✅ Modification of pre-filled data
- ✅ Profile update checkbox display
- ✅ Profile update when checkbox checked
- ✅ No profile update when checkbox unchecked
- ✅ Loading state during profile fetch

**Guest User Booking** (3 tests)
- ✅ No pre-fill for guest users
- ✅ No profile update checkbox for guests
- ✅ Complete booking flow for guests

**Edge Cases** (3 tests)
- ✅ Handle missing driver's license data
- ✅ Preserve modifications during navigation
- ✅ Form validation with pre-filled data

### US-6: Similar Vehicle Suggestions (21 tests)

**Similar Vehicles Display** (6 tests)
- ✅ Display similar vehicles section
- ✅ Display vehicle cards with information
- ✅ Display price comparison
- ✅ Display similarity reasons
- ✅ Display vehicle specifications
- ✅ Limit to maximum 4 suggestions

**Vehicle Unavailability Warning** (2 tests)
- ✅ Show warning for unavailable vehicles
- ✅ Display alternatives when unavailable

**"Book This Instead" Functionality** (7 tests)
- ✅ Switch to alternative vehicle
- ✅ Preserve booking dates
- ✅ Preserve location selection
- ✅ Update similar vehicles after switch
- ✅ Scroll to top after switch
- ✅ Clear unavailable warning after switch
- ✅ Update form with new vehicle details

**Responsive Design** (2 tests)
- ✅ Multi-column grid on desktop
- ✅ Single column layout on mobile

**Edge Cases** (2 tests)
- ✅ Handle no similar vehicles gracefully
- ✅ Handle API errors gracefully

**Total: 171 E2E Tests**

## Helper Functions

### Authentication Helpers

```typescript
import { login, register, logout, isLoggedIn } from './helpers/auth.helper';

// Login with test user
await login(page);

// Register new user
await register(page, userData);

// Check authentication status
const authenticated = await isLoggedIn(page);

// Logout
await logout(page);
```

### Booking Helpers

```typescript
import { startBooking, fillCustomerInfo, nextStep, completeBooking } from './helpers/booking.helper';

// Start booking with vehicle
await startBooking(page, vehicleId);

// Fill form sections
await fillCustomerInfo(page, userData);
await fillAddress(page, addressData);
await fillDriversLicense(page, licenseData);

// Navigate steps
await nextStep(page);

// Complete entire booking flow
await completeBooking(page, userData);
```

## Test Data

Test data is centralized in `fixtures/test-data.ts`:

```typescript
import { testUsers, testVehicles, testBooking } from './fixtures/test-data';

// Use registered user credentials
testUsers.registered.email
testUsers.registered.password

// Generate booking dates
const pickupDate = testBooking.pickupDate(); // 7 days from now
const returnDate = testBooking.returnDate();  // 14 days from now
```

## Configuration

### Playwright Configuration

The `playwright.config.ts` file configures:

- **Test Directory**: `./e2e`
- **Base URL**: `http://localhost:4200`
- **Browsers**: Chromium, Firefox, WebKit
- **Mobile Devices**: Pixel 5, iPhone 12
- **Retries**: 2 on CI, 0 locally
- **Reporters**: HTML, list, GitHub (on CI)
- **Dev Server**: Auto-starts Angular dev server

### Timeouts

- **Action timeout**: 30 seconds (Playwright default)
- **Navigation timeout**: 30 seconds (Playwright default)
- **Test timeout**: 30 seconds (Playwright default)

Adjust in `playwright.config.ts` if needed:

```typescript
use: {
  actionTimeout: 10000,  // 10 seconds
  navigationTimeout: 30000  // 30 seconds
}
```

## Best Practices

### 1. Use Helpers for Common Operations

Don't repeat login/booking flows - use helper functions:

```typescript
// ❌ Don't repeat
await page.goto('/login');
await page.fill('input[name="email"]', email);
await page.fill('input[name="password"]', password);
await page.click('button[type="submit"]');

// ✅ Use helpers
await login(page);
```

### 2. Wait for Elements Properly

```typescript
// ✅ Wait for elements before interacting
await page.waitForSelector('.booking-form', { timeout: 5000 });
await page.click('button');

// ✅ Wait for navigation
await page.click('button[type="submit"]');
await page.waitForURL('/confirmation', { timeout: 15000 });
```

### 3. Use Test Data Constants

```typescript
// ❌ Hardcoded values
await page.fill('input[name="email"]', 'test@example.com');

// ✅ Use test data
await page.fill('input[name="email"]', testUsers.registered.email);
```

### 4. Clean Up After Tests

```typescript
test.afterEach(async ({ page }) => {
  // Logout after authenticated tests
  await logout(page);
});
```

### 5. Handle Optional Elements

```typescript
// Check if element exists before interacting
const checkbox = page.locator('input[type="checkbox"]');
if (await checkbox.isVisible()) {
  await checkbox.check();
}
```

### 6. Use Descriptive Test Names

```typescript
// ✅ Clear, descriptive test names
test('should pre-fill customer information for logged-in user', async ({ page }) => {
  // Test implementation
});
```

## CI/CD Integration

### GitHub Actions Example

```yaml
name: E2E Tests

on: [push, pull_request]

jobs:
  test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      - uses: actions/setup-node@v3
        with:
          node-version: '18'

      - name: Install dependencies
        run: npm ci

      - name: Install Playwright browsers
        run: npx playwright install --with-deps

      - name: Run E2E tests
        run: npm run e2e
        env:
          CI: true

      - name: Upload test results
        if: always()
        uses: actions/upload-artifact@v3
        with:
          name: playwright-report
          path: playwright-report/
```

## Debugging

### Debug Mode

Run tests in debug mode with Playwright Inspector:

```bash
npm run e2e:debug
```

### Headed Mode

See the browser while tests run:

```bash
npm run e2e:headed
```

### UI Mode

Interactive UI for running and debugging tests:

```bash
npm run e2e:ui
```

### Screenshots and Videos

Failed tests automatically capture:
- **Screenshots**: On failure
- **Videos**: On failure (retain-on-failure)
- **Traces**: On first retry

Find artifacts in `test-results/` and `playwright-report/`.

### Inspect Element Selectors

Use Playwright Inspector to test selectors:

```bash
npx playwright codegen http://localhost:4200
```

## Known Issues and Limitations

### Test Data

- Tests use mock data defined in `fixtures/test-data.ts`
- Update test data if API contracts change
- Some tests may fail if backend is not running

### Timing Issues

- Tests may be flaky if API responses are slow
- Increase timeouts in `playwright.config.ts` if needed
- Use proper wait conditions instead of arbitrary delays

### Browser Compatibility

- All tests run on Chromium, Firefox, and WebKit
- Some CSS animations may behave differently across browsers
- Mobile viewport tests verify responsive design

## Maintenance

### Adding New Tests

1. Create test file: `e2e/us-X-feature-name.spec.ts`
2. Import helpers and test data
3. Write test suites with descriptive names
4. Use existing helpers where possible
5. Update this documentation

### Updating Test Data

Edit `e2e/fixtures/test-data.ts` to:
- Add new test users
- Update vehicle data
- Modify booking parameters

### Updating Helpers

Helpers are in `e2e/helpers/`:
- `auth.helper.ts` - Authentication flows
- `booking.helper.ts` - Booking operations

Add reusable functions that simplify test code.

## Troubleshooting

### Tests Fail Locally

1. **Check dev server**: Ensure `npm run start:dev` works
2. **Install browsers**: Run `npx playwright install`
3. **Clear cache**: Delete `node_modules/.cache`
4. **Check ports**: Ensure port 4200 is not in use

### Tests Pass Locally But Fail on CI

1. **Check timeouts**: CI may be slower, increase timeouts
2. **Check dependencies**: Ensure `npx playwright install --with-deps`
3. **Check environment**: Verify environment variables
4. **Review artifacts**: Download test results from CI

### Flaky Tests

1. **Avoid `waitForTimeout`**: Use proper wait conditions
2. **Wait for elements**: Always wait for elements before interaction
3. **Handle race conditions**: Wait for network requests to complete
4. **Increase retries**: Configure retries in `playwright.config.ts`

## Resources

- [Playwright Documentation](https://playwright.dev/)
- [Playwright Best Practices](https://playwright.dev/docs/best-practices)
- [Playwright API Reference](https://playwright.dev/docs/api/class-playwright)
- [Angular Testing Guide](https://angular.io/guide/testing)

## Support

For issues or questions:
1. Check this documentation
2. Review Playwright documentation
3. Check test output and artifacts
4. Contact the development team
