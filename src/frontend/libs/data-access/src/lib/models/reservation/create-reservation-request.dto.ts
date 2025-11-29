/**
 * Create reservation request matching backend CreateReservationCommand
 */
import type { ISODateString, Price } from '../common';
import type { VehicleId, CategoryCode } from '../vehicle';
import type { LocationCode } from '../location';
import type { CustomerId } from './customer-id.type';

export type CreateReservationRequest = {
  readonly vehicleId: VehicleId;
  readonly customerId: CustomerId;
  readonly categoryCode: CategoryCode;
  readonly pickupDate: ISODateString;
  readonly returnDate: ISODateString;
  readonly pickupLocationCode: LocationCode;
  readonly dropoffLocationCode: LocationCode;
  readonly totalPriceNet?: Price;
};
