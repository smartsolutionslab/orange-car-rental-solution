/**
 * Manufacturer - branded type for type safety
 * Matches backend Manufacturer value object
 */
export type Manufacturer = string & { readonly __brand: 'Manufacturer' };

const MAX_LENGTH = 100;

export function isManufacturer(value: string): value is Manufacturer {
  return value.length > 0 && value.length <= MAX_LENGTH;
}

export function createManufacturer(value: string): Manufacturer {
  const trimmed = value.trim();
  if (!isManufacturer(trimmed)) {
    throw new Error(`Invalid Manufacturer: ${value}`);
  }
  return trimmed;
}

/**
 * Safely convert a string to Manufacturer, returning undefined if invalid
 */
export function toManufacturer(value: string | null | undefined): Manufacturer | undefined {
  if (!value) return undefined;
  const trimmed = value.trim();
  return isManufacturer(trimmed) ? trimmed : undefined;
}
