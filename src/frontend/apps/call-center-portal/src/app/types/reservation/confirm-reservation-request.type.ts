/**
 * Confirm reservation request
 */
import type { ReservationId } from '@orange-car-rental/reservation-api';

export interface ConfirmReservationRequest {
  readonly reservationId: ReservationId;
}
