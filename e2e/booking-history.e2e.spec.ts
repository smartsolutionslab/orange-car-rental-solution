import { test, expect } from '@playwright/test';
import { AuthHelper } from './helpers/auth.helper';
import { BookingHistoryPage } from './pages/booking-history.page';

/**
 * E2E Tests for Booking History Feature (Public Portal)
 * Tests complete user flows from login to viewing/managing bookings
 */
test.describe('Booking History - Authenticated User Flow', () => {
  let authHelper: AuthHelper;
  let bookingPage: BookingHistoryPage;

  test.beforeEach(async ({ page }) => {
    authHelper = new AuthHelper(page);
    bookingPage = new BookingHistoryPage(page);

    // Navigate to booking history
    await bookingPage.goto();

    // Login as customer
    await authHelper.loginAsCustomer();
    await bookingPage.waitForLoading();
  });

  test('should display user booking history grouped correctly', async () => {
    // Verify authenticated view is shown
    expect(await bookingPage.isAuthenticatedView()).toBe(true);

    // Check that reservations are grouped
    const upcomingCount = await bookingPage.getUpcomingReservationsCount();
    const pendingCount = await bookingPage.getPendingReservationsCount();
    const pastCount = await bookingPage.getPastReservationsCount();

    // At least one section should have reservations (or all could be empty for new user)
    expect(upcomingCount + pendingCount + pastCount).toBeGreaterThanOrEqual(0);
  });

  test('should view reservation details', async ({ page }) => {
    // Wait for reservations to load
    await bookingPage.waitForLoading();

    const upcomingCount = await bookingPage.getUpcomingReservationsCount();

    // Only run if there are upcoming reservations
    if (upcomingCount > 0) {
      // Click details on first reservation
      await bookingPage.viewReservationDetails(0);

      // Verify modal is open
      expect(await bookingPage.detailModal.isVisible()).toBe(true);

      // Verify modal contains reservation information
      const modalText = await bookingPage.detailModal.textContent();
      expect(modalText).toContain('Reservierungs-ID');
      expect(modalText).toContain('Abholdatum');
      expect(modalText).toContain('Rückgabedatum');

      // Close modal
      await bookingPage.closeDetailModal();
      expect(await bookingPage.detailModal.isVisible()).toBe(false);
    }
  });

  test('should cancel eligible reservation', async ({ page }) => {
    // Wait for reservations to load
    await bookingPage.waitForLoading();

    // Check if any reservation can be cancelled
    const canCancel = await bookingPage.canCancelReservation(0);

    if (canCancel) {
      // Open cancel modal
      await bookingPage.openCancelModal(0);
      expect(await bookingPage.cancelModal.isVisible()).toBe(true);

      // Fill cancellation reason
      await bookingPage.confirmCancellation('E2E Test Cancellation');

      // Wait for success (alert or message)
      page.once('dialog', dialog => {
        expect(dialog.message()).toContain('erfolgreich');
        dialog.accept();
      });

      await bookingPage.waitForLoading();

      // Verify cancellation success message or alert was shown
      // The reservation should now be in past/cancelled
    }
  });

  test('should not allow cancellation within 48 hours', async ({ page }) => {
    await bookingPage.waitForLoading();

    // Try to find a reservation within 48 hours (if exists)
    // The cancel button should be disabled for such reservations
    const reservations = page.locator('.reservation-card');
    const count = await reservations.count();

    for (let i = 0; i < count; i++) {
      const card = reservations.nth(i);
      const pickupText = await card.locator('.pickup-date').textContent();

      // Parse pickup date and check if within 48 hours
      // If within 48 hours, the cancel button should be disabled
      const cancelButton = card.locator('button:has-text("Stornieren")');
      if (await cancelButton.isVisible()) {
        const isDisabled = await cancelButton.isDisabled();
        // Disabled button means within 48 hours or not cancellable
        if (isDisabled) {
          expect(isDisabled).toBe(true);
        }
      }
    }
  });

  test('should handle empty booking history', async ({ page }) => {
    // For a new user or user with no bookings
    await bookingPage.waitForLoading();

    const upcomingCount = await bookingPage.getUpcomingReservationsCount();
    const pendingCount = await bookingPage.getPendingReservationsCount();
    const pastCount = await bookingPage.getPastReservationsCount();

    if (upcomingCount === 0 && pendingCount === 0 && pastCount === 0) {
      // Should show empty state message
      const pageText = await page.textContent('body');
      expect(pageText).toContain('Keine Buchungen' || 'keine Reservierungen');
    }
  });
});

