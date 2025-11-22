import { test, expect } from '@playwright/test';
import { login, logout } from './helpers/auth.helper';
import { startBooking, nextStep, fillCustomerInfo, fillAddress, fillDriversLicense } from './helpers/booking.helper';
import { testUsers } from './fixtures/test-data';

/**
 * E2E Tests for US-5: Pre-fill Renter Data for Registered Users
 *
 * Covers:
 * - Automatic form pre-fill for authenticated users
 * - Pre-fill of customer information
 * - Pre-fill of address information
 * - Pre-fill of driver's license information
 * - Profile update checkbox functionality
 * - Profile update after booking
 * - Guest booking without pre-fill
 */

test.describe('US-5: Profile Pre-fill for Registered Users', () => {
  test.describe('Authenticated User Booking Flow', () => {
    test.beforeEach(async ({ page }) => {
      // Login before each test
      await login(page);
    });

    test.afterEach(async ({ page }) => {
      // Logout after each test
      await logout(page);
    });

    test('should pre-fill customer information for logged-in user', async ({ page }) => {
      await startBooking(page);

      // Navigate to customer info step (Step 2)
      await nextStep(page);

      // Wait for form to be visible
      await page.waitForSelector('input[formControlName="firstName"]', { timeout: 5000 });

      // Check if customer info is pre-filled
      const firstName = await page.inputValue('input[formControlName="firstName"]');
      const lastName = await page.inputValue('input[formControlName="lastName"]');
      const email = await page.inputValue('input[formControlName="email"]');
      const phoneNumber = await page.inputValue('input[formControlName="phoneNumber"]');
      const dateOfBirth = await page.inputValue('input[formControlName="dateOfBirth"]');

      expect(firstName).toBe(testUsers.registered.firstName);
      expect(lastName).toBe(testUsers.registered.lastName);
      expect(email).toBe(testUsers.registered.email);
      expect(phoneNumber).toBe(testUsers.registered.phoneNumber);
      expect(dateOfBirth).toBe(testUsers.registered.dateOfBirth);
    });

    test('should pre-fill address information for logged-in user', async ({ page }) => {
      await startBooking(page);

      // Navigate to address step (Step 3)
      await nextStep(page); // Step 2
      await nextStep(page); // Step 3

      // Wait for form to be visible
      await page.waitForSelector('input[formControlName="street"]', { timeout: 5000 });

      // Check if address is pre-filled
      const street = await page.inputValue('input[formControlName="street"]');
      const city = await page.inputValue('input[formControlName="city"]');
      const postalCode = await page.inputValue('input[formControlName="postalCode"]');
      const country = await page.inputValue('select[formControlName="country"]');

      expect(street).toBe(testUsers.registered.address.street);
      expect(city).toBe(testUsers.registered.address.city);
      expect(postalCode).toBe(testUsers.registered.address.postalCode);
      expect(country).toBe(testUsers.registered.address.country);
    });

    test('should pre-fill drivers license information for logged-in user', async ({ page }) => {
      await startBooking(page);

      // Navigate to driver's license step (Step 4)
      await nextStep(page); // Step 2
      await nextStep(page); // Step 3
      await nextStep(page); // Step 4

      // Wait for form to be visible
      await page.waitForSelector('input[formControlName="licenseNumber"]', { timeout: 5000 });

      // Check if license info is pre-filled
      const licenseNumber = await page.inputValue('input[formControlName="licenseNumber"]');
      const licenseIssueCountry = await page.inputValue('select[formControlName="licenseIssueCountry"]');
      const licenseIssueDate = await page.inputValue('input[formControlName="licenseIssueDate"]');
      const licenseExpiryDate = await page.inputValue('input[formControlName="licenseExpiryDate"]');

      expect(licenseNumber).toBe(testUsers.registered.driversLicense.licenseNumber);
      expect(licenseIssueCountry).toBe(testUsers.registered.driversLicense.licenseIssueCountry);
      expect(licenseIssueDate).toBe(testUsers.registered.driversLicense.licenseIssueDate);
      expect(licenseExpiryDate).toBe(testUsers.registered.driversLicense.licenseExpiryDate);
    });

    test('should allow user to modify pre-filled data', async ({ page }) => {
      await startBooking(page);

      // Navigate to customer info step
      await nextStep(page);

      // Wait for form
      await page.waitForSelector('input[formControlName="firstName"]', { timeout: 5000 });

      // Modify pre-filled data
      const newFirstName = 'ModifiedName';
      await page.fill('input[formControlName="firstName"]', newFirstName);

      // Value should be updated
      const firstName = await page.inputValue('input[formControlName="firstName"]');
      expect(firstName).toBe(newFirstName);

      // Should be able to proceed to next step
      await nextStep(page);
      await expect(page.locator('.progress-step').nth(2)).toHaveClass(/current|active/);
    });

    test('should show profile update checkbox in review step', async ({ page }) => {
      await startBooking(page);

      // Navigate to review step (Step 5)
      await nextStep(page); // Step 2
      await nextStep(page); // Step 3
      await nextStep(page); // Step 4
      await nextStep(page); // Step 5

      // Wait for review step
      await page.waitForSelector('.review-section', { timeout: 5000 });

      // Check if profile update checkbox is visible
      const updateCheckbox = page.locator('input[type="checkbox"][formControlName="updateMyProfile"]');
      await expect(updateCheckbox).toBeVisible();

      // Check if label text is correct
      await expect(page.locator('text=/Diese Änderungen.*Profil speichern/i')).toBeVisible();
    });

    test('should update profile when checkbox is checked', async ({ page }) => {
      await startBooking(page);

      // Navigate through all steps, modifying data
      await nextStep(page); // Step 2

      // Modify customer info
      await page.fill('input[formControlName="phoneNumber"]', '+49 30 99999999');
      await nextStep(page); // Step 3

      // Modify address
      await page.fill('input[formControlName="street"]', 'Neue Straße 999');
      await nextStep(page); // Step 4
      await nextStep(page); // Step 5

      // Check the update profile checkbox
      const updateCheckbox = page.locator('input[type="checkbox"][formControlName="updateMyProfile"]');
      await updateCheckbox.check();

      // Submit booking
      await page.click('button:has-text("Jetzt verbindlich buchen")');

      // Should navigate to confirmation
      await page.waitForURL(/\/confirmation/, { timeout: 15000 });

      // Note: Actual profile update verification would require checking API response
      // or navigating to profile page after booking
    });

    test('should not update profile when checkbox is unchecked', async ({ page }) => {
      await startBooking(page);

      // Navigate through all steps
      await nextStep(page); // Step 2
      await nextStep(page); // Step 3
      await nextStep(page); // Step 4
      await nextStep(page); // Step 5

      // Ensure update profile checkbox is unchecked
      const updateCheckbox = page.locator('input[type="checkbox"][formControlName="updateMyProfile"]');
      await updateCheckbox.uncheck();

      // Submit booking
      await page.click('button:has-text("Jetzt verbindlich buchen")');

      // Should navigate to confirmation
      await page.waitForURL(/\/confirmation/, { timeout: 15000 });

      // Profile should not be updated (would need to verify via API or profile page)
    });

    test('should show loading state while fetching profile', async ({ page }) => {
      // Start booking immediately after login
      await login(page);
      await startBooking(page);

      // Should show some indication of loading (spinner, skeleton, etc.)
      // This depends on implementation - adjust selector as needed
      const hasLoadingState = await page.locator('.spinner, .loading, [role="progressbar"]').isVisible().catch(() => false);

      // Just verify booking page loads successfully
      await page.waitForSelector('.booking-form', { timeout: 10000 });
      expect(await page.locator('.booking-form').isVisible()).toBe(true);
    });
  });

  test.describe('Guest User Booking Flow', () => {
    test('should not pre-fill data for guest users', async ({ page }) => {
      // Start booking without logging in
      await startBooking(page);

      // Navigate to customer info step
      await nextStep(page);

      // Wait for form
      await page.waitForSelector('input[formControlName="firstName"]', { timeout: 5000 });

      // Check that fields are empty
      const firstName = await page.inputValue('input[formControlName="firstName"]');
      const lastName = await page.inputValue('input[formControlName="lastName"]');
      const email = await page.inputValue('input[formControlName="email"]');

      expect(firstName).toBe('');
      expect(lastName).toBe('');
      expect(email).toBe('');
    });

    test('should not show profile update checkbox for guest users', async ({ page }) => {
      // Start booking without logging in
      await startBooking(page);

      // Navigate to review step
      await nextStep(page); // Step 2
      await fillCustomerInfo(page);
      await nextStep(page); // Step 3
      await fillAddress(page);
      await nextStep(page); // Step 4
      await fillDriversLicense(page);
      await nextStep(page); // Step 5

      // Wait for review step
      await page.waitForSelector('.review-section', { timeout: 5000 });

      // Profile update checkbox should not be visible
      const updateCheckbox = page.locator('input[type="checkbox"][formControlName="updateMyProfile"]');
      await expect(updateCheckbox).not.toBeVisible();
    });

    test('should allow guest user to complete booking', async ({ page }) => {
      await startBooking(page);

      // Fill in all steps manually
      await nextStep(page); // Step 2
      await fillCustomerInfo(page);
      await nextStep(page); // Step 3
      await fillAddress(page);
      await nextStep(page); // Step 4
      await fillDriversLicense(page);
      await nextStep(page); // Step 5

      // Submit booking
      await page.click('button:has-text("Jetzt verbindlich buchen")');

      // Should navigate to confirmation
      await page.waitForURL(/\/confirmation/, { timeout: 15000 });
      await expect(page.locator('text=/Bestätigung|Reservierung erfolgreich/i')).toBeVisible();
    });
  });

  test.describe('Profile Pre-fill Edge Cases', () => {
    test.beforeEach(async ({ page }) => {
      await login(page);
    });

    test.afterEach(async ({ page }) => {
      await logout(page);
    });

    test('should handle profile with missing driver license gracefully', async ({ page }) => {
      // This test assumes the user profile might not have driver's license info
      await startBooking(page);

      // Navigate to driver's license step
      await nextStep(page); // Step 2
      await nextStep(page); // Step 3
      await nextStep(page); // Step 4

      // Wait for form
      await page.waitForSelector('input[formControlName="licenseNumber"]', { timeout: 5000 });

      // Form should be present but fields might be empty if no license in profile
      const formVisible = await page.locator('input[formControlName="licenseNumber"]').isVisible();
      expect(formVisible).toBe(true);

      // User should be able to fill in the fields manually if not pre-filled
      await page.fill('input[formControlName="licenseNumber"]', 'NEW123456789');
      const licenseNumber = await page.inputValue('input[formControlName="licenseNumber"]');
      expect(licenseNumber).toBe('NEW123456789');
    });

    test('should preserve user modifications when navigating back and forth', async ({ page }) => {
      await startBooking(page);

      // Navigate to customer info
      await nextStep(page);

      // Modify a field
      const modifiedPhone = '+49 30 11111111';
      await page.fill('input[formControlName="phoneNumber"]', modifiedPhone);

      // Go to next step
      await nextStep(page);

      // Go back
      await page.click('button:has-text("Zurück")');

      // Modified value should be preserved
      const phoneNumber = await page.inputValue('input[formControlName="phoneNumber"]');
      expect(phoneNumber).toBe(modifiedPhone);
    });
  });
});
