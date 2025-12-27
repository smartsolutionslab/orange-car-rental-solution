/**
 * Page Objects for Public Portal E2E Tests
 *
 * Usage:
 * ```typescript
 * import { VehicleSearchPage, BookingPage, LoginPage } from './pages';
 *
 * test('search for vehicles', async ({ page }) => {
 *   const searchPage = new VehicleSearchPage(page);
 *   await searchPage.navigate();
 *   await searchPage.search({ locationCode: 'BER-HBF' });
 *   expect(await searchPage.getVehicleCount()).toBeGreaterThan(0);
 * });
 * ```
 */

export { BasePage } from './base.page';
export { VehicleSearchPage } from './vehicle-search.page';
export { BookingPage } from './booking.page';
export type { CustomerInfo, AddressInfo, DriversLicenseInfo } from './booking.page';
export { LoginPage, RegisterPage } from './login.page';
export { BookingHistoryPage } from './booking-history.page';
