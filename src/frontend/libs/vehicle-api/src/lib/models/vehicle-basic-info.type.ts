/**
 * Vehicle basic info for add request
 */
import type { VehicleName } from './vehicle-name.type';
import type { Manufacturer } from './manufacturer.type';
import type { VehicleModel } from './vehicle-model.type';
import type { ManufacturingYear } from './manufacturing-year.type';
import type { ImageUrl } from './image-url.type';

export type VehicleBasicInfo = {
  readonly name: VehicleName;
  readonly manufacturer?: Manufacturer;
  readonly model?: VehicleModel;
  readonly year?: ManufacturingYear;
  readonly imageUrl?: ImageUrl;
};
