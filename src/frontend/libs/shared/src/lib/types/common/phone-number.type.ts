/**
 * Phone Number - branded type for type safety
 * Matches backend PhoneNumber value object (German format)
 * Supports formats: +49 XXX XXXXXXX, 0XXX XXXXXXX, +49XXXXXXXXXX
 */
export type PhoneNumber = string & { readonly __brand: "PhoneNumber" };

// Matches German phone numbers and international formats
const PHONE_REGEX =
  /^(\+\d{1,3}[\s.-]?)?\(?\d{2,5}\)?[\s.-]?\d{3,}[\s.-]?\d{2,}$/;

export function isPhoneNumber(value: string): value is PhoneNumber {
  const cleaned = value.replace(/[\s.-]/g, "");
  return cleaned.length >= 8 && cleaned.length <= 20 && PHONE_REGEX.test(value);
}

export function createPhoneNumber(value: string): PhoneNumber {
  const trimmed = value.trim();
  if (!isPhoneNumber(trimmed)) {
    throw new Error(`Invalid PhoneNumber format: ${value}`);
  }
  return trimmed as PhoneNumber;
}

/**
 * Safely convert a string to PhoneNumber, returning undefined if invalid
 */
export function toPhoneNumber(
  value: string | null | undefined,
): PhoneNumber | undefined {
  if (!value) return undefined;
  const trimmed = value.trim();
  return isPhoneNumber(trimmed) ? (trimmed as PhoneNumber) : undefined;
}
