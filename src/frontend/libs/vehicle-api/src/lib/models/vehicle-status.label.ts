/**
 * Vehicle status labels (German)
 */
import type { VehicleStatus } from "./vehicle-status.enum";
import { VehicleStatus as VS } from "./vehicle-status.enum";

export const VehicleStatusLabel: Record<VehicleStatus, string> = {
  [VS.Available]: "Verfügbar",
  [VS.Rented]: "Vermietet",
  [VS.Maintenance]: "Wartung",
  [VS.OutOfService]: "Außer Betrieb",
  [VS.Reserved]: "Reserviert",
};
