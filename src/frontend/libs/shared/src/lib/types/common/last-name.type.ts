/**
 * Last Name - branded type for type safety
 * Matches backend LastName value object
 */
export type LastName = string & { readonly __brand: "LastName" };

const MIN_LENGTH = 1;
const MAX_LENGTH = 100;
// Allows letters, spaces, hyphens, apostrophes (for names like O'Brien, van der Berg)
const NAME_REGEX = /^[\p{L}\s'-]+$/u;

export function isLastName(value: string): value is LastName {
  return (
    value.length >= MIN_LENGTH &&
    value.length <= MAX_LENGTH &&
    NAME_REGEX.test(value)
  );
}

export function createLastName(value: string): LastName {
  const trimmed = value.trim();
  if (!isLastName(trimmed)) {
    throw new Error(`Invalid LastName format: ${value}`);
  }
  return trimmed;
}

/**
 * Safely convert a string to LastName, returning undefined if invalid
 */
export function toLastName(
  value: string | null | undefined,
): LastName | undefined {
  if (!value) return undefined;
  const trimmed = value.trim();
  return isLastName(trimmed) ? trimmed : undefined;
}
