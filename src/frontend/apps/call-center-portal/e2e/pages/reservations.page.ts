import { Page, Locator } from '@playwright/test';
import { BasePage } from './base.page';

/**
 * Page Object for Reservations List (US-7, US-8)
 */
export class ReservationsPage extends BasePage {
  // Statistics cards
  readonly statsSection: Locator;
  readonly totalCountStat: Locator;
  readonly todayCountStat: Locator;
  readonly pendingCountStat: Locator;

  // Table
  readonly reservationsTable: Locator;
  readonly tableRows: Locator;
  readonly tableHeaders: Locator;

  // Filters
  readonly filterSection: Locator;
  readonly statusFilter: Locator;
  readonly dateFromFilter: Locator;
  readonly dateToFilter: Locator;
  readonly searchInput: Locator;
  readonly applyFiltersButton: Locator;
  readonly clearFiltersButton: Locator;

  // Grouping
  readonly groupBySelect: Locator;
  readonly groupHeaders: Locator;

  // Sorting
  readonly sortableHeaders: Locator;

  // Pagination
  readonly paginationSection: Locator;
  readonly previousPageButton: Locator;
  readonly nextPageButton: Locator;
  readonly pageInfo: Locator;
  readonly pageSizeSelect: Locator;

  // Action buttons in table
  readonly viewButtons: Locator;
  readonly confirmButtons: Locator;
  readonly cancelButtons: Locator;

  // Modals
  readonly detailModal: Locator;
  readonly confirmModal: Locator;
  readonly cancelModal: Locator;

  // States
  readonly loadingIndicator: Locator;
  readonly emptyState: Locator;
  readonly errorState: Locator;

  constructor(page: Page) {
    super(page);

    // Stats
    this.statsSection = page.locator('.stats-cards, .statistics, .dashboard-stats');
    this.totalCountStat = page.locator('text=/Gesamt|Total/i').locator('..').locator('.stat-value, .count');
    this.todayCountStat = page.locator('text=/Heute|Today/i').locator('..').locator('.stat-value, .count');
    this.pendingCountStat = page.locator('text=/Ausstehend|Pending/i').locator('..').locator('.stat-value, .count');

    // Table
    this.reservationsTable = page.locator('table, .reservations-table');
    this.tableRows = page.locator('tbody tr, .reservation-row');
    this.tableHeaders = page.locator('thead th, .table-header');

    // Filters
    this.filterSection = page.locator('.filter-section, .filters');
    this.statusFilter = page.locator('select[formControlName="status"], #statusFilter');
    this.dateFromFilter = page.locator('input[formControlName="dateFrom"], #dateFromFilter');
    this.dateToFilter = page.locator('input[formControlName="dateTo"], #dateToFilter');
    this.searchInput = page.locator('input[formControlName="search"], input[placeholder*="Suchen"]');
    this.applyFiltersButton = page.locator('button:has-text("Filtern"), button:has-text("Anwenden")');
    this.clearFiltersButton = page.locator('button:has-text("Zurücksetzen"), button:has-text("Filter löschen")');

    // Grouping
    this.groupBySelect = page.locator('select[formControlName="groupBy"], #groupBySelect');
    this.groupHeaders = page.locator('.group-header, .group-title');

    // Sorting
    this.sortableHeaders = page.locator('th[data-sortable], th.sortable');

    // Pagination
    this.paginationSection = page.locator('.pagination');
    this.previousPageButton = page.locator('button:has-text("Zurück"), .pagination-prev');
    this.nextPageButton = page.locator('button:has-text("Weiter"), .pagination-next');
    this.pageInfo = page.locator('.page-info, .pagination-info');
    this.pageSizeSelect = page.locator('select[formControlName="pageSize"], #pageSizeSelect');

    // Actions
    this.viewButtons = page.locator('button:has-text("Ansehen"), button:has-text("Details")');
    this.confirmButtons = page.locator('button:has-text("Bestätigen")');
    this.cancelButtons = page.locator('button:has-text("Stornieren")');

    // Modals
    this.detailModal = page.locator('.detail-modal, [data-testid="detail-modal"]');
    this.confirmModal = page.locator('.confirm-modal, [data-testid="confirm-modal"]');
    this.cancelModal = page.locator('.cancel-modal, [data-testid="cancel-modal"]');

    // States
    this.loadingIndicator = page.locator('.loading, .spinner');
    this.emptyState = page.locator('.empty-state, text=/keine.*Reservierungen/i');
    this.errorState = page.locator('.error-state, .alert-danger');
  }

