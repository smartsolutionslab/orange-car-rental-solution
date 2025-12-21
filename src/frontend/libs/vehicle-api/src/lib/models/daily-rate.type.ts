/**
 * Daily Rate - branded type for type safety
 * Matches backend Money value object (daily rental rate)
 * Values are in EUR with 2 decimal precision
 */
export type DailyRate = number & { readonly __brand: 'DailyRate' };

export function isDailyRate(value: number): value is DailyRate {
  return Number.isFinite(value) && value >= 0;
}

export function createDailyRate(value: number): DailyRate {
  if (!isDailyRate(value)) {
    throw new Error(`Invalid DailyRate: ${value}`);
  }
  // Round to 2 decimal places
  return Math.round(value * 100) / 100 as DailyRate;
}

/**
 * Safely convert a number to DailyRate, returning undefined if invalid
 */
export function toDailyRate(value: number | null | undefined): DailyRate | undefined {
  if (value === null || value === undefined) return undefined;
  return isDailyRate(value) ? (Math.round(value * 100) / 100 as DailyRate) : undefined;
}

/**
 * Format daily rate for display (e.g., "45,00 €/Tag")
 */
export function formatDailyRate(rate: DailyRate, locale: string = 'de-DE'): string {
  return new Intl.NumberFormat(locale, {
    style: 'currency',
    currency: 'EUR',
  }).format(rate);
}
