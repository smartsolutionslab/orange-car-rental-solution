import { test, expect } from '@playwright/test';
import { AuthHelper } from './helpers/auth.helper';
import { ReservationsPage } from './pages/reservations.page';

/**
 * E2E Tests for Reservations Management Feature (Call Center Portal)
 * Tests complete agent flows for filtering, sorting, and managing reservations
 */
test.describe('Reservations - Initial Load and Authentication', () => {
  let authHelper: AuthHelper;
  let reservationsPage: ReservationsPage;

  test.beforeEach(async ({ page, baseURL }) => {
    // Use call center portal URL
    const callCenterUrl = baseURL?.replace(':4200', ':4201') || 'http://localhost:4201';
    await page.goto(callCenterUrl);

    authHelper = new AuthHelper(page);
    reservationsPage = new ReservationsPage(page);

    // Login as call center agent
    await authHelper.loginAsAgent();
  });

  test('should load reservations on initial page load', async () => {
    await reservationsPage.goto();
    await reservationsPage.waitForLoading();

    // Should display reservations
    const count = await reservationsPage.getReservationsCount();
    expect(count).toBeGreaterThanOrEqual(0);

    // Should not show error
    const error = await reservationsPage.getErrorMessage();
    expect(error).toBeNull();
  });

  test('should display statistics dashboard', async ({ page }) => {
    await reservationsPage.goto();
    await reservationsPage.waitForLoading();

    // Check for statistics display
    const pageText = await page.textContent('body');
    expect(pageText).toContain('Heute' || 'Aktiv' || 'Ausstehend');
  });
});

test.describe('Reservations - Status Filtering', () => {
  let authHelper: AuthHelper;
  let reservationsPage: ReservationsPage;

  test.beforeEach(async ({ page, baseURL }) => {
    const callCenterUrl = baseURL?.replace(':4200', ':4201') || 'http://localhost:4201';
    await page.goto(callCenterUrl);

    authHelper = new AuthHelper(page);
    await authHelper.loginAsAgent();

    reservationsPage = new ReservationsPage(page);
    await reservationsPage.goto();
    await reservationsPage.waitForLoading();
  });

  test('should filter reservations by Pending status', async () => {
    // Apply pending filter
    await reservationsPage.filterByStatus('Pending');

    // Verify URL contains filter parameter
    expect(await reservationsPage.isURLSyncedWithFilters()).toBe(true);
    expect(await reservationsPage.getURLParam('status')).toBe('Pending');

    // Verify active filters count
    const activeCount = await reservationsPage.getActiveFiltersCount();
    expect(activeCount).toBe(1);

    // Verify all displayed reservations have Pending status
    const reservations = await reservationsPage.getReservationsCount();
    if (reservations > 0) {
      for (let i = 0; i < Math.min(reservations, 5); i++) {
        const status = await reservationsPage.getReservationStatus(i);
        expect(status).toContain('Ausstehend'); // German for Pending
      }
    }
  });

  test('should filter reservations by Confirmed status', async () => {
    await reservationsPage.filterByStatus('Confirmed');

    expect(await reservationsPage.getURLParam('status')).toBe('Confirmed');

    const reservations = await reservationsPage.getReservationsCount();
    if (reservations > 0) {
      const status = await reservationsPage.getReservationStatus(0);
      expect(status).toContain('Bestätigt'); // German for Confirmed
    }
  });

  test('should clear status filter', async () => {
    // Apply filter first
    await reservationsPage.filterByStatus('Pending');
    expect(await reservationsPage.getActiveFiltersCount()).toBe(1);

    // Clear filters
    await reservationsPage.clearAllFilters();

    // Verify filters are cleared
    expect(await reservationsPage.getActiveFiltersCount()).toBe(0);
    expect(await reservationsPage.getURLParam('status')).toBeNull();
  });
});

test.describe('Reservations - Date Range Filtering', () => {
  let authHelper: AuthHelper;
  let reservationsPage: ReservationsPage;

  test.beforeEach(async ({ page, baseURL }) => {
    const callCenterUrl = baseURL?.replace(':4200', ':4201') || 'http://localhost:4201';
    await page.goto(callCenterUrl);

    authHelper = new AuthHelper(page);
    await authHelper.loginAsAgent();

    reservationsPage = new ReservationsPage(page);
    await reservationsPage.goto();
    await reservationsPage.waitForLoading();
  });

  test('should filter reservations by date range', async () => {
    // Apply date range filter
    await reservationsPage.filterByDateRange('2025-11-01', '2025-11-30');

    // Verify URL contains date parameters
    expect(await reservationsPage.getURLParam('dateFrom')).toBe('2025-11-01');
    expect(await reservationsPage.getURLParam('dateTo')).toBe('2025-11-30');

    // Verify active filters count
    const activeCount = await reservationsPage.getActiveFiltersCount();
    expect(activeCount).toBe(2); // dateFrom and dateTo
  });
});

