/**
 * Location with availability information
 */
import type { Location } from "./location.dto";

export type LocationWithAvailability = Location & {
  readonly availableVehicleCount: number;
  readonly isOpen: boolean;
};
