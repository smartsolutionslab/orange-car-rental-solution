import { Page, Locator } from '@playwright/test';
import { BasePage } from './base.page';

/**
 * Page Object for Vehicle Dashboard (US-10)
 */
export class VehicleDashboardPage extends BasePage {
  // Dashboard overview
  readonly dashboardSection: Locator;
  readonly totalVehiclesCount: Locator;
  readonly availableCount: Locator;
  readonly rentedCount: Locator;
  readonly maintenanceCount: Locator;

  // Fleet status chart/visualization
  readonly fleetChart: Locator;
  readonly statusPieChart: Locator;
  readonly locationBarChart: Locator;

  // Vehicle list/table
  readonly vehicleTable: Locator;
  readonly vehicleRows: Locator;
  readonly vehicleCards: Locator;

  // Filters
  readonly statusFilter: Locator;
  readonly locationFilter: Locator;
  readonly categoryFilter: Locator;
  readonly searchInput: Locator;

  // Vehicle details
  readonly vehicleDetailPanel: Locator;
  readonly vehicleName: Locator;
  readonly vehicleStatus: Locator;
  readonly vehicleLocation: Locator;
  readonly vehicleCategory: Locator;
  readonly vehicleLicensePlate: Locator;
  readonly currentReservation: Locator;

  // Actions
  readonly viewDetailsButtons: Locator;
  readonly changeStatusButton: Locator;
  readonly assignLocationButton: Locator;

  // Status change modal
  readonly statusModal: Locator;
  readonly newStatusSelect: Locator;
  readonly statusNotes: Locator;
  readonly confirmStatusButton: Locator;

  // States
  readonly loadingIndicator: Locator;
  readonly emptyState: Locator;
  readonly errorState: Locator;

  constructor(page: Page) {
    super(page);

    // Dashboard
    this.dashboardSection = page.locator('.dashboard, .vehicle-dashboard');
    this.totalVehiclesCount = page.locator('[data-stat="total"], text=/Gesamt.*Fahrzeuge/i');
    this.availableCount = page.locator('[data-stat="available"], text=/Verfügbar/i');
    this.rentedCount = page.locator('[data-stat="rented"], text=/Vermietet/i');
    this.maintenanceCount = page.locator('[data-stat="maintenance"], text=/Wartung/i');

    // Charts
    this.fleetChart = page.locator('.fleet-chart, .chart');
    this.statusPieChart = page.locator('.status-pie-chart, [data-chart="status"]');
    this.locationBarChart = page.locator('.location-bar-chart, [data-chart="location"]');

    // List/Table
    this.vehicleTable = page.locator('table, .vehicle-table');
    this.vehicleRows = page.locator('tbody tr, .vehicle-row');
    this.vehicleCards = page.locator('.vehicle-card, [data-testid="vehicle-card"]');

    // Filters
    this.statusFilter = page.locator('select[formControlName="status"], #statusFilter');
    this.locationFilter = page.locator('select[formControlName="location"], #locationFilter');
    this.categoryFilter = page.locator('select[formControlName="category"], #categoryFilter');
    this.searchInput = page.locator('input[formControlName="search"], input[placeholder*="Suchen"]');

    // Details
    this.vehicleDetailPanel = page.locator('.vehicle-details, .detail-panel');
    this.vehicleName = page.locator('.vehicle-name, [data-field="name"]');
    this.vehicleStatus = page.locator('.vehicle-status, [data-field="status"]');
    this.vehicleLocation = page.locator('.vehicle-location, [data-field="location"]');
    this.vehicleCategory = page.locator('.vehicle-category, [data-field="category"]');
    this.vehicleLicensePlate = page.locator('.license-plate, [data-field="licensePlate"]');
    this.currentReservation = page.locator('.current-reservation, [data-section="reservation"]');

    // Actions
    this.viewDetailsButtons = page.locator('button:has-text("Details"), button:has-text("Ansehen")');
    this.changeStatusButton = page.locator('button:has-text("Status ändern")');
    this.assignLocationButton = page.locator('button:has-text("Standort zuweisen")');

    // Status modal
    this.statusModal = page.locator('.status-modal, [data-testid="status-modal"]');
    this.newStatusSelect = page.locator('select[formControlName="newStatus"]');
    this.statusNotes = page.locator('textarea[formControlName="notes"]');
    this.confirmStatusButton = page.locator('button:has-text("Status ändern bestätigen")');

    // States
    this.loadingIndicator = page.locator('.loading, .spinner');
    this.emptyState = page.locator('.empty-state');
    this.errorState = page.locator('.error-state, .alert-danger');
  }

