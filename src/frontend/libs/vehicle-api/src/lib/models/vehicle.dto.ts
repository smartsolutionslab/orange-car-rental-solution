/**
 * Vehicle data model matching the backend VehicleDto
 * All prices include German VAT (19%)
 */
import type { Currency } from '@orange-car-rental/shared';
import type { LocationCode, CityName } from '@orange-car-rental/location-api';
import type { VehicleId } from './vehicle-id.type';
import type { VehicleName } from './vehicle-name.type';
import type { CategoryCode } from './category-code.type';
import type { CategoryName } from './category-name.type';
import type { SeatingCapacity } from './seating-capacity.type';
import type { DailyRate } from './daily-rate.type';
import type { LicensePlate } from './license-plate.type';
import type { Manufacturer } from './manufacturer.type';
import type { VehicleModel } from './vehicle-model.type';
import type { ManufacturingYear } from './manufacturing-year.type';
import type { ImageUrl } from './image-url.type';
import type { FuelType } from './fuel-type.enum';
import type { TransmissionType } from './transmission-type.enum';
import type { VehicleStatus } from './vehicle-status.enum';

export type Vehicle = {
  readonly id: VehicleId;
  readonly name: VehicleName;
  readonly categoryCode: CategoryCode;
  readonly categoryName: CategoryName;
  readonly locationCode: LocationCode;
  readonly city: CityName;
  readonly seats: SeatingCapacity;
  readonly fuelType: FuelType;
  readonly transmissionType: TransmissionType;
  readonly dailyRateNet: DailyRate;
  readonly dailyRateVat: DailyRate;
  readonly dailyRateGross: DailyRate;
  readonly currency: Currency;
  readonly status: VehicleStatus;
  readonly licensePlate: LicensePlate | null;
  readonly manufacturer: Manufacturer;
  readonly model: VehicleModel;
  readonly year: ManufacturingYear;
  readonly imageUrl: ImageUrl | null;
};
