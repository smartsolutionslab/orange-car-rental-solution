/**
 * Confirm reservation request
 */
import type { ReservationId } from '@orange-car-rental/reservation-api';

export type ConfirmReservationRequest = {
  readonly reservationId: ReservationId;
};