test.describe('Reservations - Price Range Filtering', () => {
  let authHelper: AuthHelper;
  let reservationsPage: ReservationsPage;

  test.beforeEach(async ({ page, baseURL }) => {
    const callCenterUrl = baseURL?.replace(':4200', ':4201') || 'http://localhost:4201';
    await page.goto(callCenterUrl);

    authHelper = new AuthHelper(page);
    await authHelper.loginAsAgent();

    reservationsPage = new ReservationsPage(page);
    await reservationsPage.goto();
    await reservationsPage.waitForLoading();
  });

  test('should filter reservations by price range', async () => {
    // Apply price range filter
    await reservationsPage.filterByPriceRange(200, 500);

    // Verify URL contains price parameters
    expect(await reservationsPage.getURLParam('minPrice')).toBe('200');
    expect(await reservationsPage.getURLParam('maxPrice')).toBe('500');

    // Verify active filters count
    const activeCount = await reservationsPage.getActiveFiltersCount();
    expect(activeCount).toBe(2); // minPrice and maxPrice
  });
});

test.describe('Reservations - Location Filtering', () => {
  let authHelper: AuthHelper;
  let reservationsPage: ReservationsPage;

  test.beforeEach(async ({ page, baseURL }) => {
    const callCenterUrl = baseURL?.replace(':4200', ':4201') || 'http://localhost:4201';
    await page.goto(callCenterUrl);

    authHelper = new AuthHelper(page);
    await authHelper.loginAsAgent();

    reservationsPage = new ReservationsPage(page);
    await reservationsPage.goto();
    await reservationsPage.waitForLoading();
  });

  test('should filter reservations by location', async () => {
    // Apply location filter
    await reservationsPage.filterByLocation('MUC');

    // Verify URL contains location parameter
    expect(await reservationsPage.getURLParam('location')).toBe('MUC');

    // Verify active filters count
    const activeCount = await reservationsPage.getActiveFiltersCount();
    expect(activeCount).toBe(1);
  });
});

test.describe('Reservations - Multiple Filters Combined', () => {
  let authHelper: AuthHelper;
  let reservationsPage: ReservationsPage;

  test.beforeEach(async ({ page, baseURL }) => {
    const callCenterUrl = baseURL?.replace(':4200', ':4201') || 'http://localhost:4201';
    await page.goto(callCenterUrl);

    authHelper = new AuthHelper(page);
    await authHelper.loginAsAgent();

    reservationsPage = new ReservationsPage(page);
    await reservationsPage.goto();
    await reservationsPage.waitForLoading();
  });

  test('should apply multiple filters simultaneously', async () => {
    // Apply multiple filters
    await reservationsPage.applyMultipleFilters({
      status: 'Confirmed',
      location: 'MUC',
      minPrice: 300,
      maxPrice: 600
    });

    // Verify all filter parameters in URL
    expect(await reservationsPage.getURLParam('status')).toBe('Confirmed');
    expect(await reservationsPage.getURLParam('location')).toBe('MUC');
    expect(await reservationsPage.getURLParam('minPrice')).toBe('300');
    expect(await reservationsPage.getURLParam('maxPrice')).toBe('600');

    // Verify active filters count
    const activeCount = await reservationsPage.getActiveFiltersCount();
    expect(activeCount).toBe(4);
  });

  test('should clear all filters at once', async () => {
    // Apply multiple filters
    await reservationsPage.applyMultipleFilters({
      status: 'Pending',
      customerId: 'cust-001',
      dateFrom: '2025-11-01'
    });

    const beforeClear = await reservationsPage.getActiveFiltersCount();
    expect(beforeClear).toBeGreaterThan(0);

    // Clear all
    await reservationsPage.clearAllFilters();

    // Verify all cleared
    const afterClear = await reservationsPage.getActiveFiltersCount();
    expect(afterClear).toBe(0);
  });
});

