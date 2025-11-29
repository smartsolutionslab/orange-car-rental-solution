/**
 * Vehicle and reservation details
 */
import type { ISODateString } from '../common';
import type { VehicleId, CategoryCode } from '../vehicle';
import type { LocationCode } from '../location';

export type ReservationDetails = {
  readonly vehicleId: VehicleId;
  readonly categoryCode: CategoryCode;
  readonly pickupDate: ISODateString;
  readonly returnDate: ISODateString;
  readonly pickupLocationCode: LocationCode;
  readonly dropoffLocationCode: LocationCode;
};
