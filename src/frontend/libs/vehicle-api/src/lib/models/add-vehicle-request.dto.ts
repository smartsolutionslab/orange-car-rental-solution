/**
 * Add vehicle to fleet request
 */
import type { VehicleBasicInfo } from './vehicle-basic-info.type';
import type { VehicleSpecifications } from './vehicle-specifications.type';
import type { VehicleLocationAndPricing } from './vehicle-location-and-pricing.type';
import type { VehicleRegistration } from './vehicle-registration.type';

export type AddVehicleRequest = {
  readonly basicInfo: VehicleBasicInfo;
  readonly specifications: VehicleSpecifications;
  readonly locationAndPricing: VehicleLocationAndPricing;
  readonly registration?: VehicleRegistration;
};
