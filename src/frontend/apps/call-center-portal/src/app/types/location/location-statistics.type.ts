/**
 * Location statistics
 */
import type { LocationCode, LocationName } from '@orange-car-rental/data-access';

export type LocationStatistics = {
  readonly locationCode: LocationCode;
  readonly locationName: LocationName;
  readonly totalVehicles: number;
  readonly availableVehicles: number;
  readonly rentedVehicles: number;
  readonly maintenanceVehicles: number;
  readonly outOfServiceVehicles: number;
  readonly utilizationRate: number;
};