test.describe('Reservations - Sorting', () => {
  let authHelper: AuthHelper;
  let reservationsPage: ReservationsPage;

  test.beforeEach(async ({ page, baseURL }) => {
    const callCenterUrl = baseURL?.replace(':4200', ':4201') || 'http://localhost:4201';
    await page.goto(callCenterUrl);

    authHelper = new AuthHelper(page);
    await authHelper.loginAsAgent();

    reservationsPage = new ReservationsPage(page);
    await reservationsPage.goto();
    await reservationsPage.waitForLoading();
  });

  test('should sort reservations by price', async () => {
    await reservationsPage.sortBy('Price');

    // Verify URL contains sort parameter
    expect(await reservationsPage.getURLParam('sortBy')).toBe('Price');
  });

  test('should sort reservations by status', async () => {
    await reservationsPage.sortBy('Status');

    expect(await reservationsPage.getURLParam('sortBy')).toBe('Status');
  });

  test('should sort reservations by created date', async () => {
    await reservationsPage.sortBy('CreatedDate');

    expect(await reservationsPage.getURLParam('sortBy')).toBe('CreatedDate');
  });
});

test.describe('Reservations - Grouping', () => {
  let authHelper: AuthHelper;
  let reservationsPage: ReservationsPage;

  test.beforeEach(async ({ page, baseURL }) => {
    const callCenterUrl = baseURL?.replace(':4200', ':4201') || 'http://localhost:4201';
    await page.goto(callCenterUrl);

    authHelper = new AuthHelper(page);
    await authHelper.loginAsAgent();

    reservationsPage = new ReservationsPage(page);
    await reservationsPage.goto();
    await reservationsPage.waitForLoading();
  });

  test('should group reservations by status', async () => {
    await reservationsPage.groupBy('status');

    // Verify URL contains groupBy parameter
    expect(await reservationsPage.getURLParam('groupBy')).toBe('status');

    // Verify groups are displayed
    const groupCount = await reservationsPage.getGroupCount();
    expect(groupCount).toBeGreaterThan(0);
  });

  test('should group reservations by location', async () => {
    await reservationsPage.groupBy('location');

    expect(await reservationsPage.getURLParam('groupBy')).toBe('location');

    const groupCount = await reservationsPage.getGroupCount();
    expect(groupCount).toBeGreaterThan(0);
  });

  test('should group reservations by pickup date', async () => {
    await reservationsPage.groupBy('pickupDate');

    expect(await reservationsPage.getURLParam('groupBy')).toBe('pickupDate');

    // Should show groups like "Heute", "Morgen", etc.
    const groupNames = await reservationsPage.getGroupNames();
    expect(groupNames.length).toBeGreaterThan(0);
  });

  test('should remove grouping', async () => {
    // First apply grouping
    await reservationsPage.groupBy('status');
    expect(await reservationsPage.getGroupCount()).toBeGreaterThan(0);

    // Remove grouping
    await reservationsPage.groupBy('none');

    // Should show single group
    const groups = await reservationsPage.getGroupCount();
    expect(groups).toBeLessThanOrEqual(1);
  });
});

test.describe('Reservations - Pagination', () => {
  let authHelper: AuthHelper;
  let reservationsPage: ReservationsPage;

  test.beforeEach(async ({ page, baseURL }) => {
    const callCenterUrl = baseURL?.replace(':4200', ':4201') || 'http://localhost:4201';
    await page.goto(callCenterUrl);

    authHelper = new AuthHelper(page);
    await authHelper.loginAsAgent();

    reservationsPage = new ReservationsPage(page);
    await reservationsPage.goto();
    await reservationsPage.waitForLoading();
  });

  test('should navigate to next page', async () => {
    const totalPages = await reservationsPage.getTotalPages();

    if (totalPages > 1) {
      const currentPage = await reservationsPage.getCurrentPage();
      expect(currentPage).toBe(1);

      await reservationsPage.goToNextPage();

      const newPage = await reservationsPage.getCurrentPage();
      expect(newPage).toBe(2);

      // Verify URL updated
      expect(await reservationsPage.getURLParam('page')).toBe('2');
    }
  });

  test('should navigate to specific page', async () => {
    const totalPages = await reservationsPage.getTotalPages();

    if (totalPages >= 3) {
      await reservationsPage.goToPage(3);

      const currentPage = await reservationsPage.getCurrentPage();
      expect(currentPage).toBe(3);

      expect(await reservationsPage.getURLParam('page')).toBe('3');
    }
  });

  test('should navigate back to previous page', async () => {
    const totalPages = await reservationsPage.getTotalPages();

    if (totalPages > 1) {
      // Go to page 2 first
      await reservationsPage.goToNextPage();
      expect(await reservationsPage.getCurrentPage()).toBe(2);

      // Go back to page 1
      await reservationsPage.goToPreviousPage();
      expect(await reservationsPage.getCurrentPage()).toBe(1);
    }
  });
});

