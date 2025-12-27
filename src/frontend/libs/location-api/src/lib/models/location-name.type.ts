/**
 * Location Name - branded type for type safety
 * Matches backend LocationName value object
 */
export type LocationName = string & { readonly __brand: 'LocationName' };

const MAX_LENGTH = 200;

export function isLocationName(value: string): value is LocationName {
  return value.length > 0 && value.length <= MAX_LENGTH;
}

export function createLocationName(value: string): LocationName {
  const trimmed = value.trim();
  if (!isLocationName(trimmed)) {
    throw new Error(`Invalid LocationName: ${value}`);
  }
  return trimmed;
}

/**
 * Safely convert a string to LocationName, returning undefined if invalid
 */
export function toLocationName(value: string | null | undefined): LocationName | undefined {
  if (!value) return undefined;
  const trimmed = value.trim();
  return isLocationName(trimmed) ? trimmed : undefined;
}
