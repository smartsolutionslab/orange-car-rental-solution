/**
 * Shared business rule constants used across all applications
 */

/**
 * German VAT rate (19%)
 */
export const GERMAN_VAT_RATE = 0.19;

/**
 * German VAT multiplier (1.19) for calculating gross from net
 */
export const GERMAN_VAT_MULTIPLIER = 1.19;

/**
 * Cancellation policy constants
 */
export const CANCELLATION_POLICY = {
  /** Hours before pickup for free cancellation */
  FREE_CANCELLATION_HOURS: 48,
} as const;

/**
 * Customer eligibility requirements
 */
export const CUSTOMER_REQUIREMENTS = {
  /** Minimum age to rent a vehicle */
  MINIMUM_RENTAL_AGE: 18,
  /** Minimum age for luxury vehicles */
  MINIMUM_LUXURY_AGE: 25,
} as const;

/**
 * Vehicle constraints for validation
 */
export const VEHICLE_CONSTRAINTS = {
  /** Earliest valid manufacturing year */
  MIN_YEAR: 1900,
  /** Offset from current year for max manufacturing year */
  MAX_YEAR_OFFSET: 1,
  /** Minimum seating capacity */
  MIN_SEATS: 1,
  /** Maximum seating capacity */
  MAX_SEATS: 9,
  /** Default seating capacity */
  DEFAULT_SEATS: 5,
  /** Default daily rate (net) in EUR */
  DEFAULT_DAILY_RATE_NET: 50,
} as const;

/**
 * Utilization thresholds for fleet management
 */
export const UTILIZATION_THRESHOLDS = {
  /** High utilization percentage */
  HIGH: 80,
  /** Medium utilization percentage */
  MEDIUM: 50,
} as const;

/**
 * UI timing constants (in milliseconds)
 */
export const UI_TIMING = {
  /** Duration to show success messages */
  SUCCESS_MESSAGE_DURATION: 3000,
  /** Duration to show error messages */
  ERROR_MESSAGE_DURATION: 5000,
  /** Debounce time for search inputs */
  DEBOUNCE_TIME: 300,
  /** Delay before redirect after success */
  REDIRECT_DELAY: 2000,
} as const;

/**
 * Common page sizes for pagination
 */
export const PAGE_SIZES = {
  EXTRA_SMALL: 10,
  SMALL: 25,
  MEDIUM: 50,
  LARGE: 100,
  ALL: 1000,
} as const;
