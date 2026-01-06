/**
 * First Name - branded type for type safety
 * Matches backend FirstName value object
 */
export type FirstName = string & { readonly __brand: "FirstName" };

const MIN_LENGTH = 1;
const MAX_LENGTH = 100;
// Allows letters, spaces, hyphens, apostrophes (for names like O'Brien, Mary-Jane)
const NAME_REGEX = /^[\p{L}\s'-]+$/u;

export function isFirstName(value: string): value is FirstName {
  return (
    value.length >= MIN_LENGTH &&
    value.length <= MAX_LENGTH &&
    NAME_REGEX.test(value)
  );
}

export function createFirstName(value: string): FirstName {
  const trimmed = value.trim();
  if (!isFirstName(trimmed)) {
    throw new Error(`Invalid FirstName format: ${value}`);
  }
  return trimmed;
}

/**
 * Safely convert a string to FirstName, returning undefined if invalid
 */
export function toFirstName(
  value: string | null | undefined,
): FirstName | undefined {
  if (!value) return undefined;
  const trimmed = value.trim();
  return isFirstName(trimmed) ? trimmed : undefined;
}
