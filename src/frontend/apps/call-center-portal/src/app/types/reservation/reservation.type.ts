/**
 * Call-center reservation model (extended version)
 */
import type { ReservationId, ReservationStatus } from '@orange-car-rental/reservation-api';
import type { CustomerId } from '@orange-car-rental/customer-api';
import type { VehicleId } from '@orange-car-rental/vehicle-api';
import type { LocationCode } from '@orange-car-rental/location-api';
import type { ISODateString, Price, Currency } from '@orange-car-rental/shared';
import type { RentalDays } from './rental-days.type';

export interface Reservation {
  readonly reservationId: ReservationId;
  readonly vehicleId: VehicleId;
  readonly customerId: CustomerId;
  readonly pickupDate: ISODateString;
  readonly returnDate: ISODateString;
  readonly pickupLocationCode: LocationCode;
  readonly dropoffLocationCode: LocationCode;
  readonly rentalDays: RentalDays;
  readonly totalPriceNet: Price;
  readonly totalPriceVat: Price;
  readonly totalPriceGross: Price;
  readonly currency: Currency;
  readonly status: ReservationStatus;
  readonly cancellationReason?: string;
  readonly createdAt: ISODateString;
  readonly confirmedAt?: ISODateString;
  readonly cancelledAt?: ISODateString;
  readonly completedAt?: ISODateString;
}
