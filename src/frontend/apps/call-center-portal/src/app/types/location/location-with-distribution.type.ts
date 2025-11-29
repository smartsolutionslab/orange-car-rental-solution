/**
 * Location with vehicle distribution
 */
import type { Location } from '@orange-car-rental/data-access';
import type { VehicleDistribution } from './vehicle-distribution.type';

export type LocationWithDistribution = Location & {
  readonly distribution: VehicleDistribution;
};