  /**
   * Navigate to reservations page
   */
  async navigate(): Promise<void> {
    await this.goto('/reservations');
    await this.page.waitForTimeout(2000);
    await this.waitForLoading();
  }

  /**
   * Get number of reservations displayed
   */
  async getReservationCount(): Promise<number> {
    return this.tableRows.count();
  }

  /**
   * Filter by status
   */
  async filterByStatus(status: string): Promise<void> {
    await this.statusFilter.selectOption(status);
    await this.page.waitForTimeout(500);
    await this.waitForLoading();
  }

  /**
   * Filter by date range
   */
  async filterByDateRange(from: string, to: string): Promise<void> {
    await this.dateFromFilter.fill(from);
    await this.dateToFilter.fill(to);
    if (await this.isVisible(this.applyFiltersButton)) {
      await this.applyFiltersButton.click();
    }
    await this.page.waitForTimeout(500);
    await this.waitForLoading();
  }

  /**
   * Search reservations
   */
  async search(query: string): Promise<void> {
    await this.searchInput.fill(query);
    await this.page.waitForTimeout(500);
    await this.waitForLoading();
  }

  /**
   * Clear all filters
   */
  async clearFilters(): Promise<void> {
    if (await this.isVisible(this.clearFiltersButton)) {
      await this.clearFiltersButton.click();
      await this.page.waitForTimeout(500);
      await this.waitForLoading();
    }
  }

  /**
   * Group reservations by a field
   */
  async groupBy(field: string): Promise<void> {
    await this.groupBySelect.selectOption(field);
    await this.page.waitForTimeout(500);
  }

  /**
   * Get number of groups displayed
   */
  async getGroupCount(): Promise<number> {
    return this.groupHeaders.count();
  }

  /**
   * Sort by column
   */
  async sortByColumn(columnName: string): Promise<void> {
    const header = this.page.locator(`th:has-text("${columnName}")`);
    await header.click();
    await this.page.waitForTimeout(500);
  }

  /**
   * Go to next page
   */
  async nextPage(): Promise<void> {
    await this.nextPageButton.click();
    await this.page.waitForTimeout(500);
    await this.waitForLoading();
  }

  /**
   * Go to previous page
   */
  async previousPage(): Promise<void> {
    await this.previousPageButton.click();
    await this.page.waitForTimeout(500);
    await this.waitForLoading();
  }

  /**
   * Change page size
   */
  async setPageSize(size: number): Promise<void> {
    await this.pageSizeSelect.selectOption(size.toString());
    await this.page.waitForTimeout(500);
    await this.waitForLoading();
  }

  /**
   * View reservation details
   */
  async viewReservation(index: number): Promise<void> {
    await this.viewButtons.nth(index).click();
    await this.page.waitForTimeout(500);
  }

  /**
   * Confirm a reservation
   */
  async confirmReservation(index: number): Promise<void> {
    await this.confirmButtons.nth(index).click();
    await this.page.waitForTimeout(500);
  }

  /**
   * Cancel a reservation
   */
  async cancelReservation(index: number): Promise<void> {
    await this.cancelButtons.nth(index).click();
    await this.page.waitForTimeout(500);
  }

  /**
   * Check if detail modal is visible
   */
  async isDetailModalVisible(): Promise<boolean> {
    return this.isVisible(this.detailModal);
  }

  /**
   * Close detail modal
   */
  async closeDetailModal(): Promise<void> {
    await this.page.locator('.detail-modal button:has-text("Schließen")').click();
    await this.page.waitForTimeout(300);
  }

  /**
   * Get reservation status from row
   */
  async getReservationStatus(index: number): Promise<string | null> {
    const row = this.tableRows.nth(index);
    const statusBadge = row.locator('.status-badge, .badge');
    return statusBadge.textContent();
  }

  /**
   * Check if empty state is shown
   */
  async hasEmptyState(): Promise<boolean> {
    return this.isVisible(this.emptyState);
  }

  /**
   * Check if stats are visible
   */
  async areStatsVisible(): Promise<boolean> {
    return this.isVisible(this.statsSection);
  }
}
