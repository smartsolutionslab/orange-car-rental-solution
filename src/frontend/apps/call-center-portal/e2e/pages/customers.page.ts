import { Page, Locator } from '@playwright/test';
import { BasePage } from './base.page';

/**
 * Page Object for Customers (US-9, US-12)
 */
export class CustomersPage extends BasePage {
  // Search form
  readonly searchForm: Locator;
  readonly emailSearch: Locator;
  readonly lastNameSearch: Locator;
  readonly phoneSearch: Locator;
  readonly searchButton: Locator;
  readonly clearSearchButton: Locator;

  // Results table
  readonly customersTable: Locator;
  readonly customerRows: Locator;
  readonly noResultsMessage: Locator;

  // Customer cards (alternative view)
  readonly customerCards: Locator;

  // Customer profile
  readonly profileSection: Locator;
  readonly customerName: Locator;
  readonly customerEmail: Locator;
  readonly customerPhone: Locator;
  readonly customerAddress: Locator;
  readonly driversLicense: Locator;

  // Reservation history in profile
  readonly reservationHistory: Locator;
  readonly reservationCards: Locator;

  // Action buttons
  readonly viewProfileButtons: Locator;
  readonly editButton: Locator;
  readonly backButton: Locator;

  // Edit form
  readonly editForm: Locator;
  readonly saveButton: Locator;
  readonly cancelEditButton: Locator;

  // States
  readonly loadingIndicator: Locator;
  readonly errorState: Locator;

  constructor(page: Page) {
    super(page);

    // Search
    this.searchForm = page.locator('.search-form, form');
    this.emailSearch = page.locator('input[formControlName="email"], input[placeholder*="E-Mail"]');
    this.lastNameSearch = page.locator('input[formControlName="lastName"], input[placeholder*="Nachname"]');
    this.phoneSearch = page.locator('input[formControlName="phoneNumber"], input[placeholder*="Telefon"]');
    this.searchButton = page.locator('button:has-text("Suchen")');
    this.clearSearchButton = page.locator('button:has-text("Zurücksetzen"), button:has-text("Löschen")');

    // Results
    this.customersTable = page.locator('table, .customers-table');
    this.customerRows = page.locator('tbody tr, .customer-row');
    this.noResultsMessage = page.locator('text=/keine.*Kunden.*gefunden|no.*customers.*found/i');
    this.customerCards = page.locator('.customer-card, [data-testid="customer-card"]');

    // Profile
    this.profileSection = page.locator('.customer-profile, .profile-section');
    this.customerName = page.locator('.customer-name, [data-field="name"]');
    this.customerEmail = page.locator('.customer-email, [data-field="email"]');
    this.customerPhone = page.locator('.customer-phone, [data-field="phone"]');
    this.customerAddress = page.locator('.customer-address, [data-field="address"]');
    this.driversLicense = page.locator('.drivers-license, [data-section="license"]');

    // Reservation history
    this.reservationHistory = page.locator('.reservation-history, [data-section="reservations"]');
    this.reservationCards = page.locator('.reservation-card, [data-testid="reservation-card"]');

    // Actions
    this.viewProfileButtons = page.locator('button:has-text("Profil"), button:has-text("Details"), a:has-text("Ansehen")');
    this.editButton = page.locator('button:has-text("Bearbeiten")');
    this.backButton = page.locator('button:has-text("Zurück")');

    // Edit form
    this.editForm = page.locator('.edit-form, form.customer-form');
    this.saveButton = page.locator('button:has-text("Speichern")');
    this.cancelEditButton = page.locator('button:has-text("Abbrechen")');

    // States
    this.loadingIndicator = page.locator('.loading, .spinner');
    this.errorState = page.locator('.error-state, .alert-danger');
  }

  /**
   * Navigate to customers page
   */
  async navigate(): Promise<void> {
    await this.goto('/customers');
    await this.page.waitForTimeout(1000);
    await this.waitForLoading();
  }

