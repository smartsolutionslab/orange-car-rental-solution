/**
 * Update vehicle status request
 */
import type { VehicleStatus } from "./vehicle-status.enum";

export type UpdateVehicleStatusRequest = {
  readonly status: VehicleStatus;
};
