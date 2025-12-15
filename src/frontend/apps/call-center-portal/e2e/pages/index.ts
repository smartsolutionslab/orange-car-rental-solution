/**
 * Page Objects for Call Center Portal E2E Tests
 *
 * Usage:
 * ```typescript
 * import { ReservationsPage, CustomersPage, VehicleDashboardPage } from './pages';
 *
 * test('view reservations', async ({ page }) => {
 *   const reservationsPage = new ReservationsPage(page);
 *   await reservationsPage.navigate();
 *   await reservationsPage.filterByStatus('Pending');
 *   expect(await reservationsPage.getReservationCount()).toBeGreaterThan(0);
 * });
 * ```
 */

export { BasePage } from './base.page';
export { ReservationsPage } from './reservations.page';
export { CustomersPage } from './customers.page';
export { VehicleDashboardPage } from './vehicle-dashboard.page';
export { LocationsPage } from './locations.page';
