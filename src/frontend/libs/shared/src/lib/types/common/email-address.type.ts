/**
 * Email Address - branded type for type safety
 * Matches backend Email value object (RFC 5322 format)
 */
export type EmailAddress = string & { readonly __brand: 'EmailAddress' };

// Simplified email regex that covers most valid emails
const EMAIL_REGEX = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
const MAX_LENGTH = 254;

export function isEmailAddress(value: string): value is EmailAddress {
  return value.length <= MAX_LENGTH && EMAIL_REGEX.test(value);
}

export function createEmailAddress(value: string): EmailAddress {
  const normalized = value.toLowerCase().trim();
  if (!isEmailAddress(normalized)) {
    throw new Error(`Invalid EmailAddress format: ${value}`);
  }
  return normalized;
}

/**
 * Safely convert a string to EmailAddress, returning undefined if invalid
 */
export function toEmailAddress(value: string | null | undefined): EmailAddress | undefined {
  if (!value) return undefined;
  const normalized = value.toLowerCase().trim();
  return isEmailAddress(normalized) ? normalized : undefined;
}