  /**
   * Navigate to vehicle dashboard
   */
  async navigate(): Promise<void> {
    await this.goto('/vehicles');
    await this.page.waitForTimeout(1000);
    await this.waitForLoading();
  }

  /**
   * Get vehicle count
   */
  async getVehicleCount(): Promise<number> {
    const rows = await this.vehicleRows.count();
    const cards = await this.vehicleCards.count();
    return Math.max(rows, cards);
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
   * Filter by location
   */
  async filterByLocation(locationCode: string): Promise<void> {
    await this.locationFilter.selectOption(locationCode);
    await this.page.waitForTimeout(500);
    await this.waitForLoading();
  }

  /**
   * Filter by category
   */
  async filterByCategory(categoryCode: string): Promise<void> {
    await this.categoryFilter.selectOption(categoryCode);
    await this.page.waitForTimeout(500);
    await this.waitForLoading();
  }

  /**
   * Search vehicles
   */
  async search(query: string): Promise<void> {
    await this.searchInput.fill(query);
    await this.page.waitForTimeout(500);
    await this.waitForLoading();
  }

  /**
   * View vehicle details
   */
  async viewDetails(index: number): Promise<void> {
    await this.viewDetailsButtons.nth(index).click();
    await this.page.waitForTimeout(500);
  }

  /**
   * Check if vehicle details panel is visible
   */
  async isDetailPanelVisible(): Promise<boolean> {
    return this.isVisible(this.vehicleDetailPanel);
  }

  /**
   * Get vehicle details
   */
  async getVehicleDetails(): Promise<{
    name: string | null;
    status: string | null;
    location: string | null;
    category: string | null;
    licensePlate: string | null;
  }> {
    return {
      name: await this.vehicleName.textContent(),
      status: await this.vehicleStatus.textContent(),
      location: await this.vehicleLocation.textContent(),
      category: await this.vehicleCategory.textContent(),
      licensePlate: await this.vehicleLicensePlate.textContent()
    };
  }

  /**
   * Check if vehicle has current reservation
   */
  async hasCurrentReservation(): Promise<boolean> {
    return this.isVisible(this.currentReservation);
  }

  /**
   * Open status change modal
   */
  async openStatusModal(): Promise<void> {
    await this.changeStatusButton.click();
    await this.page.waitForTimeout(300);
  }

  /**
   * Change vehicle status
   */
  async changeStatus(newStatus: string, notes?: string): Promise<void> {
    await this.openStatusModal();
    await this.newStatusSelect.selectOption(newStatus);
    if (notes) {
      await this.statusNotes.fill(notes);
    }
    await this.confirmStatusButton.click();
    await this.page.waitForTimeout(500);
    await this.waitForLoading();
  }

  /**
   * Check if dashboard stats are visible
   */
  async areStatsVisible(): Promise<boolean> {
    return this.isVisible(this.dashboardSection);
  }

  /**
   * Check if fleet chart is visible
   */
  async isChartVisible(): Promise<boolean> {
    return this.isVisible(this.fleetChart);
  }

  /**
   * Get status from vehicle row
   */
  async getVehicleStatus(index: number): Promise<string | null> {
    const row = this.vehicleRows.nth(index);
    const status = row.locator('.status-badge, .badge');
    return status.textContent();
  }
}
