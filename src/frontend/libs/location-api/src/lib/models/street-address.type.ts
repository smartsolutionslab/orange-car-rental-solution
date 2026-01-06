/**
 * Street Address - branded type for type safety
 * Matches backend StreetAddress value object
 */
export type StreetAddress = string & { readonly __brand: "StreetAddress" };

const MIN_LENGTH = 1;
const MAX_LENGTH = 200;

export function isStreetAddress(value: string): value is StreetAddress {
  return value.length >= MIN_LENGTH && value.length <= MAX_LENGTH;
}

export function createStreetAddress(value: string): StreetAddress {
  const trimmed = value.trim();
  if (!isStreetAddress(trimmed)) {
    throw new Error(`Invalid StreetAddress format: ${value}`);
  }
  return trimmed;
}

/**
 * Safely convert a string to StreetAddress, returning undefined if invalid
 */
export function toStreetAddress(
  value: string | null | undefined,
): StreetAddress | undefined {
  if (!value) return undefined;
  const trimmed = value.trim();
  return isStreetAddress(trimmed) ? trimmed : undefined;
}
