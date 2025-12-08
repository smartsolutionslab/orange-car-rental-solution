/**
 * Create reservation request for registered customers
 */
import type { VehicleId } from '@orange-car-rental/vehicle-api';
import type { CustomerId } from '@orange-car-rental/reservation-api';
import type { LocationCode } from '@orange-car-rental/location-api';
import type { ISODateString, Price } from '@orange-car-rental/shared';

export type CreateReservationRequest = {
  readonly vehicleId: VehicleId;
  readonly customerId: CustomerId;
  readonly pickupDate: ISODateString;
  readonly returnDate: ISODateString;
  readonly pickupLocationCode: LocationCode;
  readonly dropoffLocationCode: LocationCode;
  readonly totalPriceNet?: Price;
};
