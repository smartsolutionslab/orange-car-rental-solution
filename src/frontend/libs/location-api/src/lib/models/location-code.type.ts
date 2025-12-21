/**
 * Location Code - branded type for type safety
 * Format: 3+ uppercase characters (e.g., "BER-HBF", "MUC-FLG")
 * Matches backend LocationCode value object
 */
export type LocationCode = string & { readonly __brand: 'LocationCode' };

const LOCATION_CODE_REGEX = /^[A-Z0-9-]{3,20}$/;

export function isLocationCode(value: string): value is LocationCode {
  return LOCATION_CODE_REGEX.test(value);
}

export function createLocationCode(value: string): LocationCode {
  const normalized = value.toUpperCase();
  if (!isLocationCode(normalized)) {
    throw new Error(`Invalid LocationCode format: ${value}`);
  }
  return normalized;
}

/**
 * Safely convert a string to LocationCode, returning undefined if invalid
 */
export function toLocationCode(value: string | null | undefined): LocationCode | undefined {
  if (!value) return undefined;
  const normalized = value.toUpperCase();
  return isLocationCode(normalized) ? normalized : undefined;
}
