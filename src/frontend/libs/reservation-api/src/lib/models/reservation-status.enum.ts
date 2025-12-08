/**
 * Reservation status enum - use ReservationStatus.Pending instead of 'Pending'
 */
export const ReservationStatus = {
  Pending: 'Pending',
  Confirmed: 'Confirmed',
  Active: 'Active',
  Completed: 'Completed',
  Cancelled: 'Cancelled',
} as const;

export type ReservationStatus = (typeof ReservationStatus)[keyof typeof ReservationStatus];
