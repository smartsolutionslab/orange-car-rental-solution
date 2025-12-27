/**
 * Shared test helper utilities
 */
import type { ISODateString } from '../types/common/iso-date-string.type';

/**
 * Get a future date as ISO string
 * @param daysFromNow Number of days from today
 * @returns ISO date string (YYYY-MM-DD format)
 */
export function getFutureDate(daysFromNow: number): ISODateString {
  const date = new Date();
  date.setDate(date.getDate() + daysFromNow);
  return date.toISOString().split('T')[0]! as ISODateString;
}

/**
 * Get a past date as ISO string
 * @param daysAgo Number of days before today
 * @returns ISO date string (YYYY-MM-DD format)
 */
export function getPastDate(daysAgo: number): ISODateString {
  const date = new Date();
  date.setDate(date.getDate() - daysAgo);
  return date.toISOString().split('T')[0]! as ISODateString;
}

/**
 * Get a future datetime as ISO string
 * @param daysFromNow Number of days from today
 * @returns Full ISO datetime string
 */
export function getFutureDateTime(daysFromNow: number): ISODateString {
  const date = new Date();
  date.setDate(date.getDate() + daysFromNow);
  return date.toISOString() as ISODateString;
}

/**
 * Get a past datetime as ISO string
 * @param daysAgo Number of days before today
 * @returns Full ISO datetime string
 */
export function getPastDateTime(daysAgo: number): ISODateString {
  const date = new Date();
  date.setDate(date.getDate() - daysAgo);
  return date.toISOString() as ISODateString;
}

/**
 * Generate a UUID v4 for testing
 * @param seed Optional seed number for deterministic UUIDs
 */
export function generateTestId(seed?: number): string {
  if (seed !== undefined) {
    // Deterministic UUID based on seed
    const hex = seed.toString(16).padStart(8, '0');
    return `${hex}-e89b-12d3-a456-426614174${seed.toString().padStart(3, '0')}`;
  }
  // Random UUID
  return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, (c) => {
    const r = (Math.random() * 16) | 0;
    const v = c === 'x' ? r : (r & 0x3) | 0x8;
    return v.toString(16);
  });
}
