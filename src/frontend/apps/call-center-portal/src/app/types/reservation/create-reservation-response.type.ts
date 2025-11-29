/**
 * Create reservation response
 */
import type {
  ReservationId,
  ReservationStatus,
  Price,
  Currency,
} from '@orange-car-rental/data-access';

export type CreateReservationResponse = {
  readonly reservationId: ReservationId;
  readonly status: ReservationStatus;
  readonly totalPriceNet: Price;
  readonly totalPriceVat: Price;
  readonly totalPriceGross: Price;
  readonly currency: Currency;
};
