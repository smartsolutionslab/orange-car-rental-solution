import { Page, Locator } from '@playwright/test';

/**
 * Page Object for Reservations Management (Call Center Portal)
 */
export class ReservationsPage {
  readonly page: Page;
  readonly statusFilter: Locator;
  readonly customerIdFilter: Locator;
  readonly dateFromFilter: Locator;
  readonly dateToFilter: Locator;
  readonly locationFilter: Locator;
  readonly minPriceFilter: Locator;
  readonly maxPriceFilter: Locator;
  readonly applyFiltersButton: Locator;
  readonly clearFiltersButton: Locator;
  readonly activeFiltersCount: Locator;
  readonly sortByDropdown: Locator;
  readonly groupByDropdown: Locator;
  readonly reservationCards: Locator;
  readonly pagination: Locator;
  readonly detailModal: Locator;
  readonly cancelModal: Locator;
  readonly loadingSpinner: Locator;
  readonly errorMessage: Locator;
  readonly successMessage: Locator;

  constructor(page: Page) {
    this.page = page;
    this.statusFilter = page.locator('select[name="status"]');
    this.customerIdFilter = page.locator('input[name="customerId"]');
    this.dateFromFilter = page.locator('input[name="dateFrom"]');
    this.dateToFilter = page.locator('input[name="dateTo"]');
    this.locationFilter = page.locator('input[name="location"]');
    this.minPriceFilter = page.locator('input[name="minPrice"]');
    this.maxPriceFilter = page.locator('input[name="maxPrice"]');
    this.applyFiltersButton = page.locator('button:has-text("Filter anwenden")');
    this.clearFiltersButton = page.locator('button:has-text("Zurücksetzen")');
    this.activeFiltersCount = page.locator('.active-filters-count');
    this.sortByDropdown = page.locator('select[name="sortBy"]');
    this.groupByDropdown = page.locator('select[name="groupBy"]');
    this.reservationCards = page.locator('.reservation-card');
    this.pagination = page.locator('.pagination');
    this.detailModal = page.locator('.detail-modal');
    this.cancelModal = page.locator('.cancel-modal');
    this.loadingSpinner = page.locator('.loading-spinner');
    this.errorMessage = page.locator('.error-message');
    this.successMessage = page.locator('.success-message');
  }

  async goto(): Promise<void> {
    await this.page.goto('/reservations');
  }

  async filterByStatus(status: string): Promise<void> {
    await this.statusFilter.selectOption(status);
    await this.applyFiltersButton.click();
    await this.waitForLoading();
  }

  async filterByCustomerId(customerId: string): Promise<void> {
    await this.customerIdFilter.fill(customerId);
    await this.applyFiltersButton.click();
    await this.waitForLoading();
  }

  async filterByDateRange(from: string, to: string): Promise<void> {
    await this.dateFromFilter.fill(from);
    await this.dateToFilter.fill(to);
    await this.applyFiltersButton.click();
    await this.waitForLoading();
  }

  async filterByLocation(location: string): Promise<void> {
    await this.locationFilter.fill(location);
    await this.applyFiltersButton.click();
    await this.waitForLoading();
  }

  async filterByPriceRange(min: number, max: number): Promise<void> {
    await this.minPriceFilter.fill(min.toString());
    await this.maxPriceFilter.fill(max.toString());
    await this.applyFiltersButton.click();
    await this.waitForLoading();
  }

  async applyMultipleFilters(filters: {
    status?: string;
    customerId?: string;
    dateFrom?: string;
    dateTo?: string;
    location?: string;
    minPrice?: number;
    maxPrice?: number;
  }): Promise<void> {
    if (filters.status) await this.statusFilter.selectOption(filters.status);
    if (filters.customerId) await this.customerIdFilter.fill(filters.customerId);
    if (filters.dateFrom) await this.dateFromFilter.fill(filters.dateFrom);
    if (filters.dateTo) await this.dateToFilter.fill(filters.dateTo);
    if (filters.location) await this.locationFilter.fill(filters.location);
    if (filters.minPrice !== undefined) await this.minPriceFilter.fill(filters.minPrice.toString());
    if (filters.maxPrice !== undefined) await this.maxPriceFilter.fill(filters.maxPrice.toString());

    await this.applyFiltersButton.click();
    await this.waitForLoading();
  }

  async clearAllFilters(): Promise<void> {
    await this.clearFiltersButton.click();
    await this.waitForLoading();
  }

  async getActiveFiltersCount(): Promise<number> {
    if (await this.activeFiltersCount.isVisible()) {
      const text = await this.activeFiltersCount.textContent();
      return parseInt(text || '0', 10);
    }
    return 0;
  }

