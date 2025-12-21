/**
 * Postal Code - branded type for type safety
 * Matches backend PostalCode value object (German 5-digit format)
 */
export type PostalCode = string & { readonly __brand: 'PostalCode' };

// German postal codes: 5 digits
const POSTAL_CODE_REGEX = /^\d{5}$/;

export function isPostalCode(value: string): value is PostalCode {
  return POSTAL_CODE_REGEX.test(value);
}

export function createPostalCode(value: string): PostalCode {
  const trimmed = value.trim();
  if (!isPostalCode(trimmed)) {
    throw new Error(`Invalid PostalCode format: ${value}`);
  }
  return trimmed;
}

/**
 * Safely convert a string to PostalCode, returning undefined if invalid
 */
export function toPostalCode(value: string | null | undefined): PostalCode | undefined {
  if (!value) return undefined;
  const trimmed = value.trim();
  return isPostalCode(trimmed) ? trimmed : undefined;
}
