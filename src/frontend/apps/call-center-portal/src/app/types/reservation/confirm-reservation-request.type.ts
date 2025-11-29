/**
 * Confirm reservation request
 */
import type { ReservationId } from '@orange-car-rental/data-access';

export type ConfirmReservationRequest = {
  readonly reservationId: ReservationId;
};
