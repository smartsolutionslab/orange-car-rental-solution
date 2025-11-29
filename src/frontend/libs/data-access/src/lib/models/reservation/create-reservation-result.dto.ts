/**
 * Create reservation result matching backend CreateReservationResult
 */
import type { Price } from '../common';
import type { ReservationId } from './reservation-id.type';
import type { ReservationStatus } from './reservation-status.enum';

export type CreateReservationResult = {
  readonly reservationId: ReservationId;
  readonly status: ReservationStatus;
  readonly totalPriceNet: Price;
  readonly totalPriceVat: Price;
  readonly totalPriceGross: Price;
};
