# Frontend Testing Documentation

## Test Results

**Status**: All tests passing ✅

### Summary
- **Total Tests**: 76
- **Passed**: 76 (100%)
- **Failed**: 0
- **Execution Time**: ~0.6 seconds

### Code Coverage
- **Statements**: 87.5% (224/256)
- **Branches**: 51.72% (45/87)
- **Functions**: 85.18% (46/54)
- **Lines**: 86.49% (205/237)

## Test Structure

### Component Tests
1. **App Component** (2 tests)
   - Component creation
   - Layout and router outlet rendering

2. **VehicleListComponent** (35+ tests)
   - Search functionality
   - Vehicle booking navigation
   - State management
   - Error handling

3. **BookingComponent** (15+ tests)
   - Form validation
   - Reservation creation
   - Error handling

4. **ConfirmationComponent** (10+ tests)
   - Reservation display
   - Error handling

### Service Tests
1. **VehicleService** (15+ tests)
   - Vehicle search with various filters
   - HTTP request handling
   - Error scenarios

2. **ReservationService** (10+ tests)
   - Guest reservations
   - Reservation retrieval
   - Error handling

3. **LocationService** (3 tests)
   - Location listing
   - HTTP integration

4. **ConfigService** (1 test)
   - Configuration management

## How to Run Tests

### Prerequisites
- Node.js v24+
- npm or pnpm
- Puppeteer (bundled Chromium)

### Running Tests

#### Run all tests once:
```bash
cd src/frontend/apps/public-portal
npm test
```

#### Run tests with coverage:
```bash
npm test -- --code-coverage
```

#### Run specific test file:
```bash
npx ng test --include='**/vehicle.service.spec.ts'
```

#### Run tests in watch mode:
```bash
npx ng test
```

## Test Configuration

### Karma Configuration
Tests use Karma with:
- **Test Framework**: Jasmine 5.9
- **Browser**: ChromeHeadlessNoSandbox (via Puppeteer)
- **Coverage**: Karma Coverage Reporter
- **Reporters**: Progress, Jasmine HTML Reporter

**Configuration File**: `karma.conf.js`

### Key Features
- **Puppeteer Integration**: Uses bundled Chromium for headless testing
- **German Locale Support**: Registered for DecimalPipe testing
- **HTTP Mocking**: Uses `provideHttpClientTesting()` for service tests
- **Router Testing**: Uses `provideRouter([])` for routing tests

## Test Coverage Details

### Well-Covered Areas
- **VehicleService**: All search scenarios, filters, pagination
- **ReservationService**: Guest reservations, retrieval, error handling
- **VehicleListComponent**: Search, booking navigation, state management
- **BookingComponent**: Form validation, reservation creation

### Areas for Improvement
- **Branch Coverage** (51.72%): Could add more conditional path tests
- **Edge Cases**: More boundary condition testing
- **Integration Tests**: End-to-end user flows

## Common Test Patterns

### Service Testing
```typescript
it('should search vehicles successfully', () => {
  const query = { locationCode: 'BER-HBF' };

  service.searchVehicles(query).subscribe(result => {
    expect(result.vehicles.length).toBeGreaterThan(0);
  });

  const req = httpMock.expectOne(req =>
    req.url === `${apiUrl}/api/vehicles`
  );
  req.flush(mockSearchResult);
});
```

### Component Testing
```typescript
it('should handle search', () => {
  const query = { locationCode: 'BER-HBF' };

  component['onSearch'](query);

  expect(vehicleService.searchVehicles)
    .toHaveBeenCalledWith(query);
  expect(component['vehicles']().length).toBe(1);
});
```

## Troubleshooting

### Chrome Not Found
If you get "Cannot find Chrome" error:
- Tests now use Puppeteer's bundled Chromium
- Ensure `puppeteer` is installed: `npm install --save-dev puppeteer`

### Locale Errors
If you get "Missing locale data" errors:
- German locale is registered in `vehicle-list.component.spec.ts`
- Other components may need locale registration if using DecimalPipe

### Provider Errors
If you get "No provider found" errors:
- Add `provideHttpClient()` and `provideHttpClientTesting()` to TestBed
- Add `provideRouter([])` for components using routing
- Mock services that are injected by child components (e.g., `LocationService` for `VehicleSearchComponent`)
  ```typescript
  const locationServiceSpy = jasmine.createSpyObj('LocationService', ['getAllLocations', 'getLocationByCode']);
  providers: [
    { provide: LocationService, useValue: locationServiceSpy },
    // ... other providers
  ]
  locationServiceSpy.getAllLocations.and.returnValue(of([]));
  ```

## Next Steps

### Recommended Improvements
1. **E2E Tests**: Add Playwright tests for complete user flows
2. **Visual Regression**: Add screenshot comparison tests
3. **Performance Tests**: Measure component render times
4. **Accessibility Tests**: Add a11y testing with axe-core
5. **Increase Branch Coverage**: Add more conditional path tests

### E2E Test Candidates
- Complete booking flow (search → select → book → confirm)
- Navigation between pages
- Form validation end-to-end
- Error handling flows

## Test Maintenance

### Adding New Tests
1. Create `.spec.ts` file next to component/service
2. Import testing utilities and dependencies
3. Configure TestBed with required providers
4. Write descriptive test cases
5. Run tests and verify coverage

### Updating Existing Tests
- Keep tests focused and isolated
- Use meaningful test descriptions
- Mock external dependencies
- Test both success and error paths

## CI/CD Integration

### Running in CI
```bash
# In CI pipeline
export CHROME_BIN=/path/to/chromium
npm test -- --watch=false --browsers=ChromeHeadlessNoSandbox --code-coverage
```

### Coverage Reports
- HTML report: `coverage/public-portal/index.html`
- LCOV format: `coverage/public-portal/lcov.info`

## Resources

- [Angular Testing Guide](https://angular.dev/guide/testing)
- [Jasmine Documentation](https://jasmine.github.io/)
- [Karma Documentation](https://karma-runner.github.io/)
- [Testing Best Practices](https://angular.dev/guide/testing-best-practices)
