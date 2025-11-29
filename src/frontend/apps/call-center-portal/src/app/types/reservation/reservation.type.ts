/**
 * Call-center reservation model (extended version)
 */
import type {
  ReservationId,
  CustomerId,
  VehicleId,
  LocationCode,
  ISODateString,
  Price,
  Currency,
  ReservationStatus,
} from '@orange-car-rental/data-access';
import type { RentalDays } from './rental-days.type';

export type Reservation = {
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
};
