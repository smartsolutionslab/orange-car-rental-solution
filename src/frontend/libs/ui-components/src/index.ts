/**
 * UI Components Library
 * Reusable UI components for Orange Car Rental applications
 */

// Vehicle Search Component
export * from "./lib/vehicle-search/vehicle-search.component";

// Selection Components
export { SelectCategoryComponent } from "./lib/select-category/select-category.component";
export { SelectLocationComponent } from "./lib/select-location/select-location.component";
export { SelectFuelTypeComponent } from "./lib/select-fuel-type/select-fuel-type.component";
export { SelectTransmissionComponent } from "./lib/select-transmission/select-transmission.component";
export { SelectVehicleStatusComponent } from "./lib/select-status/select-vehicle-status.component";
export { SelectReservationStatusComponent } from "./lib/select-status/select-reservation-status.component";

// Date Range Picker
export {
  DateRangePickerComponent,
  type DateRange,
} from "./lib/date-range-picker/date-range-picker.component";

// Status Badge Component
export { StatusBadgeComponent } from "./lib/status-badge/status-badge.component";

// Modal Component
export { ModalComponent } from "./lib/modal/modal.component";

// State Display Components
export { LoadingStateComponent } from "./lib/state-display/loading-state.component";
export { EmptyStateComponent } from "./lib/state-display/empty-state.component";
export { ErrorStateComponent } from "./lib/state-display/error-state.component";
export { SuccessAlertComponent } from "./lib/state-display/success-alert.component";
export { ErrorAlertComponent } from "./lib/state-display/error-alert.component";

// Stat Card Component
export {
  StatCardComponent,
  type StatCardVariant,
} from "./lib/stat-card/stat-card.component";

// Utility Functions
export {
  getVehicleStatusClass,
  getVehicleStatusLabel,
  getReservationStatusClass,
  getReservationStatusLabel,
  formatDateDE,
  formatDateLongDE,
  formatPriceDE,
  calculateRentalDays,
} from "./lib/utils/status-display.utils";

// Pagination Component
export { PaginationComponent } from "./lib/pagination/pagination.component";

// Select Seats Component
export { SelectSeatsComponent } from "./lib/select-seats/select-seats.component";

// Select Page Size Component
export { SelectPageSizeComponent } from "./lib/select-page-size/select-page-size.component";

// Detail Row Component
export { DetailRowComponent } from "./lib/detail-row/detail-row.component";

// Reservation Card Component
export {
  ReservationCardComponent,
  type ReservationCardVariant,
  type ReservationCardData,
} from "./lib/reservation-card/reservation-card.component";

// Form Field Component
export { FormFieldComponent } from "./lib/form-field/form-field.component";

// Icon Component
export {
  IconComponent,
  ICONS,
  ICON_NAMES,
  isValidIconName,
  type IconName,
} from "./lib/icon";

// Date Utilities
export {
  getTodayDateString,
  getTomorrowDateString,
  addDays,
  getMinReturnDate,
  isDateInPast,
  isDateTodayOrFuture,
  isLicenseExpired,
  isLicenseExpiringSoon,
  calculateAge,
  getDaysUntil,
  getHoursUntil,
  canCancelReservation,
  parseDate,
  isBefore,
  isAfter,
  isSameDay,
  getMaxDateOfBirthForAge,
  formatDateRange,
} from "./lib/utils/date.utils";

// Wizard Component
export {
  WizardComponent,
  WizardStepComponent,
  type WizardStepConfig,
  type WizardStepStatus,
  type WizardStepState,
  type WizardCompleteEvent,
  type WizardStepChangeEvent,
} from "./lib/wizard";

// Tabs Component
export { TabsComponent, TabComponent, type TabChangeEvent } from "./lib/tabs";

// Accordion Component
export {
  AccordionComponent,
  AccordionItemComponent,
  type AccordionChangeEvent,
} from "./lib/accordion";

// Data Table Component
export {
  DataTableComponent,
  DataTableColumnDirective,
  type DataTableColumn,
  type DataTableSort,
  type DataTableSortEvent,
  type DataTableSelection,
  type DataTableSelectionEvent,
  type DataTableRowClickEvent,
  type DataTablePagination,
  type DataTableCellContext,
  type DataTableHeaderContext,
  type SortDirection,
} from "./lib/data-table";

// Form Components
export { InputComponent, type InputSize, type InputType } from "./lib/input";
export {
  TextareaComponent,
  type TextareaSize,
  type TextareaResize,
} from "./lib/textarea";
export { CheckboxComponent, type CheckboxSize } from "./lib/checkbox";
export {
  RadioGroupComponent,
  type RadioGroupSize,
  type RadioGroupOrientation,
  type RadioOption,
} from "./lib/radio-group";
export { SwitchComponent, type SwitchSize } from "./lib/switch";
export { FileUploadComponent, type UploadedFile } from "./lib/file-upload";
export {
  FormSectionComponent,
  type FormSectionVariant,
} from "./lib/form-section";
export {
  ValidationMessagesComponent,
  DEFAULT_VALIDATION_MESSAGES,
  type ValidationMessageConfig,
} from "./lib/validation-messages";
export {
  FormGroupDirective,
  FormRowDirective,
  FullWidthDirective,
  type FormGroupLayout,
  type FormGroupSpacing,
} from "./lib/form-group";

// Collapsible Panel Component
export {
  CollapsiblePanelComponent,
  type CollapsiblePanelChangeEvent,
} from "./lib/collapsible-panel";

// Generic Select Component
export {
  SelectComponent,
  type SelectSize,
  type SelectOption,
} from "./lib/select";

// Vehicle Card Component
export {
  VehicleCardComponent,
  type VehicleCardVariant,
  type PriceDifference,
} from "./lib/vehicle-card";