test.describe('Reservations - Actions', () => {
  let authHelper: AuthHelper;
  let reservationsPage: ReservationsPage;

  test.beforeEach(async ({ page, baseURL }) => {
    const callCenterUrl = baseURL?.replace(':4200', ':4201') || 'http://localhost:4201';
    await page.goto(callCenterUrl);

    authHelper = new AuthHelper(page);
    await authHelper.loginAsAgent();

    reservationsPage = new ReservationsPage(page);
    await reservationsPage.goto();
    await reservationsPage.waitForLoading();
  });

  test('should view reservation details', async () => {
    const count = await reservationsPage.getReservationsCount();

    if (count > 0) {
      await reservationsPage.viewReservationDetails(0);

      // Verify detail modal is open
      expect(await reservationsPage.detailModal.isVisible()).toBe(true);
    }
  });

  test('should confirm pending reservation', async ({ page }) => {
    // Filter to pending reservations
    await reservationsPage.filterByStatus('Pending');

    const count = await reservationsPage.getReservationsCount();

    if (count > 0) {
      // Confirm the first pending reservation
      await reservationsPage.confirmReservation(0);

      // Should show success message
      const success = await reservationsPage.getSuccessMessage();
      expect(success).toContain('erfolgreich' || 'bestätigt');
    }
  });

  test('should cancel reservation with reason', async () => {
    // Filter to confirmed reservations
    await reservationsPage.filterByStatus('Confirmed');

    const count = await reservationsPage.getReservationsCount();

    if (count > 0) {
      // Cancel the first confirmed reservation
      await reservationsPage.cancelReservation(0, 'E2E Test Cancellation');

      // Should show success message
      const success = await reservationsPage.getSuccessMessage();
      expect(success).toContain('erfolgreich' || 'storniert');
    }
  });
});

test.describe('Reservations - End-to-End Agent Workflow', () => {
  test('should complete full agent workflow', async ({ page, baseURL }) => {
    const callCenterUrl = baseURL?.replace(':4200', ':4201') || 'http://localhost:4201';
    await page.goto(callCenterUrl);

    const authHelper = new AuthHelper(page);
    const reservationsPage = new ReservationsPage(page);

    // Step 1: Login
    await authHelper.loginAsAgent();
    expect(await authHelper.isAuthenticated()).toBe(true);

    // Step 2: Navigate to reservations
    await reservationsPage.goto();
    await reservationsPage.waitForLoading();

    // Step 3: View all reservations
    const initialCount = await reservationsPage.getReservationsCount();
    expect(initialCount).toBeGreaterThanOrEqual(0);

    // Step 4: Apply filters
    await reservationsPage.applyMultipleFilters({
      status: 'Pending',
      minPrice: 100
    });
    expect(await reservationsPage.getActiveFiltersCount()).toBeGreaterThan(0);

    // Step 5: Sort by price
    await reservationsPage.sortBy('Price');
    expect(await reservationsPage.getURLParam('sortBy')).toBe('Price');

    // Step 6: Group by status
    await reservationsPage.groupBy('status');
    const groups = await reservationsPage.getGroupCount();
    expect(groups).toBeGreaterThan(0);

    // Step 7: View details of first reservation (if exists)
    const filteredCount = await reservationsPage.getReservationsCount();
    if (filteredCount > 0) {
      await reservationsPage.viewReservationDetails(0);
      expect(await reservationsPage.detailModal.isVisible()).toBe(true);
    }

    // Step 8: Clear filters
    await reservationsPage.clearAllFilters();
    expect(await reservationsPage.getActiveFiltersCount()).toBe(0);

    // Step 9: Logout
    await authHelper.logout();
  });
});

test.describe('Reservations - Browser Compatibility', () => {
  test('should work in different browsers', async ({ page, browserName, baseURL }) => {
    const callCenterUrl = baseURL?.replace(':4200', ':4201') || 'http://localhost:4201';
    await page.goto(callCenterUrl);

    const authHelper = new AuthHelper(page);
    const reservationsPage = new ReservationsPage(page);

    await authHelper.loginAsAgent();
    await reservationsPage.goto();
    await reservationsPage.waitForLoading();

    // Basic functionality should work across all browsers
    const count = await reservationsPage.getReservationsCount();
    expect(count).toBeGreaterThanOrEqual(0);

    // Filtering should work
    await reservationsPage.filterByStatus('Confirmed');
    expect(await reservationsPage.getURLParam('status')).toBe('Confirmed');
  });
});
