# E2E Tests with Playwright

This directory contains end-to-end tests for the Orange Car Rental application using Playwright.

## Prerequisites

- Node.js 18+ installed
- Backend API running on `http://localhost:5000`
- Public Portal running on `http://localhost:4200`
- Call Center Portal running on `http://localhost:4201`
- Test Keycloak users configured

## Installation

```bash
# From project root
npm install --save-dev @playwright/test @types/node

# Install Playwright browsers
npx playwright install
```

## Project Structure

```
e2e/
├── helpers/
│   └── auth.helper.ts          # Authentication helper functions
├── pages/
│   ├── booking-history.page.ts # Page Object for Booking History
│   └── reservations.page.ts    # Page Object for Reservations
├── booking-history.e2e.spec.ts # E2E tests for booking history
├── reservations.e2e.spec.ts    # E2E tests for reservations
└── README.md                   # This file
```

## Configuration

Configuration is in `playwright.config.ts` at project root.

### Environment Variables

Create a `.env` file in project root:

```env
# Base URLs (optional, defaults provided)
BASE_URL=http://localhost:4200

# Test User Credentials
TEST_CUSTOMER_USERNAME=test-customer@example.com
TEST_CUSTOMER_PASSWORD=Test123!
TEST_AGENT_USERNAME=test-agent@example.com
TEST_AGENT_PASSWORD=Test123!

# Test Data
TEST_RESERVATION_ID=123e4567-e89b-12d3-a456-426614174000
TEST_GUEST_EMAIL=guest@example.com
```

## Running Tests

### Run All E2E Tests

```bash
# Run all tests
npx playwright test

# Run tests in headed mode (see browser)
npx playwright test --headed

# Run tests in debug mode
npx playwright test --debug
```

### Run Specific Test Files

```bash
# Booking History tests only
npx playwright test e2e/booking-history.e2e.spec.ts

# Reservations tests only
npx playwright test e2e/reservations.e2e.spec.ts
```

### Run Tests in Specific Browsers

```bash
# Chrome only
npx playwright test --project=chromium

# Firefox only
npx playwright test --project=firefox

# WebKit (Safari) only
npx playwright test --project=webkit

# Mobile Chrome
npx playwright test --project="Mobile Chrome"
```

### Run Tests with UI

```bash
# Interactive UI mode
npx playwright test --ui
```

## Test Reports

### View HTML Report

```bash
npx playwright show-report
```

Reports are generated in `playwright-report/` directory.

### View Traces

When tests fail, traces are automatically captured. View them with:

```bash
npx playwright show-trace playwright-report/trace.zip
```

## Writing New Tests

### Page Object Pattern

Create page objects in `e2e/pages/`:

```typescript
import { Page, Locator } from '@playwright/test';

export class MyPage {
  readonly page: Page;
  readonly myElement: Locator;

  constructor(page: Page) {
    this.page = page;
    this.myElement = page.locator('.my-element');
  }

  async goto(): Promise<void> {
    await this.page.goto('/my-page');
  }

  async doSomething(): Promise<void> {
    await this.myElement.click();
  }
}
```

### Test Structure

```typescript
import { test, expect } from '@playwright/test';
import { MyPage } from './pages/my.page';

test.describe('My Feature', () => {
  let myPage: MyPage;

  test.beforeEach(async ({ page }) => {
    myPage = new MyPage(page);
    await myPage.goto();
  });

  test('should do something', async () => {
    await myPage.doSomething();
    expect(await myPage.myElement.isVisible()).toBe(true);
  });
});
```

## Test Coverage

### Booking History Tests (Public Portal)

- ✅ Authenticated user booking history display
- ✅ Guest reservation lookup
- ✅ Reservation details viewing
- ✅ Reservation cancellation (with 48-hour policy)
- ✅ Empty state handling
- ✅ Form validation
- ✅ Error handling
- ✅ Responsive design (mobile/tablet)

### Reservations Tests (Call Center Portal)

- ✅ Initial load and authentication
- ✅ Status filtering
- ✅ Date range filtering
- ✅ Price range filtering
- ✅ Location filtering
- ✅ Multiple filters combined
- ✅ Sorting (by price, status, date)
- ✅ Grouping (by status, location, date)
- ✅ Pagination
- ✅ Reservation confirmation
- ✅ Reservation cancellation
- ✅ URL parameter synchronization
- ✅ Browser compatibility

## CI/CD Integration

Tests are configured to run in GitHub Actions. See `.github/workflows/e2e-tests.yml`.

### Required Secrets

Configure these secrets in GitHub repository settings:

- `TEST_CUSTOMER_USERNAME`
- `TEST_CUSTOMER_PASSWORD`
- `TEST_AGENT_USERNAME`
- `TEST_AGENT_PASSWORD`

## Debugging Tips

### Screenshots and Videos

Failed tests automatically capture screenshots and videos in `test-results/`.

### Debug Mode

```bash
# Run with debugger
npx playwright test --debug

# Run specific test with debugger
npx playwright test -g "should filter by status" --debug
```

### Slow Motion

```bash
# Run tests in slow motion (milliseconds)
npx playwright test --slow-mo=1000
```

### Console Logs

View console logs from the browser:

```typescript
test('my test', async ({ page }) => {
  page.on('console', msg => console.log(msg.text()));
  // ... test code
});
```

## Best Practices

1. **Use Page Objects**: Keep tests maintainable by using page objects
2. **Wait for Loading**: Always wait for loading states to complete
3. **Explicit Waits**: Use explicit waits instead of arbitrary timeouts
4. **Descriptive Names**: Use clear, descriptive test names
5. **Independent Tests**: Each test should be independent and not rely on others
6. **Clean Up**: Reset state between tests using `beforeEach`/`afterEach`
7. **Mobile Testing**: Test responsive design on mobile viewports

## Troubleshooting

### Tests Fail to Start

- Ensure all services are running (backend, frontend)
- Check Keycloak is configured correctly
- Verify test user credentials exist

### Authentication Fails

- Check Keycloak realm and client configuration
- Verify test user has correct roles
- Check CORS settings

### Flaky Tests

- Add explicit waits for elements
- Use `waitFor()` instead of `waitForTimeout()`
- Check for race conditions
- Increase timeout if needed

### Slow Tests

- Run tests in parallel: `npx playwright test --workers=4`
- Use `--project` to run specific browsers only
- Skip unnecessary setup in tests

## Performance

- **Average test execution time**: 30-60 seconds per test file
- **Parallel execution**: Tests run in parallel by default
- **CI execution time**: ~5-10 minutes for full suite

## Support

For issues or questions:
- Check [Playwright Documentation](https://playwright.dev/docs/intro)
- Review test logs in `playwright-report/`
- Check application logs for backend errors

---

**Last Updated**: 2025-11-20
**Playwright Version**: 1.40+
**Node Version Required**: 18+
