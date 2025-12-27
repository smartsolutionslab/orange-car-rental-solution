/**
 * Date Utilities
 *
 * Centralized date manipulation functions for the Orange Car Rental application.
 * These utilities handle common date operations like formatting, validation,
 * and calculations used throughout the frontend.
 */

/**
 * Get today's date as an ISO date string (YYYY-MM-DD)
 * Useful for form input min/max attributes
 */
export function getTodayDateString(): string {
  return new Date().toISOString().split('T')[0];
}

/**
 * Get tomorrow's date as an ISO date string (YYYY-MM-DD)
 */
export function getTomorrowDateString(): string {
  const tomorrow = new Date();
  tomorrow.setDate(tomorrow.getDate() + 1);
  return tomorrow.toISOString().split('T')[0];
}

/**
 * Add days to a date and return as ISO date string
 * @param date - The starting date (string or Date)
 * @param days - Number of days to add (can be negative)
 * @returns ISO date string (YYYY-MM-DD)
 */
export function addDays(date: string | Date, days: number): string {
  const d = typeof date === 'string' ? new Date(date) : new Date(date);
  d.setDate(d.getDate() + days);
  return d.toISOString().split('T')[0];
}

/**
 * Get the minimum return date (day after pickup)
 * @param pickupDate - The pickup date string
 * @returns ISO date string for minimum return date
 */
export function getMinReturnDate(pickupDate: string): string {
  return addDays(pickupDate, 1);
}

/**
 * Check if a date string is in the past (before today)
 * @param dateString - ISO date string to check
 * @returns true if the date is before today
 */
export function isDateInPast(dateString: string): boolean {
  const date = new Date(dateString);
  const today = new Date();
  today.setHours(0, 0, 0, 0);
  return date < today;
}

/**
 * Check if a date string is today or in the future
 * @param dateString - ISO date string to check
 * @returns true if the date is today or later
 */
export function isDateTodayOrFuture(dateString: string): boolean {
  return !isDateInPast(dateString);
}

/**
 * Check if a license is expired based on expiry date
 * @param expiryDate - License expiry date string
 * @returns true if the license has expired
 */
export function isLicenseExpired(expiryDate: string): boolean {
  return isDateInPast(expiryDate);
}

/**
 * Check if a license is expiring within a given number of days
 * @param expiryDate - License expiry date string
 * @param days - Number of days to check
 * @returns true if the license expires within the specified days (and is not already expired)
 */
export function isLicenseExpiringSoon(expiryDate: string, days: number): boolean {
  const expiry = new Date(expiryDate);
  const today = new Date();
  today.setHours(0, 0, 0, 0);

  const warningDate = new Date(today);
  warningDate.setDate(warningDate.getDate() + days);

  return expiry <= warningDate && expiry >= today;
}

/**
 * Calculate age from date of birth
 * @param dateOfBirth - Date of birth string (YYYY-MM-DD)
 * @returns Age in years
 */
export function calculateAge(dateOfBirth: string): number {
  const today = new Date();
  const birthDate = new Date(dateOfBirth);

  let age = today.getFullYear() - birthDate.getFullYear();
  const monthDiff = today.getMonth() - birthDate.getMonth();

  if (monthDiff < 0 || (monthDiff === 0 && today.getDate() < birthDate.getDate())) {
    age--;
  }

  return age;
}

/**
 * Get days until a future date
 * @param dateString - Target date string
 * @returns Number of days until the date (negative if in past)
 */
export function getDaysUntil(dateString: string): number {
  const target = new Date(dateString);
  const today = new Date();
  today.setHours(0, 0, 0, 0);

  const diffTime = target.getTime() - today.getTime();
  return Math.ceil(diffTime / (1000 * 60 * 60 * 24));
}

/**
 * Get hours until a specific date/time
 * @param dateString - Target date string
 * @returns Number of hours until the date (negative if in past)
 */
export function getHoursUntil(dateString: string): number {
  const target = new Date(dateString);
  const now = new Date();

  const diffTime = target.getTime() - now.getTime();
  return diffTime / (1000 * 60 * 60);
}

/**
 * Check if cancellation is still allowed (48 hours before pickup)
 * @param pickupDate - The pickup date string
 * @returns true if more than 48 hours until pickup
 */
export function canCancelReservation(pickupDate: string): boolean {
  return getHoursUntil(pickupDate) >= 48;
}

/**
 * Parse a date string to Date object safely
 * @param dateString - Date string to parse
 * @returns Date object or null if invalid
 */
export function parseDate(dateString: string | null | undefined): Date | null {
  if (!dateString) return null;

  const date = new Date(dateString);
  return isNaN(date.getTime()) ? null : date;
}

/**
 * Check if first date is before second date
 * @param date1 - First date string
 * @param date2 - Second date string
 * @returns true if date1 is before date2
 */
export function isBefore(date1: string, date2: string): boolean {
  return new Date(date1) < new Date(date2);
}

/**
 * Check if first date is after second date
 * @param date1 - First date string
 * @param date2 - Second date string
 * @returns true if date1 is after date2
 */
export function isAfter(date1: string, date2: string): boolean {
  return new Date(date1) > new Date(date2);
}

/**
 * Check if two dates are the same day
 * @param date1 - First date string
 * @param date2 - Second date string
 * @returns true if both dates are the same day
 */
export function isSameDay(date1: string, date2: string): boolean {
  const d1 = new Date(date1);
  const d2 = new Date(date2);

  return (
    d1.getFullYear() === d2.getFullYear() &&
    d1.getMonth() === d2.getMonth() &&
    d1.getDate() === d2.getDate()
  );
}

/**
 * Get max date of birth for a minimum age requirement
 * @param minAge - Minimum age in years
 * @returns ISO date string for the maximum allowed birth date
 */
export function getMaxDateOfBirthForAge(minAge: number): string {
  const date = new Date();
  date.setFullYear(date.getFullYear() - minAge);
  return date.toISOString().split('T')[0];
}

/**
 * Format a date range for display
 * @param startDate - Start date string
 * @param endDate - End date string
 * @param locale - Locale for formatting (default: 'de-DE')
 * @returns Formatted date range string
 */
export function formatDateRange(startDate: string, endDate: string, locale = 'de-DE'): string {
  const start = new Date(startDate);
  const end = new Date(endDate);

  const options: Intl.DateTimeFormatOptions = {
    day: '2-digit',
    month: '2-digit',
    year: 'numeric',
  };

  return `${start.toLocaleDateString(locale, options)} - ${end.toLocaleDateString(locale, options)}`;
}
