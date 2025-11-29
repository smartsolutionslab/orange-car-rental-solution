/**
 * Add vehicle result
 */
import type { VehicleId } from './vehicle-id.type';
import type { VehicleName } from './vehicle-name.type';
import type { CategoryCode } from './category-code.type';
import type { DailyRate } from './daily-rate.type';
import type { VehicleStatus } from './vehicle-status.enum';
import type { LocationCode } from '../location';

export type AddVehicleResult = {
  readonly vehicleId: VehicleId;
  readonly name: VehicleName;
  readonly category: CategoryCode;
  readonly location: LocationCode;
  readonly dailyRateGross: DailyRate;
  readonly status: VehicleStatus;
};
