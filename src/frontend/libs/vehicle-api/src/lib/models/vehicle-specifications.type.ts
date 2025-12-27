/**
 * Vehicle specifications for add request
 */
import type { CategoryCode } from './category-code.type';
import type { SeatingCapacity } from './seating-capacity.type';
import type { FuelType } from './fuel-type.enum';
import type { TransmissionType } from './transmission-type.enum';

export type VehicleSpecifications = {
  readonly category: CategoryCode;
  readonly seats: SeatingCapacity;
  readonly fuelType: FuelType;
  readonly transmissionType: TransmissionType;
};
