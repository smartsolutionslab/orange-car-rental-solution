import { Page, Locator, expect } from '@playwright/test';

/**
 * Base Page Object with common functionality
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
   * Get input value by form control name
   */
  async getInputValue(controlName: string): Promise<string> {
    return this.page.inputValue(`input[formControlName="${controlName}"]`);
  }
}
