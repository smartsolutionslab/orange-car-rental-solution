/**
 * ISO 8601 date string - branded type for type safety
 * Format: YYYY-MM-DD or YYYY-MM-DDTHH:mm:ss.sssZ
 * Matches backend DateOnly and DateTime serialization
 */
export type ISODateString = string & { readonly __brand: 'ISODateString' };

// Matches YYYY-MM-DD or YYYY-MM-DDTHH:mm:ss with optional milliseconds and Z
const ISO_DATE_REGEX = /^\d{4}-\d{2}-\d{2}(T\d{2}:\d{2}:\d{2}(\.\d{3})?Z?)?$/;

export function isISODateString(value: string): value is ISODateString {
  if (!ISO_DATE_REGEX.test(value)) return false;
  const date = new Date(value);
  return !isNaN(date.getTime());
}

export function createISODateString(value: string | Date): ISODateString {
  if (value instanceof Date) {
    return value.toISOString() as ISODateString;
  }
  if (!isISODateString(value)) {
    throw new Error(`Invalid ISODateString format: ${value}`);
  }
  return value;
}

/**
 * Safely convert a string or Date to ISODateString, returning undefined if invalid
 */
export function toISODateString(value: string | Date | null | undefined): ISODateString | undefined {
  if (!value) return undefined;
  if (value instanceof Date) {
    return isNaN(value.getTime()) ? undefined : (value.toISOString() as ISODateString);
  }
  return isISODateString(value) ? value : undefined;
}

/**
 * Convert ISODateString to Date object
 */
export function parseISODateString(value: ISODateString): Date {
  return new Date(value);
}

/**
 * Get date-only portion (YYYY-MM-DD) from ISODateString
 */
export function toDateOnly(value: ISODateString): ISODateString {
  return value.split('T')[0] as ISODateString;
}
