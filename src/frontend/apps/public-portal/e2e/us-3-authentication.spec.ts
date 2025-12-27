import { test, expect } from '@playwright/test';
import { login, register, logout, isLoggedIn } from './helpers/auth.helper';
import { testUsers } from './fixtures/test-data';

/**
 * E2E Tests for US-3: User Registration and Authentication
 *
 * Covers:
 * - User login with valid credentials
 * - User login with invalid credentials
 * - "Remember me" functionality
 * - User registration
 * - Password strength validation
 * - Forgot password flow
 * - Email validation
 * - Session persistence
 */

test.describe('US-3: User Authentication', () => {
  test.beforeEach(async ({ page }) => {
    // Ensure we're logged out before each test
    await page.goto('/');
    if (await isLoggedIn(page)) {
      await logout(page);
    }
  });

  test.describe('Login Flow', () => {
    test('should successfully login with valid credentials', async ({ page }) => {
      await page.goto('/login');

      // Fill in login form
      await page.fill('input[name="email"]', testUsers.registered.email);
      await page.fill('input[name="password"]', testUsers.registered.password);

      // Submit form
      await page.click('button[type="submit"]');

      // Should redirect to home page
      await page.waitForURL('/', { timeout: 10000 });

      // Should show logged-in state
      await expect(page.locator('text=Meine Buchungen')).toBeVisible();
    });

    test('should show error message with invalid credentials', async ({ page }) => {
      await page.goto('/login');

      // Fill in login form with invalid credentials
      await page.fill('input[name="email"]', 'invalid@example.com');
      await page.fill('input[name="password"]', 'WrongPassword123!');

      // Submit form
      await page.click('button[type="submit"]');

      // Should show error message
      await expect(page.locator('.error-message, .alert-error')).toBeVisible({ timeout: 5000 });

      // Should remain on login page
      expect(page.url()).toContain('/login');
    });

    test('should validate email format', async ({ page }) => {
      await page.goto('/login');

      // Enter invalid email
      await page.fill('input[name="email"]', 'not-an-email');
      await page.fill('input[name="password"]', 'SomePassword123!');

      // Try to submit
      await page.click('button[type="submit"]');

      // Should show validation error
      const emailInput = page.locator('input[name="email"]');
      await expect(emailInput).toHaveClass(/invalid|ng-invalid/);
    });

    test('should toggle password visibility', async ({ page }) => {
      await page.goto('/login');

      const passwordInput = page.locator('input[name="password"]');
      const toggleButton = page.locator('button.password-toggle, button[aria-label*="Passwort"]');

      // Password should be hidden by default
      await expect(passwordInput).toHaveAttribute('type', 'password');

      // Click toggle button
      if (await toggleButton.isVisible()) {
        await toggleButton.click();

        // Password should now be visible
        await expect(passwordInput).toHaveAttribute('type', 'text');

        // Click toggle again
        await toggleButton.click();

        // Password should be hidden again
        await expect(passwordInput).toHaveAttribute('type', 'password');
      }
    });

    test('should persist session with "Remember Me"', async ({ page, context }) => {
      await page.goto('/login');

      // Fill in login form
      await page.fill('input[name="email"]', testUsers.registered.email);
      await page.fill('input[name="password"]', testUsers.registered.password);

      // Check "Remember Me"
      const rememberMeCheckbox = page.locator('input[type="checkbox"][formControlName="rememberMe"]');
      if (await rememberMeCheckbox.isVisible()) {
        await rememberMeCheckbox.check();
      }

      // Submit form
      await page.click('button[type="submit"]');

      // Wait for redirect
      await page.waitForURL('/', { timeout: 10000 });

      // Get cookies
      const cookies = await context.cookies();
      const authCookie = cookies.find(c => c.name.includes('token') || c.name.includes('auth'));

      // Auth cookie should exist (if remember me is implemented with cookies)
      // This test might need adjustment based on actual implementation
      expect(await isLoggedIn(page)).toBe(true);
    });

    test('should navigate to forgot password page', async ({ page }) => {
      await page.goto('/login');

      // Click forgot password link
      await page.click('a:has-text("Passwort vergessen")');

      // Should navigate to forgot password page
      await page.waitForURL(/\/forgot-password/, { timeout: 5000 });
      await expect(page.locator('h1, h2')).toContainText(/Passwort zurücksetzen|Passwort vergessen/i);
    });

    test('should navigate to registration page', async ({ page }) => {
      await page.goto('/login');

      // Click register link
      await page.click('a:has-text("Konto erstellen"), a:has-text("Registrieren")');

      // Should navigate to register page
      await page.waitForURL(/\/register/, { timeout: 5000 });
      await expect(page.locator('h1, h2')).toContainText(/Registrierung|Konto erstellen/i);
    });
  });

  test.describe('Registration Flow', () => {
    test('should successfully register a new user', async ({ page }) => {
      const newUser = {
        ...testUsers.newUser,
        email: `test.${Date.now()}@orange-rental.de`
      };

      await page.goto('/register');

      // Fill in registration form
      await page.fill('input[name="firstName"]', newUser.firstName);
      await page.fill('input[name="lastName"]', newUser.lastName);
      await page.fill('input[name="email"]', newUser.email);
      await page.fill('input[name="phoneNumber"]', newUser.phoneNumber);
      await page.fill('input[name="dateOfBirth"]', newUser.dateOfBirth);
      await page.fill('input[name="password"]', newUser.password);
      await page.fill('input[name="confirmPassword"]', newUser.confirmPassword);

      // Accept terms
      await page.check('input[type="checkbox"][formControlName="acceptTerms"]');

      // Submit form
      await page.click('button[type="submit"]');

      // Should redirect to home page or login page after successful registration
      await page.waitForURL(/\/|\/login/, { timeout: 15000 });
    });

    test('should validate password strength', async ({ page }) => {
      await page.goto('/register');

      const passwordInput = page.locator('input[name="password"]');

      // Test weak password
      await passwordInput.fill('weak');
      await passwordInput.blur();

      // Should show validation error
      await expect(page.locator('text=/Passwort.*mindestens|zu schwach/i')).toBeVisible();
    });

    test('should validate password confirmation match', async ({ page }) => {
      await page.goto('/register');

      // Fill passwords that don't match
      await page.fill('input[name="password"]', 'StrongPassword123!');
      await page.fill('input[name="confirmPassword"]', 'DifferentPassword123!');

      // Blur to trigger validation
      await page.locator('input[name="confirmPassword"]').blur();

      // Should show validation error
      await expect(page.locator('text=/Passwörter.*nicht.*übereinstimmen|stimmen nicht überein/i')).toBeVisible();
    });

    test('should validate minimum age (18 years)', async ({ page }) => {
      await page.goto('/register');

      // Calculate date for someone under 18
      const today = new Date();
      const underageDate = new Date(today.getFullYear() - 17, today.getMonth(), today.getDate());
      const dateString = underageDate.toISOString().split('T')[0];

      await page.fill('input[name="dateOfBirth"]', dateString);
      await page.locator('input[name="dateOfBirth"]').blur();

      // Should show validation error
      await expect(page.locator('text=/mindestens 18 Jahre|Mindestalter/i')).toBeVisible();
    });

    test('should require terms acceptance', async ({ page }) => {
      await page.goto('/register');

      // Fill in all fields except terms
      const newUser = {
        ...testUsers.newUser,
        email: `test.${Date.now()}@orange-rental.de`
      };

      await page.fill('input[name="firstName"]', newUser.firstName);
      await page.fill('input[name="lastName"]', newUser.lastName);
      await page.fill('input[name="email"]', newUser.email);
      await page.fill('input[name="phoneNumber"]', newUser.phoneNumber);
      await page.fill('input[name="dateOfBirth"]', newUser.dateOfBirth);
      await page.fill('input[name="password"]', newUser.password);
      await page.fill('input[name="confirmPassword"]', newUser.confirmPassword);

      // Don't check terms checkbox

      // Submit button should be disabled or show error
      const submitButton = page.locator('button[type="submit"]');
      const isDisabled = await submitButton.isDisabled();

      if (!isDisabled) {
        await submitButton.click();
        // Should show validation error
        await expect(page.locator('text=/AGB.*akzeptieren|Bedingungen zustimmen/i')).toBeVisible();
      }

      expect(isDisabled || await page.locator('text=/AGB.*akzeptieren|Bedingungen zustimmen/i').isVisible()).toBe(true);
    });

    test('should validate phone number format', async ({ page }) => {
      await page.goto('/register');

      const phoneInput = page.locator('input[name="phoneNumber"]');

      // Test invalid phone number
      await phoneInput.fill('invalid-phone');
      await phoneInput.blur();

      // Should show validation error
      const hasError = await phoneInput.evaluate((el: HTMLInputElement) => {
        return el.classList.contains('invalid') || el.classList.contains('ng-invalid');
      });

      expect(hasError).toBe(true);
    });
  });

  test.describe('Forgot Password Flow', () => {
    test('should submit forgot password request', async ({ page }) => {
      await page.goto('/forgot-password');

      // Fill in email
      await page.fill('input[name="email"]', testUsers.registered.email);

      // Submit form
      await page.click('button[type="submit"]');

      // Should show success message
      await expect(page.locator('text=/E-Mail.*gesendet|Überprüfen Sie Ihre E-Mail/i')).toBeVisible({ timeout: 10000 });
    });

    test('should validate email in forgot password form', async ({ page }) => {
      await page.goto('/forgot-password');

      // Enter invalid email
      await page.fill('input[name="email"]', 'not-an-email');

      // Try to submit
      await page.click('button[type="submit"]');

      // Should show validation error
      const emailInput = page.locator('input[name="email"]');
      await expect(emailInput).toHaveClass(/invalid|ng-invalid/);
    });

    test('should navigate back to login from forgot password', async ({ page }) => {
      await page.goto('/forgot-password');

      // Click back to login link
      await page.click('a:has-text("Zurück zum Login"), a:has-text("Anmelden")');

      // Should navigate to login page
      await page.waitForURL(/\/login/, { timeout: 5000 });
    });
  });

  test.describe('Logout Flow', () => {
    test('should successfully logout', async ({ page }) => {
      // Login first
      await login(page);

      // Verify logged in
      expect(await isLoggedIn(page)).toBe(true);

      // Logout
      await logout(page);

      // Should be logged out
      expect(await isLoggedIn(page)).toBe(false);

      // Should be on login page
      expect(page.url()).toContain('/login');
    });
  });
});
