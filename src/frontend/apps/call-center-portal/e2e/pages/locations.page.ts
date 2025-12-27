import { Page, Locator } from '@playwright/test';
import { BasePage } from './base.page';

/**
 * Page Object for Locations (US-11)
 */
export class LocationsPage extends BasePage {
  // Locations list
  readonly locationsList: Locator;
  readonly locationCards: Locator;
  readonly locationRows: Locator;

  // Location details
  readonly locationName: Locator;
  readonly locationAddress: Locator;
  readonly locationCity: Locator;
  readonly locationPostalCode: Locator;
  readonly locationCode: Locator;
  readonly locationHours: Locator;
  readonly locationContact: Locator;

  // Vehicle count at location
  readonly vehicleCount: Locator;
  readonly availableVehicles: Locator;
  readonly rentedVehicles: Locator;

  // Map (if present)
  readonly mapContainer: Locator;
  readonly mapMarkers: Locator;

  // Search/Filter
  readonly searchInput: Locator;
  readonly cityFilter: Locator;

  // Detail panel
  readonly detailPanel: Locator;
  readonly viewVehiclesButton: Locator;
  readonly viewMapButton: Locator;

  // Actions
  readonly viewDetailsButtons: Locator;
  readonly editButton: Locator;

  // States
  readonly loadingIndicator: Locator;
  readonly emptyState: Locator;
  readonly errorState: Locator;

  constructor(page: Page) {
    super(page);

    // List
    this.locationsList = page.locator('.locations-list, .location-grid');
    this.locationCards = page.locator('.location-card, [data-testid="location-card"]');
    this.locationRows = page.locator('tbody tr, .location-row');

    // Details
    this.locationName = page.locator('.location-name, [data-field="name"]');
    this.locationAddress = page.locator('.location-address, [data-field="address"]');
    this.locationCity = page.locator('.location-city, [data-field="city"]');
    this.locationPostalCode = page.locator('.location-postal-code, [data-field="postalCode"]');
    this.locationCode = page.locator('.location-code, [data-field="code"]');
    this.locationHours = page.locator('.opening-hours, [data-field="hours"]');
    this.locationContact = page.locator('.location-contact, [data-field="contact"]');

    // Vehicle counts
    this.vehicleCount = page.locator('.vehicle-count, [data-stat="vehicles"]');
    this.availableVehicles = page.locator('[data-stat="available"]');
    this.rentedVehicles = page.locator('[data-stat="rented"]');

    // Map
    this.mapContainer = page.locator('.map-container, #map');
    this.mapMarkers = page.locator('.map-marker, .leaflet-marker-icon');

    // Search/Filter
    this.searchInput = page.locator('input[formControlName="search"], input[placeholder*="Suchen"]');
    this.cityFilter = page.locator('select[formControlName="city"], #cityFilter');

    // Detail panel
    this.detailPanel = page.locator('.detail-panel, .location-details');
    this.viewVehiclesButton = page.locator('button:has-text("Fahrzeuge anzeigen")');
    this.viewMapButton = page.locator('button:has-text("Auf Karte anzeigen")');

    // Actions
    this.viewDetailsButtons = page.locator('button:has-text("Details"), button:has-text("Ansehen")');
    this.editButton = page.locator('button:has-text("Bearbeiten")');

    // States
    this.loadingIndicator = page.locator('.loading, .spinner');
    this.emptyState = page.locator('.empty-state');
    this.errorState = page.locator('.error-state, .alert-danger');
  }

  /**
   * Navigate to locations page
   */
  async navigate(): Promise<void> {
    await this.goto('/locations');
    await this.page.waitForTimeout(1000);
    await this.waitForLoading();
  }

  /**
   * Get number of locations displayed
   */
  async getLocationCount(): Promise<number> {
    const cards = await this.locationCards.count();
    const rows = await this.locationRows.count();
    return Math.max(cards, rows);
  }

  /**
   * Search locations
   */
  async search(query: string): Promise<void> {
    await this.searchInput.fill(query);
    await this.page.waitForTimeout(500);
    await this.waitForLoading();
  }

  /**
   * Filter by city
   */
  async filterByCity(city: string): Promise<void> {
    await this.cityFilter.selectOption(city);
    await this.page.waitForTimeout(500);
    await this.waitForLoading();
  }

  /**
   * View location details by index
   */
  async viewDetails(index: number): Promise<void> {
    await this.viewDetailsButtons.nth(index).click();
    await this.page.waitForTimeout(500);
  }

  /**
   * Navigate to location by code
   */
  async navigateToLocation(code: string): Promise<void> {
    await this.goto(`/locations/${code}`);
    await this.page.waitForTimeout(500);
    await this.waitForLoading();
  }

  /**
   * Check if detail panel is visible
   */
  async isDetailPanelVisible(): Promise<boolean> {
    return this.isVisible(this.detailPanel);
  }

  /**
   * Get location details
   */
  async getLocationDetails(): Promise<{
    name: string | null;
    address: string | null;
    city: string | null;
    postalCode: string | null;
    code: string | null;
  }> {
    return {
      name: await this.locationName.textContent(),
      address: await this.locationAddress.textContent(),
      city: await this.locationCity.textContent(),
      postalCode: await this.locationPostalCode.textContent(),
      code: await this.locationCode.textContent()
    };
  }

  /**
   * Get vehicle count at location
   */
  async getVehicleCountAtLocation(): Promise<number> {
    const countText = await this.vehicleCount.textContent();
    const match = countText?.match(/\d+/);
    return match ? parseInt(match[0], 10) : 0;
  }

  /**
   * Click view vehicles button
   */
  async viewVehiclesAtLocation(): Promise<void> {
    await this.viewVehiclesButton.click();
    await this.page.waitForTimeout(500);
  }

  /**
   * Check if map is visible
   */
  async isMapVisible(): Promise<boolean> {
    return this.isVisible(this.mapContainer);
  }

  /**
   * Get number of map markers
   */
  async getMapMarkerCount(): Promise<number> {
    if (await this.isMapVisible()) {
      return this.mapMarkers.count();
    }
    return 0;
  }

  /**
   * Click on map marker
   */
  async clickMapMarker(index: number): Promise<void> {
    await this.mapMarkers.nth(index).click();
    await this.page.waitForTimeout(500);
  }

  /**
   * View location on map
   */
  async viewOnMap(): Promise<void> {
    await this.viewMapButton.click();
    await this.page.waitForTimeout(500);
  }

  /**
   * Get location name from card/row
   */
  async getLocationName(index: number): Promise<string | null> {
    const card = this.locationCards.nth(index);
    return card.locator('.location-name, h3, h4').textContent();
  }
}
