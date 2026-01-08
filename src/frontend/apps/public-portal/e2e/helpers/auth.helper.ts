import { Page } from '@playwright/test';
import { testUsers } from '../fixtures/test-data';

/**
 * Authentication helper functions for E2E tests
 *
 * In E2E mode with MockKeycloak, the user is already authenticated from app startup.
 * These helpers detect mock mode and skip actual form interactions.
 */

/**
 * Check if running with MockKeycloak (E2E mode)
 * MockKeycloak provides a mock token that can be detected
 */
async function isMockKeycloakMode(page: Page): Promise<boolean> {
  // Check if we're already on a page and authenticated
  // In E2E mode with MockKeycloak, the app starts authenticated
  try {
    // Navigate to home if not already there
    if (!page.url().includes('/')) {
      await page.goto('/');
    }
    // Check for authenticated UI elements (logout button visible)
    const logoutButton = page.locator('button:has-text("Abmelden")');
    return await logoutButton.isVisible({ timeout: 2000 });
  } catch {
    return false;
  }
}

/**
 * Login with existing user credentials
 * In MockKeycloak mode, user is already authenticated - just navigate home
 */
export async function login(page: Page, email?: string, password?: string) {
  // In E2E mode with MockKeycloak, user is already authenticated
  // Just navigate to home page
  await page.goto('/');

  // Check if already authenticated (MockKeycloak mode)
  const isAuthenticated = await isLoggedIn(page);
  if (isAuthenticated) {
    return; // Already logged in via MockKeycloak
  }

  // If not authenticated, try actual login flow
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
 * In MockKeycloak mode, logout is simulated - just navigate away
 */
export async function logout(page: Page) {
  // Check if logout button exists
  const logoutButton = page.locator('button:has-text("Abmelden")');
  const hasLogoutButton = await logoutButton.isVisible().catch(() => false);

  if (hasLogoutButton) {
    try {
      await logoutButton.click();
      // In E2E mode with MockKeycloak, logout might not actually redirect
      // Wait briefly for any navigation
      await page.waitForTimeout(500);
    } catch {
      // Ignore logout errors in E2E mode
    }
  }
  // Don't wait for /login redirect - MockKeycloak may not support it
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
