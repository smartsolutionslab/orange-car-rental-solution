/**
 * Driver's License Number - branded type for type safety
 * Matches backend LicenseNumber value object
 * Various international formats supported
 */
export type LicenseNumber = string & { readonly __brand: 'LicenseNumber' };

const MIN_LENGTH = 3;
const MAX_LENGTH = 30;

export function isLicenseNumber(value: string): value is LicenseNumber {
  return value.length >= MIN_LENGTH && value.length <= MAX_LENGTH;
}

export function createLicenseNumber(value: string): LicenseNumber {
  const trimmed = value.trim();
  if (!isLicenseNumber(trimmed)) {
    throw new Error(`Invalid LicenseNumber format: ${value}`);
  }
  return trimmed;
}

/**
 * Safely convert a string to LicenseNumber, returning undefined if invalid
 */
export function toLicenseNumber(value: string | null | undefined): LicenseNumber | undefined {
  if (!value) return undefined;
  const trimmed = value.trim();
  return isLicenseNumber(trimmed) ? trimmed : undefined;
}
