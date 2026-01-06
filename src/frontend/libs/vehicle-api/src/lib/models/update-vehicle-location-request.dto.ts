/**
 * Update vehicle location request
 */
import type { LocationCode } from "@orange-car-rental/location-api";

export type UpdateVehicleLocationRequest = {
  readonly locationCode: LocationCode;
};
