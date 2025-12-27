import { Page, Locator } from '@playwright/test';
import { BasePage } from './base.page';

/**
 * Page Object for Booking History (US-4)
 */
export class BookingHistoryPage extends BasePage {
  // Sections
  readonly upcomingSection: Locator;
  readonly pastSection: Locator;
  readonly pendingSection: Locator;

  // Reservation cards
  readonly reservationCards: Locator;

  // Guest lookup form
  readonly guestLookupForm: Locator;
  readonly reservationIdInput: Locator;
  readonly emailInput: Locator;
  readonly lookupButton: Locator;
  readonly lookupError: Locator;

  // Detail modal
  readonly detailModal: Locator;
  readonly closeDetailButton: Locator;

  // Cancel modal
  readonly cancelModal: Locator;
  readonly cancelReasonSelect: Locator;
  readonly confirmCancelButton: Locator;
  readonly closeCancelButton: Locator;

  // Loading and empty states
  readonly loadingIndicator: Locator;
  readonly emptyState: Locator;
  readonly errorState: Locator;

  constructor(page: Page) {
    super(page);

    // Sections
    this.upcomingSection = page.locator('.upcoming-reservations, [data-section="upcoming"]');
    this.pastSection = page.locator('.past-reservations, [data-section="past"]');
    this.pendingSection = page.locator('.pending-reservations, [data-section="pending"]');

    // Cards
    this.reservationCards = page.locator('.reservation-card, [data-testid="reservation-card"]');

    // Guest lookup
    this.guestLookupForm = page.locator('.guest-lookup-form');
    this.reservationIdInput = page.locator('input[formControlName="reservationId"], input[name="reservationId"]');
    this.emailInput = page.locator('input[formControlName="email"], input[name="email"]');
    this.lookupButton = page.locator('button:has-text("Suchen"), button:has-text("Reservierung suchen")');
    this.lookupError = page.locator('.lookup-error, .guest-lookup-error');

    // Detail modal
    this.detailModal = page.locator('.detail-modal, [data-testid="detail-modal"]');
    this.closeDetailButton = page.locator('.detail-modal button:has-text("Schließen")');

    // Cancel modal
    this.cancelModal = page.locator('.cancel-modal, [data-testid="cancel-modal"]');
    this.cancelReasonSelect = page.locator('select[formControlName="cancellationReason"]');
    this.confirmCancelButton = page.locator('button:has-text("Stornieren bestätigen")');
    this.closeCancelButton = page.locator('.cancel-modal button:has-text("Abbrechen")');

    // States
    this.loadingIndicator = page.locator('.loading, .spinner');
    this.emptyState = page.locator('.empty-state, text=/keine.*Buchungen/i');
    this.errorState = page.locator('.error-state, .alert-danger');
  }

  /**
   * Navigate to booking history page
   */
  async navigate(): Promise<void> {
    await this.goto('/booking-history');
    await this.page.waitForTimeout(1000);
  }

  /**
   * Wait for reservations to load
   */
  async waitForLoad(): Promise<void> {
    const loading = await this.isVisible(this.loadingIndicator);
    if (loading) {
      await this.loadingIndicator.waitFor({ state: 'hidden', timeout: 10000 });
    }
  }

  /**
   * Get count of reservations in a section
   */
  async getReservationCount(section: 'upcoming' | 'past' | 'pending'): Promise<number> {
    const sectionLocator = {
      upcoming: this.upcomingSection,
      past: this.pastSection,
      pending: this.pendingSection
    }[section];

    if (await this.isVisible(sectionLocator)) {
      return sectionLocator.locator('.reservation-card').count();
    }
    return 0;
  }

  /**
   * Get total reservation count
   */
  async getTotalReservationCount(): Promise<number> {
    return this.reservationCards.count();
  }

  /**
   * Lookup guest reservation
   */
  async lookupGuestReservation(reservationId: string, email: string): Promise<void> {
    await this.reservationIdInput.fill(reservationId);
    await this.emailInput.fill(email);
    await this.lookupButton.click();
    await this.page.waitForTimeout(1000);
  }

  /**
   * Check if guest lookup error is shown
   */
  async hasLookupError(): Promise<boolean> {
    return this.isVisible(this.lookupError);
  }

  /**
   * View reservation details
   */
  async viewReservationDetails(index: number): Promise<void> {
    const card = this.reservationCards.nth(index);
    await card.locator('button:has-text("Details"), button:has-text("Ansehen")').click();
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
    await this.closeDetailButton.click();
    await this.page.waitForTimeout(300);
  }

  /**
   * Open cancel modal for a reservation
   */
  async openCancelModal(index: number): Promise<void> {
    const card = this.reservationCards.nth(index);
    await card.locator('button:has-text("Stornieren")').click();
    await this.page.waitForTimeout(500);
  }

  /**
   * Check if cancel modal is visible
   */
  async isCancelModalVisible(): Promise<boolean> {
    return this.isVisible(this.cancelModal);
  }

  /**
   * Cancel a reservation
   */
  async cancelReservation(reason: string): Promise<void> {
    await this.cancelReasonSelect.selectOption(reason);
    await this.confirmCancelButton.click();
    await this.page.waitForTimeout(1000);
  }

  /**
   * Close cancel modal
   */
  async closeCancelModal(): Promise<void> {
    await this.closeCancelButton.click();
    await this.page.waitForTimeout(300);
  }

  /**
   * Check if a reservation can be cancelled
   */
  async canCancelReservation(index: number): Promise<boolean> {
    const card = this.reservationCards.nth(index);
    const cancelButton = card.locator('button:has-text("Stornieren")');
    return this.isVisible(cancelButton);
  }

  /**
   * Get reservation status from card
   */
  async getReservationStatus(index: number): Promise<string | null> {
    const card = this.reservationCards.nth(index);
    const statusBadge = card.locator('.status-badge, .badge');
    return statusBadge.textContent();
  }

  /**
   * Check if empty state is shown
   */
  async hasEmptyState(): Promise<boolean> {
    return this.isVisible(this.emptyState);
  }

  /**
   * Check if error state is shown
   */
  async hasErrorState(): Promise<boolean> {
    return this.isVisible(this.errorState);
  }

  /**
   * Print current reservation (from detail modal)
   */
  async printReservation(): Promise<void> {
    const printButton = this.page.locator('button:has-text("Drucken")');
    if (await this.isVisible(printButton)) {
      await printButton.click();
    }
  }
}