  async sortBy(field: string, order?: 'asc' | 'desc'): Promise<void> {
    await this.sortByDropdown.selectOption(field);
    await this.waitForLoading();

    // If order specified and needs to be toggled
    if (order) {
      const currentOrder = await this.getCurrentSortOrder();
      if (currentOrder !== order) {
        await this.sortByDropdown.selectOption(field); // Click again to toggle
        await this.waitForLoading();
      }
    }
  }

  async getCurrentSortOrder(): Promise<string> {
    const sortOrderIndicator = this.page.locator('.sort-order-indicator');
    if (await sortOrderIndicator.isVisible()) {
      const text = await sortOrderIndicator.textContent();
      return text?.includes('↑') ? 'asc' : 'desc';
    }
    return 'desc'; // default
  }

  async groupBy(grouping: string): Promise<void> {
    await this.groupByDropdown.selectOption(grouping);
    // Grouping doesn't reload, just reorganizes
    await this.page.waitForTimeout(500);
  }

  async getReservationsCount(): Promise<number> {
    return await this.reservationCards.count();
  }

  async getGroupCount(): Promise<number> {
    const groups = this.page.locator('.reservation-group');
    return await groups.count();
  }

  async getGroupNames(): Promise<string[]> {
    const groupHeaders = this.page.locator('.group-header');
    const count = await groupHeaders.count();
    const names: string[] = [];
    for (let i = 0; i < count; i++) {
      const text = await groupHeaders.nth(i).textContent();
      if (text) names.push(text.trim());
    }
    return names;
  }

  async viewReservationDetails(index: number): Promise<void> {
    const detailButtons = this.page.locator('button:has-text("Details")');
    await detailButtons.nth(index).click();
    await this.detailModal.waitFor({ state: 'visible' });
  }

  async confirmReservation(index: number): Promise<void> {
    const confirmButtons = this.page.locator('button:has-text("Bestätigen")');
    await confirmButtons.nth(index).click();

    // Handle confirmation dialog
    this.page.once('dialog', dialog => dialog.accept());
    await this.waitForLoading();
  }

  async cancelReservation(index: number, reason: string): Promise<void> {
    const cancelButtons = this.page.locator('button:has-text("Stornieren")');
    await cancelButtons.nth(index).click();
    await this.cancelModal.waitFor({ state: 'visible' });

    await this.cancelModal.locator('textarea[name="reason"]').fill(reason);
    await this.cancelModal.locator('button:has-text("Stornieren")').click();
    await this.waitForLoading();
  }

  async goToPage(pageNumber: number): Promise<void> {
    const pageButton = this.pagination.locator(`button:has-text("${pageNumber}")`);
    await pageButton.click();
    await this.waitForLoading();
  }

  async goToNextPage(): Promise<void> {
    const nextButton = this.pagination.locator('button:has-text("Weiter"), button:has-text("Next")');
    await nextButton.click();
    await this.waitForLoading();
  }

  async goToPreviousPage(): Promise<void> {
    const prevButton = this.pagination.locator('button:has-text("Zurück"), button:has-text("Previous")');
    await prevButton.click();
    await this.waitForLoading();
  }

  async getCurrentPage(): Promise<number> {
    const activePage = this.pagination.locator('.page-active');
    const text = await activePage.textContent();
    return parseInt(text || '1', 10);
  }

  async getTotalPages(): Promise<number> {
    const pageButtons = this.pagination.locator('button[data-page]');
    return await pageButtons.count();
  }

  async waitForLoading(): Promise<void> {
    await this.loadingSpinner.waitFor({ state: 'hidden', timeout: 15000 });
  }

  async getErrorMessage(): Promise<string | null> {
    if (await this.errorMessage.isVisible()) {
      return await this.errorMessage.textContent();
    }
    return null;
  }

  async getSuccessMessage(): Promise<string | null> {
    if (await this.successMessage.isVisible()) {
      return await this.successMessage.textContent();
    }
    return null;
  }

  async getReservationById(reservationId: string): Promise<Locator | null> {
    const card = this.page.locator(`.reservation-card:has-text("${reservationId.substring(0, 8)}")`);
    if (await card.isVisible()) {
      return card;
    }
    return null;
  }

  async getReservationStatus(index: number): Promise<string | null> {
    const statusBadges = this.page.locator('.status-badge');
    if (await statusBadges.count() > index) {
      return await statusBadges.nth(index).textContent();
    }
    return null;
  }

  async isURLSyncedWithFilters(): Promise<boolean> {
    const url = this.page.url();
    return url.includes('?');
  }

  async getURLParam(param: string): Promise<string | null> {
    const url = new URL(this.page.url());
    return url.searchParams.get(param);
  }
}
