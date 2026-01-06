/**
 * City Name - branded type for type safety
 * Matches backend City value object
 */
export type CityName = string & { readonly __brand: "CityName" };

const MAX_LENGTH = 100;

export function isCityName(value: string): value is CityName {
  return value.length > 0 && value.length <= MAX_LENGTH;
}

export function createCityName(value: string): CityName {
  const trimmed = value.trim();
  if (!isCityName(trimmed)) {
    throw new Error(`Invalid CityName: ${value}`);
  }
  return trimmed;
}

/**
 * Safely convert a string to CityName, returning undefined if invalid
 */
export function toCityName(
  value: string | null | undefined,
): CityName | undefined {
  if (!value) return undefined;
  const trimmed = value.trim();
  return isCityName(trimmed) ? trimmed : undefined;
}
