/**
 * UI Components Library
 * Reusable UI components for Orange Car Rental applications
 */

// Vehicle Search Component
export * from './lib/vehicle-search/vehicle-search.component';

// Selection Components
export { SelectCategoryComponent } from './lib/select-category/select-category.component';
export { SelectLocationComponent } from './lib/select-location/select-location.component';
export { SelectFuelTypeComponent } from './lib/select-fuel-type/select-fuel-type.component';
export { SelectTransmissionComponent } from './lib/select-transmission/select-transmission.component';
export { SelectVehicleStatusComponent } from './lib/select-status/select-vehicle-status.component';
export { SelectReservationStatusComponent } from './lib/select-status/select-reservation-status.component';

// Date Range Picker
export {
  DateRangePickerComponent,
  type DateRange,
} from './lib/date-range-picker/date-range-picker.component';

// Status Badge Component
export { StatusBadgeComponent } from './lib/status-badge/status-badge.component';

// Modal Component
export { ModalComponent } from './lib/modal/modal.component';

// State Display Components
export { LoadingStateComponent } from './lib/state-display/loading-state.component';
export { EmptyStateComponent } from './lib/state-display/empty-state.component';
export { ErrorStateComponent } from './lib/state-display/error-state.component';

// Stat Card Component
export { StatCardComponent, type StatCardVariant } from './lib/stat-card/stat-card.component';

// Utility Functions
export {
  getVehicleStatusClass,
  getVehicleStatusLabel,
  getReservationStatusClass,
  getReservationStatusLabel,
  formatDateDE,
  formatDateLongDE,
  formatPriceDE,
} from './lib/utils/status-display.utils';

// Pagination Component
export { PaginationComponent } from './lib/pagination/pagination.component';

// Select Seats Component
export { SelectSeatsComponent } from './lib/select-seats/select-seats.component';

// Select Page Size Component
export { SelectPageSizeComponent } from './lib/select-page-size/select-page-size.component';

// Detail Row Component
export { DetailRowComponent } from './lib/detail-row/detail-row.component';

// Reservation Card Component
export {
  ReservationCardComponent,
  type ReservationCardVariant,
  type ReservationCardData,
} from './lib/reservation-card/reservation-card.component';

// Form Field Component
export { FormFieldComponent } from './lib/form-field/form-field.component';