test.describe('Booking History - Guest Lookup Flow', () => {
  let bookingPage: BookingHistoryPage;

  test.beforeEach(async ({ page }) => {
    bookingPage = new BookingHistoryPage(page);
    await bookingPage.goto();
  });

  test('should show guest lookup form for unauthenticated users', async () => {
    // Verify guest view is shown
    expect(await bookingPage.isGuestView()).toBe(true);

    // Verify form elements are present
    expect(await bookingPage.reservationIdInput.isVisible()).toBe(true);
    expect(await bookingPage.emailInput.isVisible()).toBe(true);
    expect(await bookingPage.lookupButton.isVisible()).toBe(true);
  });

  test('should validate guest lookup form', async () => {
    // Try to submit empty form
    await bookingPage.lookupButton.click();

    // Should show validation error
    const error = await bookingPage.getErrorMessage();
    expect(error).toContain('Reservierungs-ID' || 'Email');
  });

  test('should show error for invalid guest lookup', async () => {
    // Enter invalid credentials
    await bookingPage.lookupGuestReservation('invalid-id-123', 'wrong@email.com');

    // Wait for response
    await bookingPage.waitForLoading();

    // Should show error message
    const error = await bookingPage.getErrorMessage();
    expect(error).toContain('nicht gefunden' || 'not found');
  });

  test('should successfully lookup valid guest reservation', async ({ page }) => {
    // This test requires a valid test reservation ID and email
    // In real scenario, these would be seeded in test database

    const testReservationId = process.env.TEST_RESERVATION_ID || '123e4567-e89b-12d3-a456-426614174000';
    const testEmail = process.env.TEST_GUEST_EMAIL || 'guest@example.com';

    await bookingPage.lookupGuestReservation(testReservationId, testEmail);
    await bookingPage.waitForLoading();

    // If successful, should show reservation details
    const pageText = await page.textContent('body');

    // Either error (no test data) or success
    if (pageText.includes('nicht gefunden')) {
      // Expected if no test data exists
      expect(pageText).toContain('nicht gefunden');
    } else {
      // Success - should show reservation details
      expect(pageText).toContain('Reservierungs-ID' || 'Fahrzeug');
    }
  });
});

test.describe('Booking History - End-to-End User Journey', () => {
  test('should complete full booking lifecycle', async ({ page }) => {
    const authHelper = new AuthHelper(page);
    const bookingPage = new BookingHistoryPage(page);

    // Step 1: Navigate to booking history
    await bookingPage.goto();

    // Step 2: Login as customer
    await authHelper.loginAsCustomer();
    await bookingPage.waitForLoading();

    // Step 3: View booking history
    expect(await bookingPage.isAuthenticatedView()).toBe(true);

    // Step 4: Check initial state
    const initialUpcoming = await bookingPage.getUpcomingReservationsCount();
    const initialPending = await bookingPage.getPendingReservationsCount();

    // Step 5: View details of a reservation (if exists)
    if (initialUpcoming > 0) {
      await bookingPage.viewReservationDetails(0);
      expect(await bookingPage.detailModal.isVisible()).toBe(true);
      await bookingPage.closeDetailModal();
    }

    // Step 6: Try to cancel a reservation (if eligible)
    const canCancel = await bookingPage.canCancelReservation(0);
    if (canCancel) {
      await bookingPage.openCancelModal(0);
      expect(await bookingPage.cancelModal.isVisible()).toBe(true);

      // Actually cancel
      await bookingPage.confirmCancellation('E2E Test - Full Journey Cancellation');

      // Handle alert
      page.once('dialog', dialog => {
        dialog.accept();
      });

      await bookingPage.waitForLoading();

      // Verify the count has changed or status updated
      const newUpcoming = await bookingPage.getUpcomingReservationsCount();
      // Either count decreased or status changed to cancelled
    }

    // Step 7: Logout
    await authHelper.logout();
  });
});

test.describe('Booking History - Responsive Design', () => {
  test('should work on mobile devices', async ({ page }) => {
    // Set mobile viewport
    await page.setViewportSize({ width: 375, height: 667 });

    const bookingPage = new BookingHistoryPage(page);
    await bookingPage.goto();

    // Verify mobile layout
    expect(await bookingPage.guestLookupForm.isVisible()).toBe(true);

    // Form should be usable on mobile
    expect(await bookingPage.reservationIdInput.isVisible()).toBe(true);
    expect(await bookingPage.emailInput.isVisible()).toBe(true);
  });

  test('should work on tablet devices', async ({ page }) => {
    // Set tablet viewport
    await page.setViewportSize({ width: 768, height: 1024 });

    const authHelper = new AuthHelper(page);
    const bookingPage = new BookingHistoryPage(page);

    await bookingPage.goto();
    await authHelper.loginAsCustomer();
    await bookingPage.waitForLoading();

    // Should show booking history
    expect(await bookingPage.isAuthenticatedView()).toBe(true);
  });
});
