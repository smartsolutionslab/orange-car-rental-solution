# Quick Start Guide - Testing Infrastructure

This guide will help you get started with running all the tests in the Orange Car Rental project.

## Prerequisites

Ensure you have the following installed:

```bash
# Node.js 18+
node --version

# npm 9+
npm --version

# .NET 8+
dotnet --version
```

## Initial Setup

### 1. Install Dependencies

```bash
# Install root dependencies (Playwright)
npm install

# Install Public Portal dependencies
cd src/frontend/apps/public-portal
npm install

# Install Call Center Portal dependencies
cd ../call-center-portal
npm install

# Restore backend dependencies
cd ../../../backend
dotnet restore
```

### 2. Install Playwright Browsers

```bash
# From project root
npx playwright install
```

## Running Tests

### Unit Tests

**Public Portal:**
```bash
cd src/frontend/apps/public-portal

# Run tests once
npm test

# Run tests with coverage
npm run test:coverage

# Run tests in CI mode (headless)
npm run test:ci

# Run tests in watch mode
npm run test:watch
```

**Call Center Portal:**
```bash
cd src/frontend/apps/call-center-portal

# Run tests once
npm test

# Run tests with coverage
npm run test:coverage

# Run tests in CI mode
npm run test:ci
```

**Backend:**
```bash
cd src/backend

# Run all tests
dotnet test

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"
```

### Integration Tests

Integration tests require the backend API to be running.

**Setup:**
```bash
# Terminal 1: Start backend API
cd src/backend/Api
dotnet run

# Terminal 2: Run integration tests
cd src/frontend/apps/public-portal
npm run test:integration
```

**Run Integration Tests:**
```bash
# Public Portal
cd src/frontend/apps/public-portal
npm run test:integration

# Call Center Portal
cd src/frontend/apps/call-center-portal
npm run test:integration
```

### E2E Tests with Playwright

E2E tests require the full stack to be running.

**Setup:**
```bash
# Terminal 1: Start backend API
cd src/backend/Api
dotnet run

# Terminal 2: Start Public Portal
cd src/frontend/apps/public-portal
npm run start

# Terminal 3: Start Call Center Portal
cd src/frontend/apps/call-center-portal
npm run start

# Terminal 4: Run E2E tests
cd <project-root>
npm run test:e2e
```

**E2E Test Commands:**
```bash
# From project root

# Run all E2E tests
npm run test:e2e

# Run with visible browser
npm run test:e2e:headed

# Run in debug mode
npm run test:e2e:debug

# Run with UI mode (recommended)
npm run test:e2e:ui

# Run specific browser
npm run test:e2e:chromium
npm run test:e2e:firefox
npm run test:e2e:webkit

# Run mobile tests only
npm run test:e2e:mobile

# View last test report
npm run playwright:report
```

### Run All Tests

From project root:
```bash
# Run all unit tests
npm run test:unit

# Run all integration tests
npm run test:integration

# Run all E2E tests
npm run test:e2e

# Run everything (unit + integration + E2E)
npm run test:all
```

## Environment Variables for E2E Tests

Create a `.env` file in the project root:

```env
# Base URLs
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

## Viewing Test Results

### Unit Test Coverage

After running `npm run test:coverage`:

```bash
# Public Portal
cd src/frontend/apps/public-portal
open coverage/index.html

# Call Center Portal
cd src/frontend/apps/call-center-portal
open coverage/index.html
```

### Playwright Reports

After E2E tests complete:

```bash
# View HTML report
npm run playwright:report

# Or manually open
open playwright-report/index.html
```

### Test Artifacts

- **Screenshots**: `test-results/` (failures only)
- **Videos**: `test-results/` (failures only)
- **Coverage**: `coverage/`
- **E2E Reports**: `playwright-report/`

## Quick Test Matrix

| Test Type | Command | Time | Coverage |
|-----------|---------|------|----------|
| **Unit Tests** | `npm test` | 3-5 min | ~89% |
| **Integration** | `npm run test:integration` | 5-8 min | Full HTTP |
| **E2E** | `npm run test:e2e` | 15-20 min | Full Stack |

## Troubleshooting

### Unit Tests Fail

```bash
# Clear cache
rm -rf node_modules coverage .angular
npm install
```

### Integration Tests Timeout

```bash
# Ensure API is running
curl http://localhost:5000/health

