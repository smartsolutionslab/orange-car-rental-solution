/**
 * Update vehicle location request
 */
import type { LocationCode } from '../location/location-code.type';

export type UpdateVehicleLocationRequest = {
  readonly locationCode: LocationCode;
};
