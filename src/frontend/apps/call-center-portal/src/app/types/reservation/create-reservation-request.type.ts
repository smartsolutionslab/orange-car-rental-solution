/**
 * Create reservation request for registered customers
 */
import type {
  VehicleId,
  CustomerId,
  LocationCode,
  ISODateString,
  Price,
} from '@orange-car-rental/data-access';

export type CreateReservationRequest = {
  readonly vehicleId: VehicleId;
  readonly customerId: CustomerId;
  readonly pickupDate: ISODateString;
  readonly returnDate: ISODateString;
  readonly pickupLocationCode: LocationCode;
  readonly dropoffLocationCode: LocationCode;
  readonly totalPriceNet?: Price;
};
