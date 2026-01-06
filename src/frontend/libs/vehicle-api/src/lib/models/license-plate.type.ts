/**
 * License Plate - branded type for type safety
 * Matches backend LicensePlate value object (German format)
 * Format: B AB 1234, M-XY 123, etc.
 */
export type LicensePlate = string & { readonly __brand: "LicensePlate" };

// German license plate format with optional umlauts
const LICENSE_PLATE_REGEX =
  /^[A-ZÄÖÜ]{1,3}[-\s]?[A-ZÄÖÜ]{1,2}[-\s]?\d{1,4}[EH]?$/i;
const MAX_LENGTH = 20;

export function isLicensePlate(value: string): value is LicensePlate {
  return value.length <= MAX_LENGTH && LICENSE_PLATE_REGEX.test(value);
}

export function createLicensePlate(value: string): LicensePlate {
  const normalized = value.toUpperCase().trim();
  if (!isLicensePlate(normalized)) {
    throw new Error(`Invalid LicensePlate format: ${value}`);
  }
  return normalized;
}

/**
 * Safely convert a string to LicensePlate, returning undefined if invalid
 */
export function toLicensePlate(
  value: string | null | undefined,
): LicensePlate | undefined {
  if (!value) return undefined;
  const normalized = value.toUpperCase().trim();
  return isLicensePlate(normalized) ? normalized : undefined;
}
