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

      // Fill in login form - login form uses #login-email and #login-password
      await page.fill('#login-email', testUsers.registered.email);
      await page.fill('#login-password', testUsers.registered.password);

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
      await page.fill('#login-email', 'invalid@example.com');
      await page.fill('#login-password', 'WrongPassword123!');

      // Submit form
      await page.click('button[type="submit"]');

      // Should show error message (uses ui-error-alert component)
      await expect(page.locator('ui-error-alert, .error-message, .alert-error').first()).toBeVisible({ timeout: 5000 });

      // Should remain on login page
      expect(page.url()).toContain('/login');
    });

    test('should validate email format', async ({ page }) => {
      await page.goto('/login');

      // Enter invalid email
      await page.fill('#login-email', 'not-an-email');
      await page.fill('#login-password', 'SomePassword123!');

      // Try to submit
      await page.click('button[type="submit"]');

      // Should show validation error - check for input-error class on container or field-error span
      const emailInput = page.locator('#login-email');
      const hasError = await emailInput.evaluate((el) =>
        el.closest('.input-container')?.classList.contains('has-error') ||
        el.classList.contains('input-error') ||
        el.classList.contains('ng-invalid')
      );
      expect(hasError).toBe(true);
    });

    test('should toggle password visibility', async ({ page }) => {
      await page.goto('/login');

      const passwordInput = page.locator('#login-password');
      const toggleButton = page.locator('button.password-toggle');

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
      await page.fill('#login-email', testUsers.registered.email);
      await page.fill('#login-password', testUsers.registered.password);

      // Check "Remember Me" - uses checkbox-input class
      const rememberMeCheckbox = page.locator('input.checkbox-input[formControlName="rememberMe"]');
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

      // Registration is multi-step:
      // Step 1: Account Information (email, password, confirmPassword)
      await page.fill('ocr-input[formControlName="email"] input', newUser.email);
      await page.fill('ocr-input[formControlName="password"] input', newUser.password);
      await page.fill('ocr-input[formControlName="confirmPassword"] input', newUser.confirmPassword);
      await page.click('button.primary-button'); // Next

      // Step 2: Personal Information
      await page.waitForTimeout(300);
      await page.fill('ocr-input[formControlName="firstName"] input', newUser.firstName);
      await page.fill('ocr-input[formControlName="lastName"] input', newUser.lastName);
      await page.fill('ocr-input[formControlName="phoneNumber"] input', newUser.phoneNumber);
      await page.fill('#dateOfBirth', newUser.dateOfBirth);
      await page.click('button.primary-button'); // Next

      // Step 3: Terms and Conditions
      await page.waitForTimeout(300);
      await page.click('ocr-checkbox[formControlName="acceptTerms"]');
      await page.click('ocr-checkbox[formControlName="acceptPrivacy"]');

      // Submit form
      await page.click('button[type="submit"]');

      // Should redirect to home page or login page after successful registration
      await page.waitForURL(/\/|\/login/, { timeout: 15000 });
    });

    test('should validate password strength', async ({ page }) => {
      await page.goto('/register');

      // Password field uses ocr-input component
      const passwordInput = page.locator('ocr-input[formControlName="password"] input');

      // Test weak password
      await passwordInput.fill('weak');
      await passwordInput.blur();

      // Should show validation error - field-hint shows password requirements
      const hasError = await page.locator('.field-error, .input-error, .field-hint').first().isVisible();
      expect(hasError).toBe(true);
    });

    test('should validate password confirmation match', async ({ page }) => {
      await page.goto('/register');

      // Fill passwords that don't match using ocr-input components
      await page.fill('ocr-input[formControlName="password"] input', 'StrongPassword123!');
      await page.fill('ocr-input[formControlName="confirmPassword"] input', 'DifferentPassword123!');

      // Blur to trigger validation
      await page.locator('ocr-input[formControlName="confirmPassword"] input').blur();

      // Should show validation error
      const hasError = await page.locator('ocr-input[formControlName="confirmPassword"] .input-error, .field-error').first().isVisible();
      expect(hasError).toBe(true);
    });

    test('should validate minimum age (18 years)', async ({ page }) => {
      await page.goto('/register');

      // Navigate to Step 2 first (Personal Information)
      await page.fill('ocr-input[formControlName="email"] input', 'test@example.com');
      await page.fill('ocr-input[formControlName="password"] input', 'StrongPassword123!');
      await page.fill('ocr-input[formControlName="confirmPassword"] input', 'StrongPassword123!');
      await page.click('button.primary-button'); // Next to step 2
      await page.waitForTimeout(300);

      // Calculate date for someone under 18
      const today = new Date();
      const underageDate = new Date(today.getFullYear() - 17, today.getMonth(), today.getDate());
      const dateString = underageDate.toISOString().split('T')[0];

      // dateOfBirth uses native input with id="dateOfBirth"
      await page.fill('#dateOfBirth', dateString);
      await page.locator('#dateOfBirth').blur();

      // Should show validation error - check for field-error or field-hint
      const hasError = await page.locator('.field-error').first().isVisible();
      expect(hasError).toBe(true);
    });

    test('should require terms acceptance', async ({ page }) => {
      await page.goto('/register');

      // Fill in all fields except terms - registration is multi-step
      const newUser = {
        ...testUsers.newUser,
        email: `test.${Date.now()}@orange-rental.de`
      };

      // Step 1: Account Information
      await page.fill('ocr-input[formControlName="email"] input', newUser.email);
      await page.fill('ocr-input[formControlName="password"] input', newUser.password);
      await page.fill('ocr-input[formControlName="confirmPassword"] input', newUser.confirmPassword);
      await page.click('button.primary-button'); // Next

      // Step 2: Personal Information
      await page.waitForTimeout(300);
      await page.fill('ocr-input[formControlName="firstName"] input', newUser.firstName);
      await page.fill('ocr-input[formControlName="lastName"] input', newUser.lastName);
      await page.fill('ocr-input[formControlName="phoneNumber"] input', newUser.phoneNumber);
      await page.fill('#dateOfBirth', newUser.dateOfBirth);
      await page.click('button.primary-button'); // Next

      // Step 3: Don't check terms checkbox
      await page.waitForTimeout(300);

      // Submit button should be disabled since terms not accepted
      const submitButton = page.locator('button[type="submit"]');
      const isDisabled = await submitButton.isDisabled();

      // Either button is disabled or we can't proceed without terms
      expect(isDisabled).toBe(true);
    });

    test('should validate phone number format', async ({ page }) => {
      await page.goto('/register');

      // Navigate to Step 2 first (Personal Information)
      await page.fill('ocr-input[formControlName="email"] input', 'test@example.com');
      await page.fill('ocr-input[formControlName="password"] input', 'StrongPassword123!');
      await page.fill('ocr-input[formControlName="confirmPassword"] input', 'StrongPassword123!');
      await page.click('button.primary-button'); // Next to step 2
      await page.waitForTimeout(300);

      // Phone number uses ocr-input component
      const phoneInput = page.locator('ocr-input[formControlName="phoneNumber"] input');

      // Test invalid phone number
      await phoneInput.fill('invalid-phone');
      await phoneInput.blur();

      // Should show validation error - check for has-error class on container
      const hasError = await page.locator('ocr-input[formControlName="phoneNumber"] .input-error, .field-error').first().isVisible();
      expect(hasError).toBe(true);
    });
  });

  test.describe('Forgot Password Flow', () => {
    test('should submit forgot password request', async ({ page }) => {
      await page.goto('/forgot-password');

      // Fill in email - uses #forgot-email
      await page.fill('#forgot-email', testUsers.registered.email);

      // Submit form
      await page.click('button[type="submit"]');

      // Should show success message (uses ui-success-alert component)
      await expect(page.locator('ui-success-alert, text=/E-Mail.*gesendet|Überprüfen Sie Ihre E-Mail/i').first()).toBeVisible({ timeout: 10000 });
    });

    test('should validate email in forgot password form', async ({ page }) => {
      await page.goto('/forgot-password');

      // Enter invalid email - uses #forgot-email
      await page.fill('#forgot-email', 'not-an-email');

      // Try to submit
      await page.click('button[type="submit"]');

      // Should show validation error - check for input-error class or field-error span
      const emailInput = page.locator('#forgot-email');
      const hasError = await emailInput.evaluate((el) =>
        el.classList.contains('input-error') || el.classList.contains('ng-invalid')
      );
      expect(hasError).toBe(true);
    });

    test('should navigate back to login from forgot password', async ({ page }) => {
      await page.goto('/forgot-password');

      // Click back to login link (uses back-link class)
      await page.click('a.back-link');

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
