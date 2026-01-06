/**
 * Country Code - branded type for type safety
 * ISO 3166-1 alpha-2 country codes (e.g., "DE", "AT", "CH")
 */
export type CountryCode = string & { readonly __brand: "CountryCode" };

// ISO 3166-1 alpha-2: exactly 2 uppercase letters
const COUNTRY_CODE_REGEX = /^[A-Z]{2}$/;

// Common EU country codes for validation
const EU_COUNTRY_CODES = new Set([
  "AT",
  "BE",
  "BG",
  "CY",
  "CZ",
  "DE",
  "DK",
  "EE",
  "ES",
  "FI",
  "FR",
  "GR",
  "HR",
  "HU",
  "IE",
  "IT",
  "LT",
  "LU",
  "LV",
  "MT",
  "NL",
  "PL",
  "PT",
  "RO",
  "SE",
  "SI",
  "SK",
]);

export function isCountryCode(value: string): value is CountryCode {
  return COUNTRY_CODE_REGEX.test(value);
}

export function createCountryCode(value: string): CountryCode {
  const normalized = value.toUpperCase().trim();
  if (!isCountryCode(normalized)) {
    throw new Error(`Invalid CountryCode format: ${value}`);
  }
  return normalized;
}

/**
 * Safely convert a string to CountryCode, returning undefined if invalid
 */
export function toCountryCode(
  value: string | null | undefined,
): CountryCode | undefined {
  if (!value) return undefined;
  const normalized = value.toUpperCase().trim();
  return isCountryCode(normalized) ? normalized : undefined;
}

/**
 * Check if country code is an EU member state
 */
export function isEuCountry(code: CountryCode): boolean {
  return EU_COUNTRY_CODES.has(code);
}
