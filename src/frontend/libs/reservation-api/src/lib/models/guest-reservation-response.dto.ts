/**
 * Response from creating a guest reservation
 * Includes both customer and reservation IDs along with pricing breakdown
 */
import type { Currency, Price } from '@orange-car-rental/shared';
import type { CustomerId } from './customer-id.type';
import type { ReservationId } from './reservation-id.type';

export type GuestReservationResponse = {
  readonly customerId: CustomerId;
  readonly reservationId: ReservationId;
  readonly totalPriceNet: Price;
  readonly totalPriceVat: Price;
  readonly totalPriceGross: Price;
  readonly currency: Currency;
};
