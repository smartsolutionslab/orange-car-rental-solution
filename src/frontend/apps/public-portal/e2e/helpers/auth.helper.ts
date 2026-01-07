import { Page } from '@playwright/test';
import { testUsers } from '../fixtures/test-data';

/**
 * Authentication helper functions for E2E tests
 */

/**
 * Login with existing user credentials
 */
export async function login(page: Page, email?: string, password?: string) {
  const user = testUsers.registered;
  await page.goto('/login');

  // Login form uses id selectors: #login-email and #login-password
  await page.fill('#login-email', email || user.email);
  await page.fill('#login-password', password || user.password);

  await page.click('button[type="submit"]');

  // Wait for navigation to complete
  await page.waitForURL('/', { timeout: 10000 });
}

/**
 * Register a new user
 * The register form is multi-step:
 * - Step 1: email, password, confirmPassword
 * - Step 2: firstName, lastName, phoneNumber, dateOfBirth
 * - Step 3: acceptTerms, acceptPrivacy checkboxes
 */
export async function register(page: Page, userData = testUsers.newUser) {
  await page.goto('/register');

  // Step 1: Account Information
  // Uses ocr-input components which wrap native inputs
  await page.fill('ocr-input[formControlName="email"] input', userData.email);
  await page.fill('ocr-input[formControlName="password"] input', userData.password);
  await page.fill('ocr-input[formControlName="confirmPassword"] input', userData.confirmPassword);
  await page.click('button.primary-button'); // Next button

  // Step 2: Personal Information
  await page.waitForTimeout(300); // Wait for step transition
  await page.fill('ocr-input[formControlName="firstName"] input', userData.firstName);
  await page.fill('ocr-input[formControlName="lastName"] input', userData.lastName);
  await page.fill('ocr-input[formControlName="phoneNumber"] input', userData.phoneNumber);
  // dateOfBirth uses native input with id="dateOfBirth"
  await page.fill('#dateOfBirth', userData.dateOfBirth);
  await page.click('button.primary-button'); // Next button

  // Step 3: Terms and Conditions
  await page.waitForTimeout(300); // Wait for step transition
  // Accept terms using ocr-checkbox components
  await page.click('ocr-checkbox[formControlName="acceptTerms"]');
  await page.click('ocr-checkbox[formControlName="acceptPrivacy"]');

  await page.click('button[type="submit"]');

  // Wait for successful registration and redirect
  await page.waitForURL('/', { timeout: 15000 });
}

/**
 * Logout current user
 */
export async function logout(page: Page) {
  await page.click('button:has-text("Abmelden")');
  await page.waitForURL('/login', { timeout: 5000 });
}

/**
 * Check if user is logged in
 */
export async function isLoggedIn(page: Page): Promise<boolean> {
  try {
    const logoutButton = await page.locator('button:has-text("Abmelden")');
    return await logoutButton.isVisible();
  } catch {
    return false;
  }
}
