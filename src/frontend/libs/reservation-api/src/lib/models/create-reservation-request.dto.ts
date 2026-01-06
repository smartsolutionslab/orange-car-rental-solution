/**
 * Create reservation request matching backend CreateReservationCommand
 */
import type { ISODateString, Price } from "@orange-car-rental/shared";
import type { VehicleId, CategoryCode } from "@orange-car-rental/vehicle-api";
import type { LocationCode } from "@orange-car-rental/location-api";
import type { CustomerId } from "./customer-id.type";

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