  /**
   * Search by email
   */
  async searchByEmail(email: string): Promise<void> {
    await this.emailSearch.fill(email);
    await this.searchButton.click();
    await this.page.waitForTimeout(500);
    await this.waitForLoading();
  }

  /**
   * Search by last name
   */
  async searchByLastName(lastName: string): Promise<void> {
    await this.lastNameSearch.fill(lastName);
    await this.searchButton.click();
    await this.page.waitForTimeout(500);
    await this.waitForLoading();
  }

  /**
   * Search by phone number
   */
  async searchByPhone(phone: string): Promise<void> {
    await this.phoneSearch.fill(phone);
    await this.searchButton.click();
    await this.page.waitForTimeout(500);
    await this.waitForLoading();
  }

  /**
   * Combined search
   */
  async search(criteria: { email?: string; lastName?: string; phone?: string }): Promise<void> {
    if (criteria.email) await this.emailSearch.fill(criteria.email);
    if (criteria.lastName) await this.lastNameSearch.fill(criteria.lastName);
    if (criteria.phone) await this.phoneSearch.fill(criteria.phone);
    await this.searchButton.click();
    await this.page.waitForTimeout(500);
    await this.waitForLoading();
  }

  /**
   * Clear search
   */
  async clearSearch(): Promise<void> {
    if (await this.isVisible(this.clearSearchButton)) {
      await this.clearSearchButton.click();
      await this.page.waitForTimeout(500);
    }
  }

  /**
   * Get number of search results
   */
  async getResultCount(): Promise<number> {
    const tableRows = await this.customerRows.count();
    const cards = await this.customerCards.count();
    return Math.max(tableRows, cards);
  }

  /**
   * Check if no results message is shown
   */
  async hasNoResults(): Promise<boolean> {
    return this.isVisible(this.noResultsMessage);
  }

  /**
   * View customer profile by index
   */
  async viewProfile(index: number): Promise<void> {
    await this.viewProfileButtons.nth(index).click();
    await this.page.waitForTimeout(500);
    await this.waitForLoading();
  }

  /**
   * Navigate to customer profile by ID
   */
  async navigateToProfile(customerId: string): Promise<void> {
    await this.goto(`/customers/${customerId}`);
    await this.page.waitForTimeout(500);
    await this.waitForLoading();
  }

  /**
   * Check if profile is visible
   */
  async isProfileVisible(): Promise<boolean> {
    return this.isVisible(this.profileSection);
  }

  /**
   * Get customer profile data
   */
  async getProfileData(): Promise<{
    name: string | null;
    email: string | null;
    phone: string | null;
  }> {
    return {
      name: await this.customerName.textContent(),
      email: await this.customerEmail.textContent(),
      phone: await this.customerPhone.textContent()
    };
  }

  /**
   * Get reservation count in profile
   */
  async getReservationCount(): Promise<number> {
    return this.reservationCards.count();
  }

  /**
   * Check if drivers license section is visible
   */
  async hasDriversLicense(): Promise<boolean> {
    return this.isVisible(this.driversLicense);
  }

  /**
   * Start editing customer
   */
  async startEditing(): Promise<void> {
    await this.editButton.click();
    await this.page.waitForTimeout(300);
  }

  /**
   * Save customer changes
   */
  async saveChanges(): Promise<void> {
    await this.saveButton.click();
    await this.page.waitForTimeout(500);
    await this.waitForLoading();
  }

  /**
   * Cancel editing
   */
  async cancelEditing(): Promise<void> {
    await this.cancelEditButton.click();
    await this.page.waitForTimeout(300);
  }

  /**
   * Update customer field
   */
  async updateField(fieldName: string, value: string): Promise<void> {
    await this.fillInput(fieldName, value);
  }

  /**
   * Go back to customer list
   */
  async goBack(): Promise<void> {
    await this.backButton.click();
    await this.page.waitForTimeout(300);
  }
}
