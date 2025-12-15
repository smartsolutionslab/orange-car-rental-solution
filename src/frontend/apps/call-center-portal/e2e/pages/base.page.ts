import { Page, Locator } from '@playwright/test';

/**
 * Base Page Object with common functionality for Call Center Portal
 */
export abstract class BasePage {
  constructor(protected readonly page: Page) {}

  /**
   * Navigate to a specific path
   */
  async goto(path: string): Promise<void> {
    await this.page.goto(path);
  }

  /**
   * Wait for page to be fully loaded
   */
  async waitForLoad(): Promise<void> {
    await this.page.waitForLoadState('networkidle');
  }

  /**
   * Get current URL
   */
  getUrl(): string {
    return this.page.url();
  }

  /**
   * Check if element is visible
   */
  async isVisible(locator: Locator): Promise<boolean> {
    return locator.isVisible().catch(() => false);
  }

  /**
   * Wait for navigation to complete
   */
  async waitForNavigation(urlPattern: RegExp, timeout = 10000): Promise<void> {
    await this.page.waitForURL(urlPattern, { timeout });
  }

  /**
   * Get toast/notification message
   */
  async getToastMessage(): Promise<string | null> {
    const toast = this.page.locator('.toast, .notification, .alert');
    if (await this.isVisible(toast)) {
      return toast.textContent();
    }
    return null;
  }

  /**
   * Click button by text
   */
  async clickButton(text: string): Promise<void> {
    await this.page.click(`button:has-text("${text}")`);
  }

  /**
   * Fill input by form control name
   */
  async fillInput(controlName: string, value: string): Promise<void> {
    await this.page.fill(`input[formControlName="${controlName}"]`, value);
  }

  /**
   * Select option by form control name
   */
  async selectOption(controlName: string, value: string): Promise<void> {
    await this.page.selectOption(`select[formControlName="${controlName}"]`, value);
  }

  /**
   * Wait for loading to complete
   */
  async waitForLoading(): Promise<void> {
    const loading = this.page.locator('.loading, .spinner');
    if (await this.isVisible(loading)) {
      await loading.waitFor({ state: 'hidden', timeout: 10000 });
    }
  }
}
