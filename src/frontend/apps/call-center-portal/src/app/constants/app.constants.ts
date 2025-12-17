/**
 * Application-wide constants for the call-center portal
 */

// Pagination defaults
export const PAGE_SIZES = {
  SMALL: 25,
  MEDIUM: 50,
  LARGE: 100,
  ALL: 1000
} as const;

// UI timing constants (in milliseconds)
export const UI_TIMING = {
  SUCCESS_MESSAGE_DURATION: 5000,
  SUCCESS_MESSAGE_SHORT: 3000,
  ERROR_MESSAGE_DURATION: 5000,
  DEBOUNCE_TIME: 300
} as const;

// Default page size for different contexts
export const DEFAULT_PAGE_SIZE = {
  CUSTOMERS: PAGE_SIZES.MEDIUM,
  RESERVATIONS: PAGE_SIZES.SMALL,
  VEHICLES: PAGE_SIZES.ALL,
  CUSTOMER_RESERVATIONS: PAGE_SIZES.LARGE
} as const;

// Business logic constants
export const UTILIZATION_THRESHOLDS = {
  HIGH: 80,
  MEDIUM: 50
} as const;