# Check if port is in use
lsof -i :5000
```

### E2E Tests Fail to Start

```bash
# Verify all services are running
curl http://localhost:5000/health  # API
curl http://localhost:4200         # Public Portal
curl http://localhost:4201         # Call Center Portal

# Reinstall Playwright browsers
npx playwright install --force
```

### Port Already in Use

```bash
# Find and kill process on port
# Windows
netstat -ano | findstr :4200
taskkill /PID <pid> /F

# macOS/Linux
lsof -ti:4200 | xargs kill -9
```

## CI/CD Pipeline

Tests run automatically in GitHub Actions:

- **On Push**: Unit tests, integration tests, E2E tests
- **On PR**: All tests + coverage report
- **Daily**: Full E2E test suite at 2 AM UTC
- **On Deploy**: Smoke tests after deployment

View results at: `https://github.com/<org>/<repo>/actions`

## Test Development

### Creating New Unit Tests

```typescript
// src/frontend/apps/public-portal/src/app/my-feature/my.component.spec.ts
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { MyComponent } from './my.component';

describe('MyComponent', () => {
  let component: MyComponent;
  let fixture: ComponentFixture<MyComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MyComponent]
    }).compileComponents();

    fixture = TestBed.createComponent(MyComponent);
    component = fixture.componentInstance;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
```

### Creating New Integration Tests

```typescript
// *.integration.spec.ts
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';

describe('MyComponent (Integration)', () => {
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule, MyComponent]
    });
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should make real HTTP call', fakeAsync(() => {
    // Test with real HTTP
  }));
});
```

### Creating New E2E Tests

```typescript
// e2e/my-feature.e2e.spec.ts
import { test, expect } from '@playwright/test';

test.describe('My Feature', () => {
  test('should do something', async ({ page }) => {
    await page.goto('/my-feature');
    await expect(page.locator('h1')).toContainText('My Feature');
  });
});
```

## Best Practices

1. **Run tests before committing**: `npm test`
2. **Write tests for new features**: Aim for 80%+ coverage
3. **Use descriptive test names**: Clearly state what is being tested
4. **Keep tests isolated**: Each test should be independent
5. **Mock external dependencies**: Use spies and mocks
6. **Follow AAA pattern**: Arrange, Act, Assert
7. **Use page objects for E2E**: Keep tests maintainable

## Performance Tips

- **Use `--project` flag**: Run specific browsers only
- **Use `--workers` flag**: Control parallelization
- **Use `--headed` sparingly**: Headless is faster
- **Cache dependencies**: Use `npm ci` instead of `npm install`
- **Skip E2E locally**: Run in CI for comprehensive testing

## Getting Help

- **Documentation**: See `TEST-COVERAGE-REPORT.md`
- **E2E Guide**: See `e2e/README.md`
- **CI/CD Guide**: See `CI-CD-SETUP.md`
- **Full Summary**: See `TESTING-AND-CI-CD-SUMMARY.md`

## Summary

```
╔════════════════════════════════════════╗
║         TESTING QUICK START            ║
╠════════════════════════════════════════╣
║ 1. npm install (all directories)      ║
║ 2. npx playwright install              ║
║ 3. npm test (unit tests)               ║
║ 4. npm run test:integration            ║
║ 5. npm run test:e2e                    ║
╠════════════════════════════════════════╣
║ Total Test Suites: 150+                ║
║ Coverage: ~89%                         ║
║ Browsers: 6                            ║
╚════════════════════════════════════════╝
```

---

**Last Updated**: 2025-11-20
**Quick Start Version**: 1.0

🤖 Generated with [Claude Code](https://claude.com/claude-code)

Co-Authored-By: Claude <noreply@anthropic.com>
