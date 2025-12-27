/**
 * Location with vehicle distribution
 */
import type { Location } from '@orange-car-rental/location-api';
import type { VehicleDistribution } from './vehicle-distribution.type';

export type LocationWithDistribution = Location & {
  readonly distribution: VehicleDistribution;
};
