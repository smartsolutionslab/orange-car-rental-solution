import { Page, expect } from '@playwright/test';
import { testBooking, testVehicles, testUsers } from '../fixtures/test-data';

/**
 * Booking helper functions for E2E tests
 */

/**
 * Navigate to booking page with vehicle selection
 */
export async function startBooking(page: Page, vehicleId?: string) {
  const vehicle = vehicleId || testVehicles.available.id;
  const pickupDate = testBooking.pickupDate();
  const returnDate = testBooking.returnDate();
  const locationCode = testBooking.locationCode;

  await page.goto(
    `/booking?vehicleId=${vehicle}&pickupDate=${pickupDate}&returnDate=${returnDate}&locationCode=${locationCode}`
  );

  // Wait for form to load
  await page.waitForSelector('.booking-form', { timeout: 10000 });
}

/**
 * Fill in customer information step
 */
export async function fillCustomerInfo(page: Page, userData = testUsers.guest) {
  await page.fill('input[formControlName="firstName"]', userData.firstName);
  await page.fill('input[formControlName="lastName"]', userData.lastName);
  await page.fill('input[formControlName="email"]', userData.email);
  await page.fill('input[formControlName="phoneNumber"]', userData.phoneNumber);
  await page.fill('input[formControlName="dateOfBirth"]', userData.dateOfBirth);
}

/**
 * Fill in address information step
 */
export async function fillAddress(page: Page, address = testUsers.guest.address) {
  await page.fill('input[formControlName="street"]', address.street);
  await page.fill('input[formControlName="city"]', address.city);
  await page.fill('input[formControlName="postalCode"]', address.postalCode);
  await page.selectOption('select[formControlName="country"]', address.country);
}

/**
 * Fill in driver's license information step
 */
export async function fillDriversLicense(page: Page, license = testUsers.guest.driversLicense) {
  await page.fill('input[formControlName="licenseNumber"]', license.licenseNumber);
  await page.selectOption('select[formControlName="licenseIssueCountry"]', license.licenseIssueCountry);
  await page.fill('input[formControlName="licenseIssueDate"]', license.licenseIssueDate);
  await page.fill('input[formControlName="licenseExpiryDate"]', license.licenseExpiryDate);
}

/**
 * Navigate through booking form steps
 */
export async function nextStep(page: Page) {
  await page.click('button:has-text("Weiter")');
  await page.waitForTimeout(500); // Wait for animation
}

/**
 * Complete entire booking flow
 */
export async function completeBooking(page: Page, userData = testUsers.guest) {
  // Step 1: Vehicle details should already be filled
  await expect(page.locator('.progress-step.current').first()).toBeVisible();
  await nextStep(page);

  // Step 2: Customer information
  await fillCustomerInfo(page, userData);
  await nextStep(page);

  // Step 3: Address
  await fillAddress(page, userData.address);
  await nextStep(page);

  // Step 4: Driver's license
  await fillDriversLicense(page, userData.driversLicense);
  await nextStep(page);

  // Step 5: Review and submit
  await page.click('button:has-text("Jetzt verbindlich buchen")');

  // Wait for confirmation page
  await page.waitForURL(/\/confirmation/, { timeout: 15000 });
}

/**
 * Check if form fields are pre-filled
 */
export async function checkPrefilledData(page: Page, expectedData: { firstName: string; lastName: string; email: string }) {
  const firstName = await page.inputValue('input[formControlName="firstName"]');
  const lastName = await page.inputValue('input[formControlName="lastName"]');
  const email = await page.inputValue('input[formControlName="email"]');

  expect(firstName).toBe(expectedData.firstName);
  expect(lastName).toBe(expectedData.lastName);
  expect(email).toBe(expectedData.email);
}
