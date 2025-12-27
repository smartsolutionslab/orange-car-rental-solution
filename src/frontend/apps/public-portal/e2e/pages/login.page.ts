import { Page, Locator } from '@playwright/test';
import { BasePage } from './base.page';
import { testUsers } from '../fixtures/test-data';

/**
 * Page Object for Login/Authentication (US-3)
 */
export class LoginPage extends BasePage {
  readonly emailInput: Locator;
  readonly passwordInput: Locator;
  readonly submitButton: Locator;
  readonly registerLink: Locator;
  readonly forgotPasswordLink: Locator;
  readonly errorMessage: Locator;
  readonly logoutButton: Locator;

  constructor(page: Page) {
    super(page);
    this.emailInput = page.locator('input[name="email"], input[formControlName="email"]');
    this.passwordInput = page.locator('input[name="password"], input[formControlName="password"]');
    this.submitButton = page.locator('button[type="submit"]');
    this.registerLink = page.locator('a:has-text("Registrieren"), a[href="/register"]');
    this.forgotPasswordLink = page.locator('a:has-text("Passwort vergessen")');
    this.errorMessage = page.locator('.error, .alert-danger, .login-error');
    this.logoutButton = page.locator('button:has-text("Abmelden")');
  }

  /**
   * Navigate to login page
   */
  async navigate(): Promise<void> {
    await this.goto('/login');
    await this.page.waitForSelector('form', { timeout: 10000 });
  }

  /**
   * Login with credentials
   */
  async login(email?: string, password?: string): Promise<void> {
    const user = testUsers.registered;
    await this.emailInput.fill(email || user.email);
    await this.passwordInput.fill(password || user.password);
    await this.submitButton.click();
    await this.waitForNavigation(/^\/$|\/vehicles/, 10000);
  }

  /**
   * Attempt login (without waiting for success)
   */
  async attemptLogin(email: string, password: string): Promise<void> {
    await this.emailInput.fill(email);
    await this.passwordInput.fill(password);
    await this.submitButton.click();
    await this.page.waitForTimeout(1000);
  }

  /**
   * Check if login error is displayed
   */
  async hasLoginError(): Promise<boolean> {
    return this.isVisible(this.errorMessage);
  }

  /**
   * Get login error message
   */
  async getErrorMessage(): Promise<string | null> {
    if (await this.hasLoginError()) {
      return this.errorMessage.textContent();
    }
    return null;
  }

  /**
   * Click register link
   */
  async goToRegister(): Promise<void> {
    await this.registerLink.click();
    await this.waitForNavigation(/\/register/);
  }

  /**
   * Click forgot password link
   */
  async goToForgotPassword(): Promise<void> {
    await this.forgotPasswordLink.click();
    await this.waitForNavigation(/\/forgot-password/);
  }

  /**
   * Check if user is logged in
   */
  async isLoggedIn(): Promise<boolean> {
    return this.isVisible(this.logoutButton);
  }

  /**
   * Logout current user
   */
  async logout(): Promise<void> {
    await this.logoutButton.click();
    await this.waitForNavigation(/\/login/);
  }
}

/**
 * Page Object for Registration
 */
export class RegisterPage extends BasePage {
  readonly firstNameInput: Locator;
  readonly lastNameInput: Locator;
  readonly emailInput: Locator;
  readonly phoneInput: Locator;
  readonly dateOfBirthInput: Locator;
  readonly passwordInput: Locator;
  readonly confirmPasswordInput: Locator;
  readonly acceptTermsCheckbox: Locator;
  readonly submitButton: Locator;
  readonly loginLink: Locator;
  readonly errorMessage: Locator;

  constructor(page: Page) {
    super(page);
    this.firstNameInput = page.locator('input[name="firstName"], input[formControlName="firstName"]');
    this.lastNameInput = page.locator('input[name="lastName"], input[formControlName="lastName"]');
    this.emailInput = page.locator('input[name="email"], input[formControlName="email"]');
    this.phoneInput = page.locator('input[name="phoneNumber"], input[formControlName="phoneNumber"]');
    this.dateOfBirthInput = page.locator('input[name="dateOfBirth"], input[formControlName="dateOfBirth"]');
    this.passwordInput = page.locator('input[name="password"], input[formControlName="password"]');
    this.confirmPasswordInput = page.locator('input[name="confirmPassword"], input[formControlName="confirmPassword"]');
    this.acceptTermsCheckbox = page.locator('input[type="checkbox"][formControlName="acceptTerms"]');
    this.submitButton = page.locator('button[type="submit"]');
    this.loginLink = page.locator('a:has-text("Anmelden"), a[href="/login"]');
    this.errorMessage = page.locator('.error, .alert-danger');
  }

  /**
   * Navigate to registration page
   */
  async navigate(): Promise<void> {
    await this.goto('/register');
    await this.page.waitForSelector('form', { timeout: 10000 });
  }

  /**
   * Register a new user
   */
  async register(userData: {
    firstName: string;
    lastName: string;
    email: string;
    phoneNumber: string;
    dateOfBirth: string;
    password: string;
    confirmPassword: string;
  }): Promise<void> {
    await this.firstNameInput.fill(userData.firstName);
    await this.lastNameInput.fill(userData.lastName);
    await this.emailInput.fill(userData.email);
    await this.phoneInput.fill(userData.phoneNumber);
    await this.dateOfBirthInput.fill(userData.dateOfBirth);
    await this.passwordInput.fill(userData.password);
    await this.confirmPasswordInput.fill(userData.confirmPassword);
    await this.acceptTermsCheckbox.check();
    await this.submitButton.click();
    await this.waitForNavigation(/^\/$/, 15000);
  }

  /**
   * Check for registration error
   */
  async hasError(): Promise<boolean> {
    return this.isVisible(this.errorMessage);
  }

  /**
   * Go to login page
   */
  async goToLogin(): Promise<void> {
    await this.loginLink.click();
    await this.waitForNavigation(/\/login/);
  }
}
