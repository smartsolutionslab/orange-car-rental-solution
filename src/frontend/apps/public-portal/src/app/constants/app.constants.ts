/**
 * Application-wide constants for the public portal
 */

// Pagination defaults
export const PAGE_SIZES = {
  SMALL: 10,
  MEDIUM: 20,
  LARGE: 100
} as const;

// UI timing constants (in milliseconds)
export const UI_TIMING = {
  SUCCESS_MESSAGE_DURATION: 3000,
  ERROR_MESSAGE_DURATION: 5000,
  DEBOUNCE_TIME: 300
} as const;

// Default page size for different contexts
export const DEFAULT_PAGE_SIZE = {
  VEHICLE_SEARCH: PAGE_SIZES.SMALL,
  SIMILAR_VEHICLES: PAGE_SIZES.MEDIUM,
  BOOKING_HISTORY: PAGE_SIZES.LARGE,
  MAX_SIMILAR_VEHICLES: 4
} as const;
