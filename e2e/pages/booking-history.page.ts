import { Page, Locator } from '@playwright/test';

/**
 * Page Object for Booking History (Public Portal)
 */
export class BookingHistoryPage {
  readonly page: Page;
  readonly upcomingSection: Locator;
  readonly pendingSection: Locator;
  readonly pastSection: Locator;
  readonly guestLookupForm: Locator;
  readonly reservationIdInput: Locator;
  readonly emailInput: Locator;
  readonly lookupButton: Locator;
  readonly errorMessage: Locator;
  readonly loadingSpinner: Locator;
  readonly cancelModal: Locator;
  readonly detailModal: Locator;

  constructor(page: Page) {
    this.page = page;
    this.upcomingSection = page.locator('.upcoming-reservations');
    this.pendingSection = page.locator('.pending-reservations');
    this.pastSection = page.locator('.past-reservations');
    this.guestLookupForm = page.locator('.guest-lookup-form');
    this.reservationIdInput = page.locator('input[name="reservationId"]');
    this.emailInput = page.locator('input[name="email"]');
    this.lookupButton = page.locator('button:has-text("Buchung suchen")');
    this.errorMessage = page.locator('.error-message');
    this.loadingSpinner = page.locator('.loading-spinner');
    this.cancelModal = page.locator('.cancel-modal');
    this.detailModal = page.locator('.detail-modal');
  }

  async goto(): Promise<void> {
    await this.page.goto('/my-bookings');
  }

  async isAuthenticatedView(): Promise<boolean> {
    return await this.upcomingSection.isVisible();
  }

  async isGuestView(): Promise<boolean> {
    return await this.guestLookupForm.isVisible();
  }

  async getUpcomingReservationsCount(): Promise<number> {
    const cards = await this.upcomingSection.locator('.reservation-card').count();
    return cards;
  }

  async getPendingReservationsCount(): Promise<number> {
    const cards = await this.pendingSection.locator('.reservation-card').count();
    return cards;
  }

  async getPastReservationsCount(): Promise<number> {
    const cards = await this.pastSection.locator('.reservation-card').count();
    return cards;
  }

  async lookupGuestReservation(reservationId: string, email: string): Promise<void> {
    await this.reservationIdInput.fill(reservationId);
    await this.emailInput.fill(email);
    await this.lookupButton.click();
  }

  async getErrorMessage(): Promise<string | null> {
    if (await this.errorMessage.isVisible()) {
      return await this.errorMessage.textContent();
    }
    return null;
  }

  async viewReservationDetails(index: number): Promise<void> {
    const detailButtons = this.page.locator('button:has-text("Details")');
    await detailButtons.nth(index).click();
    await this.detailModal.waitFor({ state: 'visible' });
  }

  async openCancelModal(index: number): Promise<void> {
    const cancelButtons = this.page.locator('button:has-text("Stornieren")');
    await cancelButtons.nth(index).click();
    await this.cancelModal.waitFor({ state: 'visible' });
  }

  async confirmCancellation(reason: string): Promise<void> {
    await this.cancelModal.locator('textarea[name="reason"]').fill(reason);
    await this.cancelModal.locator('button:has-text("Ja, stornieren")').click();
  }

  async closeCancelModal(): Promise<void> {
    await this.cancelModal.locator('button:has-text("Abbrechen")').click();
  }

  async closeDetailModal(): Promise<void> {
    await this.detailModal.locator('button.close, button:has-text("Schließen")').click();
  }

  async waitForLoading(): Promise<void> {
    await this.loadingSpinner.waitFor({ state: 'hidden', timeout: 10000 });
  }

  async getReservationStatus(index: number): Promise<string | null> {
    const statusBadges = this.page.locator('.status-badge');
    if (await statusBadges.count() > index) {
      return await statusBadges.nth(index).textContent();
    }
    return null;
  }

  async canCancelReservation(index: number): Promise<boolean> {
    const cancelButtons = this.page.locator('button:has-text("Stornieren")');
    if (await cancelButtons.count() > index) {
      return await cancelButtons.nth(index).isEnabled();
    }
    return false;
  }
}
