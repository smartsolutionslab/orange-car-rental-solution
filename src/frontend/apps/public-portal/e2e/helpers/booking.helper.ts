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
  const categoryCode = testVehicles.available.categoryCode;
  const pickupDate = testBooking.pickupDate();
  const returnDate = testBooking.returnDate();
  const locationCode = testBooking.locationCode;

  await page.goto(
    `/booking?vehicleId=${vehicle}&categoryCode=${categoryCode}&pickupDate=${pickupDate}&returnDate=${returnDate}&locationCode=${locationCode}`
  );

  // Wait for form to load
  await page.waitForSelector('.booking-form', { timeout: 10000 });
}

/**
 * Fill in customer information step
 * Uses ocr-input custom components which wrap native inputs
 */
export async function fillCustomerInfo(page: Page, userData = testUsers.guest) {
  await page.fill('ocr-input[formControlName="firstName"] input', userData.firstName);
  await page.fill('ocr-input[formControlName="lastName"] input', userData.lastName);
  await page.fill('ocr-input[formControlName="email"] input', userData.email);
  await page.fill('ocr-input[formControlName="phoneNumber"] input', userData.phoneNumber);
  await page.fill('ocr-input[formControlName="dateOfBirth"] input', userData.dateOfBirth);
}

/**
 * Fill in address information step
 * Uses ocr-input custom components (country is also an input, not a select)
 */
export async function fillAddress(page: Page, address = testUsers.guest.address) {
  await page.fill('ocr-input[formControlName="street"] input', address.street);
  await page.fill('ocr-input[formControlName="city"] input', address.city);
  await page.fill('ocr-input[formControlName="postalCode"] input', address.postalCode);
  // Country is pre-filled with "Deutschland" and uses ocr-input, not select
  await page.fill('ocr-input[formControlName="country"] input', address.country);
}

/**
 * Fill in driver's license information step
 * Uses ocr-input custom components
 */
export async function fillDriversLicense(page: Page, license = testUsers.guest.driversLicense) {
  await page.fill('ocr-input[formControlName="licenseNumber"] input', license.licenseNumber);
  // licenseIssueCountry uses ocr-input, not select
  await page.fill('ocr-input[formControlName="licenseIssueCountry"] input', license.licenseIssueCountry);
  await page.fill('ocr-input[formControlName="licenseIssueDate"] input', license.licenseIssueDate);
  await page.fill('ocr-input[formControlName="licenseExpiryDate"] input', license.licenseExpiryDate);
}

/**
 * Navigate through booking form steps
 */
export async function nextStep(page: Page) {
  // Wait for the Next button to be visible and enabled
  const nextButton = page.locator('button:has-text("Weiter")');
  await nextButton.waitFor({ state: 'visible', timeout: 10000 });

  // Wait until button is enabled using Playwright's isEnabled check with retry
  await expect(nextButton).toBeEnabled({ timeout: 10000 });

  await nextButton.click();
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
  await page.click('button:has-text("Buchung abschließen")');

  // Wait for confirmation page
  await page.waitForURL(/\/confirmation/, { timeout: 15000 });
}

/**
 * Check if form fields are pre-filled
 */
export async function checkPrefilledData(page: Page, expectedData: { firstName: string; lastName: string; email: string }) {
  const firstName = await page.inputValue('ocr-input[formControlName="firstName"] input');
  const lastName = await page.inputValue('ocr-input[formControlName="lastName"] input');
  const email = await page.inputValue('ocr-input[formControlName="email"] input');

  expect(firstName).toBe(expectedData.firstName);
  expect(lastName).toBe(expectedData.lastName);
  expect(email).toBe(expectedData.email);
}
