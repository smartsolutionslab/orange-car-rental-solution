/**
 * Grouped reservations for booking history
 */
import type { Reservation } from '@orange-car-rental/reservation-api';

export interface GroupedReservations {
  readonly upcoming: Reservation[];
  readonly past: Reservation[];
  readonly pending: Reservation[];
};
