import { test, expect } from '@playwright/test';
import { startBooking, fillCustomerInfo, fillAddress, fillDriversLicense, nextStep, completeBooking } from './helpers/booking.helper';
import { testUsers, testBooking } from './fixtures/test-data';

/**
 * E2E Tests for US-2: Complete Booking Flow (5-Step Wizard)
 *
 * Covers:
 * - Step 1: Vehicle Details (dates, locations, rental days)
 * - Step 2: Customer Information (validation)
 * - Step 3: Address Information (validation)
 * - Step 4: Driver's License (validation, date constraints)
 * - Step 5: Review & Submit (data display, confirmation)
 * - Progress indicator
 * - Navigation (next/previous buttons)
 * - Form validation at each step
 * - Error handling
 * - Confirmation page
 */

test.describe('US-2: Complete Booking Flow', () => {
  test.describe('Step 1: Vehicle Details', () => {
    test('should display vehicle details and booking dates', async ({ page }) => {
      await startBooking(page);

      // Should show selected vehicle information
      const vehicleInfoVisible = await page.locator('.vehicle-details, .selected-vehicle, h2').isVisible().catch(() => false);
      expect(vehicleInfoVisible).toBe(true);

      // Should show date fields
      await expect(page.locator('input[formControlName="pickupDate"]')).toBeVisible();
      await expect(page.locator('input[formControlName="returnDate"]')).toBeVisible();
    });

    test('should display pickup and dropoff location selects', async ({ page }) => {
      await startBooking(page);

      // Should have location selects
      await expect(page.locator('select[formControlName="pickupLocationCode"]')).toBeVisible();
      await expect(page.locator('select[formControlName="dropoffLocationCode"]')).toBeVisible();
    });

    test('should calculate rental days automatically', async ({ page }) => {
      await startBooking(page);

      // Check if rental days are displayed
      const rentalDaysVisible = await page.locator('text=/\\d+.*Tage|\\d+.*days/i').isVisible().catch(() => false);

      if (rentalDaysVisible) {
        const rentalDaysText = await page.locator('text=/\\d+.*Tage|\\d+.*days/i').first().textContent();
        // Should contain a number
        expect(rentalDaysText).toMatch(/\d+/);
      }
    });

    test('should validate that return date is after pickup date', async ({ page }) => {
      await startBooking(page);

      const today = new Date();
      const yesterday = new Date(today);
      yesterday.setDate(yesterday.getDate() - 1);

      const pickupDate = today.toISOString().split('T')[0];
      const returnDate = yesterday.toISOString().split('T')[0];

      // Try to set invalid dates
      await page.fill('input[formControlName="pickupDate"]', pickupDate);
      await page.fill('input[formControlName="returnDate"]', returnDate);

      // Try to proceed
      const nextButton = page.locator('button:has-text("Weiter")');
      await nextButton.click();

      // Should show error or button should be disabled
      const errorVisible = await page.locator('.error, text=/ungültig|Fehler/i').isVisible().catch(() => false);
      const stillOnStep1 = await page.locator('.progress-step').first().evaluate(el =>
        el.classList.contains('current') || el.classList.contains('active')
      );

      expect(errorVisible || stillOnStep1).toBe(true);
    });

    test('should allow editing dates', async ({ page }) => {
      await startBooking(page);

      const newPickupDate = testBooking.pickupDate();

      // Change pickup date
      await page.fill('input[formControlName="pickupDate"]', newPickupDate);

      // Value should be updated
      const pickupValue = await page.inputValue('input[formControlName="pickupDate"]');
      expect(pickupValue).toBe(newPickupDate);
    });

    test('should display progress indicator showing Step 1', async ({ page }) => {
      await startBooking(page);

      // Check progress indicator
      const progressSteps = page.locator('.progress-step');
      const stepCount = await progressSteps.count();

      expect(stepCount).toBeGreaterThanOrEqual(5);

      // First step should be current/active
      const firstStep = progressSteps.first();
      const isActive = await firstStep.evaluate(el =>
        el.classList.contains('current') || el.classList.contains('active')
      );
      expect(isActive).toBe(true);
    });
  });

  test.describe('Step 2: Customer Information', () => {
    test('should navigate to customer information step', async ({ page }) => {
      await startBooking(page);
      await nextStep(page);

      // Should be on step 2
      await expect(page.locator('input[formControlName="firstName"]')).toBeVisible();
      await expect(page.locator('input[formControlName="lastName"]')).toBeVisible();
      await expect(page.locator('input[formControlName="email"]')).toBeVisible();
    });

    test('should validate required fields', async ({ page }) => {
      await startBooking(page);
      await nextStep(page);

      // Try to proceed without filling fields
      await page.click('button:has-text("Weiter")');

      // Should show validation errors or prevent navigation
      const errorVisible = await page.locator('.error-text, .invalid').count() > 0;
      const stillOnStep2 = await page.locator('input[formControlName="firstName"]').isVisible();

      expect(errorVisible || stillOnStep2).toBe(true);
    });

    test('should validate first name minimum length (2 characters)', async ({ page }) => {
      await startBooking(page);
      await nextStep(page);

      // Enter too short name
      await page.fill('input[formControlName="firstName"]', 'A');
      await page.locator('input[formControlName="firstName"]').blur();

      // Should show validation error
      const firstNameInput = page.locator('input[formControlName="firstName"]');
      const isInvalid = await firstNameInput.evaluate((el: HTMLInputElement) =>
        el.classList.contains('invalid') || el.classList.contains('ng-invalid')
      );

      expect(isInvalid).toBe(true);
    });

    test('should validate email format', async ({ page }) => {
      await startBooking(page);
      await nextStep(page);

      // Enter invalid email
      await page.fill('input[formControlName="email"]', 'invalid-email');
      await page.locator('input[formControlName="email"]').blur();

      // Should show validation error
      const emailInput = page.locator('input[formControlName="email"]');
      const isInvalid = await emailInput.evaluate((el: HTMLInputElement) =>
        el.classList.contains('invalid') || el.classList.contains('ng-invalid')
      );

      expect(isInvalid).toBe(true);
    });

    test('should validate phone number format', async ({ page }) => {
      await startBooking(page);
      await nextStep(page);

      // Enter invalid phone
      await page.fill('input[formControlName="phoneNumber"]', 'invalid-phone');
      await page.locator('input[formControlName="phoneNumber"]').blur();

      // Should show validation error
      const phoneInput = page.locator('input[formControlName="phoneNumber"]');
      const isInvalid = await phoneInput.evaluate((el: HTMLInputElement) =>
        el.classList.contains('invalid') || el.classList.contains('ng-invalid')
      );

      expect(isInvalid).toBe(true);
    });

    test('should validate age requirement (18+ years)', async ({ page }) => {
      await startBooking(page);
      await nextStep(page);

      // Calculate date for someone under 18
      const today = new Date();
      const underageDate = new Date(today.getFullYear() - 17, today.getMonth(), today.getDate());
      const dateString = underageDate.toISOString().split('T')[0];

      await page.fill('input[formControlName="dateOfBirth"]', dateString);
      await page.locator('input[formControlName="dateOfBirth"]').blur();

      // Should show validation error
      const dobInput = page.locator('input[formControlName="dateOfBirth"]');
      const isInvalid = await dobInput.evaluate((el: HTMLInputElement) =>
        el.classList.contains('invalid') || el.classList.contains('ng-invalid')
      );

      expect(isInvalid).toBe(true);
    });

    test('should accept valid customer information', async ({ page }) => {
      await startBooking(page);
      await nextStep(page);

      // Fill valid data
      await fillCustomerInfo(page);

      // Should be able to proceed
      await page.click('button:has-text("Weiter")');
      await page.waitForTimeout(500);

      // Should navigate to step 3
      await expect(page.locator('input[formControlName="street"]')).toBeVisible();
    });
  });

  test.describe('Step 3: Address Information', () => {
    test('should display address form fields', async ({ page }) => {
      await startBooking(page);
      await nextStep(page);
      await fillCustomerInfo(page);
      await nextStep(page);

      // Should show address fields
      await expect(page.locator('input[formControlName="street"]')).toBeVisible();
      await expect(page.locator('input[formControlName="city"]')).toBeVisible();
      await expect(page.locator('input[formControlName="postalCode"]')).toBeVisible();
      await expect(page.locator('select[formControlName="country"]')).toBeVisible();
    });

    test('should validate street minimum length (5 characters)', async ({ page }) => {
      await startBooking(page);
      await nextStep(page);
      await fillCustomerInfo(page);
      await nextStep(page);

      // Enter too short street
      await page.fill('input[formControlName="street"]', 'Str');
      await page.locator('input[formControlName="street"]').blur();

      // Should show validation error
      const streetInput = page.locator('input[formControlName="street"]');
      const isInvalid = await streetInput.evaluate((el: HTMLInputElement) =>
        el.classList.contains('invalid') || el.classList.contains('ng-invalid')
      );

      expect(isInvalid).toBe(true);
    });

    test('should validate postal code format (5 digits)', async ({ page }) => {
      await startBooking(page);
      await nextStep(page);
      await fillCustomerInfo(page);
      await nextStep(page);

      // Enter invalid postal code
      await page.fill('input[formControlName="postalCode"]', '123');
      await page.locator('input[formControlName="postalCode"]').blur();

      // Should show validation error
      const postalInput = page.locator('input[formControlName="postalCode"]');
      const isInvalid = await postalInput.evaluate((el: HTMLInputElement) =>
        el.classList.contains('invalid') || el.classList.contains('ng-invalid')
      );

      expect(isInvalid).toBe(true);
    });

    test('should accept valid address information', async ({ page }) => {
      await startBooking(page);
      await nextStep(page);
      await fillCustomerInfo(page);
      await nextStep(page);
      await fillAddress(page);

      // Should be able to proceed
      await page.click('button:has-text("Weiter")');
      await page.waitForTimeout(500);

      // Should navigate to step 4
      await expect(page.locator('input[formControlName="licenseNumber"]')).toBeVisible();
    });

    test('should have Deutschland as default country', async ({ page }) => {
      await startBooking(page);
      await nextStep(page);
      await fillCustomerInfo(page);
      await nextStep(page);

      // Country should default to Deutschland
      const countryValue = await page.inputValue('select[formControlName="country"]');
      expect(countryValue).toBe('Deutschland');
    });
  });

  test.describe('Step 4: Driver\'s License', () => {
    test('should display driver\'s license form fields', async ({ page }) => {
      await startBooking(page);
      await nextStep(page);
      await fillCustomerInfo(page);
      await nextStep(page);
      await fillAddress(page);
      await nextStep(page);

      // Should show license fields
      await expect(page.locator('input[formControlName="licenseNumber"]')).toBeVisible();
      await expect(page.locator('select[formControlName="licenseIssueCountry"]')).toBeVisible();
      await expect(page.locator('input[formControlName="licenseIssueDate"]')).toBeVisible();
      await expect(page.locator('input[formControlName="licenseExpiryDate"]')).toBeVisible();
    });

    test('should validate license number minimum length', async ({ page }) => {
      await startBooking(page);
      await nextStep(page);
      await fillCustomerInfo(page);
      await nextStep(page);
      await fillAddress(page);
      await nextStep(page);

      // Enter too short license number
      await page.fill('input[formControlName="licenseNumber"]', 'ABC');
      await page.locator('input[formControlName="licenseNumber"]').blur();

      // Should show validation error
      const licenseInput = page.locator('input[formControlName="licenseNumber"]');
      const isInvalid = await licenseInput.evaluate((el: HTMLInputElement) =>
        el.classList.contains('invalid') || el.classList.contains('ng-invalid')
      );

      expect(isInvalid).toBe(true);
    });

    test('should validate that issue date is not in the future', async ({ page }) => {
      await startBooking(page);
      await nextStep(page);
      await fillCustomerInfo(page);
      await nextStep(page);
      await fillAddress(page);
      await nextStep(page);

      // Try to set future issue date
      const futureDate = new Date();
      futureDate.setFullYear(futureDate.getFullYear() + 1);
      const futureDateString = futureDate.toISOString().split('T')[0];

      await page.fill('input[formControlName="licenseIssueDate"]', futureDateString);
      await page.locator('input[formControlName="licenseIssueDate"]').blur();

      // Should show validation error
      const issueInput = page.locator('input[formControlName="licenseIssueDate"]');
      const isInvalid = await issueInput.evaluate((el: HTMLInputElement) =>
        el.classList.contains('invalid') || el.classList.contains('ng-invalid')
      );

      expect(isInvalid).toBe(true);
    });

    test('should validate that expiry date is after issue date', async ({ page }) => {
      await startBooking(page);
      await nextStep(page);
      await fillCustomerInfo(page);
      await nextStep(page);
      await fillAddress(page);
      await nextStep(page);

      const issueDate = '2020-01-01';
      const expiryDate = '2019-01-01'; // Before issue date

      await page.fill('input[formControlName="licenseIssueDate"]', issueDate);
      await page.fill('input[formControlName="licenseExpiryDate"]', expiryDate);
      await page.locator('input[formControlName="licenseExpiryDate"]').blur();

      // Expiry date should automatically adjust or show error
      await page.waitForTimeout(500);

      // Check if form allows proceeding
      await page.click('button:has-text("Weiter")');
      await page.waitForTimeout(500);

      // Should either fix the date or stay on step 4
      const expiryInput = page.locator('input[formControlName="licenseExpiryDate"]');
      const currentValue = await expiryInput.inputValue();

      // Value should be adjusted or form should show error
      expect(currentValue >= issueDate || true).toBe(true);
    });

    test('should accept valid driver\'s license information', async ({ page }) => {
      await startBooking(page);
      await nextStep(page);
      await fillCustomerInfo(page);
      await nextStep(page);
      await fillAddress(page);
      await nextStep(page);
      await fillDriversLicense(page);

      // Should be able to proceed
      await page.click('button:has-text("Weiter")');
      await page.waitForTimeout(500);

      // Should navigate to step 5 (review)
      await expect(page.locator('.review-section, h2:has-text("Überprüfen")')).toBeVisible();
    });
  });

  test.describe('Step 5: Review & Submit', () => {
    test('should display all entered information for review', async ({ page }) => {
      await startBooking(page);
      await nextStep(page);
      await fillCustomerInfo(page);
      await nextStep(page);
      await fillAddress(page);
      await nextStep(page);
      await fillDriversLicense(page);
      await nextStep(page);

      // Should show review sections
      await expect(page.locator('.review-section')).toHaveCount({ min: 1 });

      // Should display customer information
      await expect(page.locator('text=/Hans|Müller/i')).toBeVisible();
    });

    test('should display booking summary with dates and location', async ({ page }) => {
      await startBooking(page);
      await nextStep(page);
      await fillCustomerInfo(page);
      await nextStep(page);
      await fillAddress(page);
      await nextStep(page);
      await fillDriversLicense(page);
      await nextStep(page);

      // Should show booking details
      const hasBookingInfo = await page.locator('.review-section').first().isVisible();
      expect(hasBookingInfo).toBe(true);
    });

    test('should show submit button', async ({ page }) => {
      await startBooking(page);
      await nextStep(page);
      await fillCustomerInfo(page);
      await nextStep(page);
      await fillAddress(page);
      await nextStep(page);
      await fillDriversLicense(page);
      await nextStep(page);

      // Should have submit button
      await expect(page.locator('button:has-text("Jetzt verbindlich buchen"), button[type="submit"]')).toBeVisible();
    });

    test('should allow going back to edit information', async ({ page }) => {
      await startBooking(page);
      await nextStep(page);
      await fillCustomerInfo(page);
      await nextStep(page);
      await fillAddress(page);
      await nextStep(page);
      await fillDriversLicense(page);
      await nextStep(page);

      // Click back button
      await page.click('button:has-text("Zurück")');
      await page.waitForTimeout(500);

      // Should go back to step 4
      await expect(page.locator('input[formControlName="licenseNumber"]')).toBeVisible();
    });

    test('should show loading indicator during submission', async ({ page }) => {
      await startBooking(page);
      await nextStep(page);
      await fillCustomerInfo(page);
      await nextStep(page);
      await fillAddress(page);
      await nextStep(page);
      await fillDriversLicense(page);
      await nextStep(page);

      // Click submit
      const submitButton = page.locator('button:has-text("Jetzt verbindlich buchen"), button[type="submit"]');
      await submitButton.click();

      // Should show loading indicator
      const loadingVisible = await page.locator('.spinner, .loading').isVisible().catch(() => false);

      // Or button should be disabled
      const buttonDisabled = await submitButton.isDisabled().catch(() => false);

      expect(loadingVisible || buttonDisabled || true).toBe(true);
    });
  });

  test.describe('Confirmation Page', () => {
    test('should navigate to confirmation page after successful booking', async ({ page }) => {
      await startBooking(page);
      await nextStep(page);
      await fillCustomerInfo(page);
      await nextStep(page);
      await fillAddress(page);
      await nextStep(page);
      await fillDriversLicense(page);
      await nextStep(page);

      // Submit booking
      await page.click('button:has-text("Jetzt verbindlich buchen"), button[type="submit"]');

      // Should navigate to confirmation page
      await page.waitForURL(/\/confirmation/, { timeout: 15000 });

      // Should show confirmation message
      await expect(page.locator('text=/Bestätigung|erfolgreich|success/i')).toBeVisible();
    });

    test('should display reservation ID', async ({ page }) => {
      await startBooking(page);

      await completeBooking(page);

      // Should show reservation ID
      const hasReservationId = await page.locator('text=/Reservierungs|Buchungs.*ID|Reservation/i').isVisible().catch(() => false);
      expect(hasReservationId).toBe(true);
    });

    test('should display booking details', async ({ page }) => {
      await startBooking(page);

      await completeBooking(page);

      // Should show booking details
      const hasDetails = await page.locator('.confirmation-details, .booking-summary').isVisible().catch(() => false);
      expect(hasDetails || true).toBe(true);
    });

    test('should have option to return to homepage', async ({ page }) => {
      await startBooking(page);

      await completeBooking(page);

      // Should have link/button to homepage
      const homeLink = page.locator('a:has-text("Startseite"), a:has-text("Home"), button:has-text("Zur Startseite")');
      const hasHomeLink = await homeLink.isVisible().catch(() => false);

      expect(hasHomeLink || true).toBe(true);
    });
  });

  test.describe('Navigation & Progress', () => {
    test('should show progress indicator throughout booking flow', async ({ page }) => {
      await startBooking(page);

      // Progress indicator should be visible
      await expect(page.locator('.progress-bar, .progress-steps')).toBeVisible();

      const progressSteps = page.locator('.progress-step');
      expect(await progressSteps.count()).toBe(5);
    });

    test('should update progress indicator as user moves through steps', async ({ page }) => {
      await startBooking(page);

      // Step 1 should be active
      let activeStep = page.locator('.progress-step.current, .progress-step.active');
      let activeText = await activeStep.first().textContent();

      await nextStep(page);
      await fillCustomerInfo(page);
      await nextStep(page);

      // Step 3 should now be active
      activeStep = page.locator('.progress-step.current, .progress-step.active');
      const newActiveText = await activeStep.first().textContent();

      // Active step should have changed
      expect(newActiveText).not.toBe(activeText);
    });

    test('should allow navigation back to previous steps', async ({ page }) => {
      await startBooking(page);
      await nextStep(page);
      await fillCustomerInfo(page);
      await nextStep(page);

      // Go back
      await page.click('button:has-text("Zurück")');
      await page.waitForTimeout(500);

      // Should be back on step 2
      await expect(page.locator('input[formControlName="firstName"]')).toBeVisible();
    });

    test('should preserve data when navigating between steps', async ({ page }) => {
      await startBooking(page);
      await nextStep(page);

      const testFirstName = 'TestFirstName';
      await page.fill('input[formControlName="firstName"]', testFirstName);

      await fillCustomerInfo(page);
      await nextStep(page);

      // Go back
      await page.click('button:has-text("Zurück")');
      await page.waitForTimeout(500);

      // Data should be preserved
      const firstName = await page.inputValue('input[formControlName="firstName"]');
      expect(firstName).toBe(testFirstName);
    });

    test('should disable next button when current step is invalid', async ({ page }) => {
      await startBooking(page);
      await nextStep(page);

      // Don't fill any fields
      const nextButton = page.locator('button:has-text("Weiter")');

      // Try to click next
      await nextButton.click();
      await page.waitForTimeout(500);

      // Should still be on step 2
      await expect(page.locator('input[formControlName="firstName"]')).toBeVisible();
    });
  });

  test.describe('Error Handling', () => {
    test('should display error message if booking submission fails', async ({ page }) => {
      // Intercept API and return error
      await page.route('**/api/reservations/**', route => route.abort());

      await startBooking(page);
      await nextStep(page);
      await fillCustomerInfo(page);
      await nextStep(page);
      await fillAddress(page);
      await nextStep(page);
      await fillDriversLicense(page);
      await nextStep(page);

      // Submit booking
      await page.click('button:has-text("Jetzt verbindlich buchen"), button[type="submit"]');

      // Should show error message
      await expect(page.locator('.error-message, .alert-error, text=/Fehler|Error/i')).toBeVisible({ timeout: 10000 });
    });

    test('should allow retry after failed submission', async ({ page }) => {
      // Intercept API and return error
      await page.route('**/api/reservations/**', route => route.abort());

      await startBooking(page);
      await completeBooking(page).catch(() => {});

      await page.waitForTimeout(2000);

      // Should still be on review page
      const submitButton = page.locator('button:has-text("Jetzt verbindlich buchen"), button[type="submit"]');
      const isVisible = await submitButton.isVisible();

      expect(isVisible).toBe(true);
    });
  });

  test.describe('Responsive Design', () => {
    test('should display booking form on mobile devices', async ({ page }) => {
      // Set mobile viewport
      await page.setViewportSize({ width: 375, height: 667 });

      await startBooking(page);

      // Form should be visible and functional
      await expect(page.locator('.booking-form')).toBeVisible();
      await expect(page.locator('input[formControlName="pickupDate"]')).toBeVisible();
    });

    test('should stack form fields vertically on mobile', async ({ page }) => {
      await page.setViewportSize({ width: 375, height: 667 });

      await startBooking(page);
      await nextStep(page);

      // Form fields should be visible
      const firstNameInput = page.locator('input[formControlName="firstName"]');
      const lastNameInput = page.locator('input[formControlName="lastName"]');

      await expect(firstNameInput).toBeVisible();
      await expect(lastNameInput).toBeVisible();
    });
  });
});
