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

  await page.fill('input[name="email"]', email || user.email);
  await page.fill('input[name="password"]', password || user.password);

  await page.click('button[type="submit"]');

  // Wait for navigation to complete
  await page.waitForURL('/', { timeout: 10000 });
}

/**
 * Register a new user
 */
export async function register(page: Page, userData = testUsers.newUser) {
  await page.goto('/register');

  await page.fill('input[name="firstName"]', userData.firstName);
  await page.fill('input[name="lastName"]', userData.lastName);
  await page.fill('input[name="email"]', userData.email);
  await page.fill('input[name="phoneNumber"]', userData.phoneNumber);
  await page.fill('input[name="dateOfBirth"]', userData.dateOfBirth);
  await page.fill('input[name="password"]', userData.password);
  await page.fill('input[name="confirmPassword"]', userData.confirmPassword);

  // Accept terms
  await page.check('input[type="checkbox"][formControlName="acceptTerms"]');

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
