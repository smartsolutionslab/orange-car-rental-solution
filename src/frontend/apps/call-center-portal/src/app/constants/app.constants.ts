/**
 * Application-wide constants for the call-center portal
 */

// Pagination defaults
export const PAGE_SIZES = {
  SMALL: 25,
  MEDIUM: 50,
  LARGE: 100,
  ALL: 1000,
} as const;

// UI timing constants (in milliseconds)
export const UI_TIMING = {
  SUCCESS_MESSAGE_DURATION: 5000,
  ERROR_MESSAGE_DURATION: 5000,
  DEBOUNCE_TIME: 300,
} as const;

// Default page size for different contexts
export const DEFAULT_PAGE_SIZE = {
  CUSTOMERS: PAGE_SIZES.MEDIUM,
  RESERVATIONS: PAGE_SIZES.SMALL,
  VEHICLES: PAGE_SIZES.ALL,
  CUSTOMER_RESERVATIONS: PAGE_SIZES.LARGE,
} as const;

// Business logic constants
export const UTILIZATION_THRESHOLDS = {
  HIGH: 80,
  MEDIUM: 50,
} as const;

// Vehicle form defaults
export const VEHICLE_DEFAULTS = {
  SEATS: 5,
  DAILY_RATE_NET: 50,
} as const;

// Vehicle form validation constraints
export const VEHICLE_CONSTRAINTS = {
  MIN_YEAR: 1900,
  MAX_YEAR_OFFSET: 1, // Current year + 1
  MIN_SEATS: 1,
  MAX_SEATS: 9,
} as const;

// German VAT rate (19%)
export const GERMAN_VAT_MULTIPLIER = 1.19;
