/**
 * Reservation ID - branded type for type safety
 * Format: GUID (xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx)
 * Matches backend ReservationIdentifier value object
 */
export type ReservationId = string & { readonly __brand: 'ReservationId' };

const GUID_REGEX = /^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$/i;

export function isReservationId(value: string): value is ReservationId {
  return GUID_REGEX.test(value);
}

export function createReservationId(value: string): ReservationId {
  if (!isReservationId(value)) {
    throw new Error(`Invalid ReservationId format: ${value}`);
  }
  return value;
}

/**
 * Safely convert a string to ReservationId, returning undefined if invalid
 */
export function toReservationId(value: string | null | undefined): ReservationId | undefined {
  if (!value) return undefined;
  return isReservationId(value) ? value : undefined;
}
