/**
 * Time String - branded type for type safety
 * Format: "HH:MM" (24-hour format)
 * Matches backend TimeOnly value object
 */
export type TimeString = string & { readonly __brand: "TimeString" };

const TIME_REGEX = /^([01]\d|2[0-3]):([0-5]\d)$/;

export function isTimeString(value: string): value is TimeString {
  return TIME_REGEX.test(value);
}

export function createTimeString(value: string): TimeString {
  const trimmed = value.trim();
  if (!isTimeString(trimmed)) {
    throw new Error(
      `Invalid TimeString format: ${value}. Expected HH:MM format.`,
    );
  }
  return trimmed;
}

/**
 * Safely convert a string to TimeString, returning undefined if invalid
 */
export function toTimeString(
  value: string | null | undefined,
): TimeString | undefined {
  if (!value) return undefined;
  const trimmed = value.trim();
  return isTimeString(trimmed) ? trimmed : undefined;
}
