/**
 * Grouped reservations for booking history
 */
import type { Reservation } from '@orange-car-rental/data-access';

export type GroupedReservations = {
  readonly upcoming: Reservation[];
  readonly past: Reservation[];
  readonly pending: Reservation[];
};
