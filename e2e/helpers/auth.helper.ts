import { Page } from '@playwright/test';

/**
 * Authentication helper for E2E tests
 * Handles login flows for both portals
 */
export class AuthHelper {
  constructor(private page: Page) {}

  /**
   * Login to Keycloak SSO
   */
  async login(username: string, password: string): Promise<void> {
    // Wait for Keycloak login redirect
    await this.page.waitForURL('**/realms/orange-car-rental/**', { timeout: 10000 });

    // Fill in credentials
    await this.page.fill('input[name="username"]', username);
    await this.page.fill('input[name="password"]', password);

    // Click login button
    await this.page.click('input[type="submit"]');

    // Wait for redirect back to application
    await this.page.waitForURL('**/');
  }

  /**
   * Login as customer (public portal)
   */
  async loginAsCustomer(): Promise<void> {
    await this.login(
      process.env.TEST_CUSTOMER_USERNAME || 'test-customer@example.com',
      process.env.TEST_CUSTOMER_PASSWORD || 'Test123!'
    );
  }

  /**
   * Login as call center agent
   */
  async loginAsAgent(): Promise<void> {
    await this.login(
      process.env.TEST_AGENT_USERNAME || 'test-agent@example.com',
      process.env.TEST_AGENT_PASSWORD || 'Test123!'
    );
  }

  /**
   * Logout from the application
   */
  async logout(): Promise<void> {
    await this.page.click('button:has-text("Abmelden"), button:has-text("Logout")');
    await this.page.waitForURL('**/realms/orange-car-rental/protocol/openid-connect/logout**');
  }

  /**
   * Check if user is authenticated
   */
  async isAuthenticated(): Promise<boolean> {
    const logoutButton = await this.page.locator('button:has-text("Abmelden"), button:has-text("Logout")');
    return await logoutButton.isVisible();
  }

  /**
   * Get current user name from UI
   */
  async getCurrentUserName(): Promise<string | null> {
    const userElement = await this.page.locator('.user-name, .user-profile');
    if (await userElement.isVisible()) {
      return await userElement.textContent();
    }
    return null;
  }
}
