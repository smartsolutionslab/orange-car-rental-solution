/**
 * Vehicle registration for add request
 */
import type { LicensePlate } from "./license-plate.type";

export type VehicleRegistration = {
  readonly licensePlate?: LicensePlate;
};
