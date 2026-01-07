import { Page, Locator, expect } from '@playwright/test';
import { BasePage } from './base.page';

/**
 * Page Object for Vehicle Search (US-1)
 */
export class VehicleSearchPage extends BasePage {
  // Locators
  readonly searchForm: Locator;
  readonly pickupDateInput: Locator;
  readonly returnDateInput: Locator;
  readonly locationSelect: Locator;
  readonly categorySelect: Locator;
  readonly searchButton: Locator;
  readonly vehicleGrid: Locator;
  readonly vehicleCards: Locator;
  readonly loadingIndicator: Locator;
  readonly noResultsMessage: Locator;

  constructor(page: Page) {
    super(page);
    this.searchForm = page.locator(
      'app-vehicle-search form, .vehicle-search-form, .search-form, form',
    );
    // Date inputs use native HTML date input with formControlName on the element
    this.pickupDateInput = page.locator('input#pickupDate, input[formControlName="pickupDate"]');
    this.returnDateInput = page.locator('input#returnDate, input[formControlName="returnDate"]');
    // Custom Angular components wrap native select elements - select by id or within the component
    this.locationSelect = page.locator('ui-select-location select, select#location');
    this.categorySelect = page.locator('ui-select-category select, select#category');
    // Search button - use type="submit" selector, translation key may show as button text
    this.searchButton = page.locator(
      'app-vehicle-search button[type="submit"], .vehicle-search-form button[type="submit"]',
    );
    this.vehicleGrid = page.locator('.vehicle-grid, .vehicles-list, .search-results');
    this.vehicleCards = page.locator('.vehicle-card, [data-testid="vehicle-card"]');
    this.loadingIndicator = page.locator('.loading, .spinner, ui-loading-state');
    // Empty state - check for component or translation key text
    this.noResultsMessage = page.locator(
      'ui-empty-state, text=/keine.*gefunden|no.*found|vehicles\\.emptyState/i',
    );
  }

  /**
   * Navigate to vehicle search page
   */
  async navigate(): Promise<void> {
    await this.goto('/vehicles');
    await this.page.waitForSelector('.search-form, form', { timeout: 10000 });
  }

  /**
   * Set pickup date
   */
  async setPickupDate(date: string): Promise<void> {
    await this.pickupDateInput.fill(date);
  }

  /**
   * Set return date
   */
  async setReturnDate(date: string): Promise<void> {
    await this.returnDateInput.fill(date);
  }

  /**
   * Select pickup location
   */
  async selectLocation(locationCode: string): Promise<void> {
    await this.locationSelect.selectOption(locationCode);
  }

  /**
   * Select vehicle category
   */
  async selectCategory(categoryCode: string): Promise<void> {
    await this.categorySelect.selectOption(categoryCode);
  }

  /**
   * Perform vehicle search with given criteria
   */
  async search(criteria: {
    pickupDate?: string;
    returnDate?: string;
    locationCode?: string;
    categoryCode?: string;
  }): Promise<void> {
    if (criteria.pickupDate) {
      await this.setPickupDate(criteria.pickupDate);
    }
    if (criteria.returnDate) {
      await this.setReturnDate(criteria.returnDate);
    }
    if (criteria.locationCode) {
      await this.selectLocation(criteria.locationCode);
    }
    if (criteria.categoryCode) {
      await this.selectCategory(criteria.categoryCode);
    }
    await this.searchButton.click();
    await this.waitForSearchResults();
  }

  /**
   * Wait for search results to load
   */
  async waitForSearchResults(): Promise<void> {
    await this.page.waitForTimeout(500);
    // Wait for loading to finish
    const loading = await this.isVisible(this.loadingIndicator);
    if (loading) {
      await this.loadingIndicator.waitFor({ state: 'hidden', timeout: 10000 });
    }
  }

  /**
   * Get number of vehicles displayed
   */
  async getVehicleCount(): Promise<number> {
    return this.vehicleCards.count();
  }

  /**
   * Check if no results message is shown
   */
  async hasNoResults(): Promise<boolean> {
    return this.isVisible(this.noResultsMessage);
  }

  /**
   * Get vehicle card by index
   */
  getVehicleCard(index: number): Locator {
    return this.vehicleCards.nth(index);
  }

  /**
   * Click "Book" button on a vehicle card
   */
  async bookVehicle(index: number): Promise<void> {
    const card = this.getVehicleCard(index);
    await card.locator('button:has-text("Buchen"), a:has-text("Buchen")').click();
  }

  /**
   * Get vehicle details from card
   */
  async getVehicleDetails(index: number): Promise<{
    name: string | null;
    price: string | null;
    category: string | null;
  }> {
    const card = this.getVehicleCard(index);
    return {
      name: await card.locator('.vehicle-name, h3, h4').textContent(),
      price: await card.locator('.price, .daily-rate').textContent(),
      category: await card.locator('.category, .vehicle-category').textContent(),
    };
  }

  /**
   * Filter vehicles by price range
   */
  async filterByPriceRange(min: number, max: number): Promise<void> {
    const minInput = this.page.locator('input[formControlName="minPrice"]');
    const maxInput = this.page.locator('input[formControlName="maxPrice"]');

    if (await this.isVisible(minInput)) {
      await minInput.fill(min.toString());
      await maxInput.fill(max.toString());
    }
  }
}
