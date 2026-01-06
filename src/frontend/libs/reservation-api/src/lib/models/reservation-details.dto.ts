/**
 * Vehicle and reservation details
 */
import type { ISODateString } from "@orange-car-rental/shared";
import type { VehicleId, CategoryCode } from "@orange-car-rental/vehicle-api";
import type { LocationCode } from "@orange-car-rental/location-api";

export type ReservationDetails = {
  readonly vehicleId: VehicleId;
  readonly categoryCode: CategoryCode;
  readonly pickupDate: ISODateString;
  readonly returnDate: ISODateString;
  readonly pickupLocationCode: LocationCode;
  readonly dropoffLocationCode: LocationCode;
};
