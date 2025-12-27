/**
 * Create reservation response
 */
import type { ReservationId, ReservationStatus } from '@orange-car-rental/reservation-api';
import type { Price, Currency } from '@orange-car-rental/shared';

export interface CreateReservationResponse {
  readonly reservationId: ReservationId;
  readonly status: ReservationStatus;
  readonly totalPriceNet: Price;
  readonly totalPriceVat: Price;
  readonly totalPriceGross: Price;
  readonly currency: Currency;
}
