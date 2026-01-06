/**
 * Seating Capacity - branded type for type safety
 * Matches backend SeatingCapacity value object
 * Valid range: 2-9 seats
 */
export type SeatingCapacity = number & { readonly __brand: "SeatingCapacity" };

const MIN_SEATS = 2;
const MAX_SEATS = 9;

export function isSeatingCapacity(value: number): value is SeatingCapacity {
  return Number.isInteger(value) && value >= MIN_SEATS && value <= MAX_SEATS;
}

export function createSeatingCapacity(value: number): SeatingCapacity {
  if (!isSeatingCapacity(value)) {
    throw new Error(
      `Invalid SeatingCapacity: ${value}. Must be between ${MIN_SEATS} and ${MAX_SEATS}.`,
    );
  }
  return value;
}

/**
 * Safely convert a number to SeatingCapacity, returning undefined if invalid
 */
export function toSeatingCapacity(
  value: number | null | undefined,
): SeatingCapacity | undefined {
  if (value === null || value === undefined) return undefined;
  return isSeatingCapacity(value) ? value : undefined;
}
