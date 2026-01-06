/**
 * Location status labels (German)
 */
import type { LocationStatus } from "./location-status.enum";
import { LocationStatus as LS } from "./location-status.enum";

export const LocationStatusLabel: Record<LocationStatus, string> = {
  [LS.Active]: "Aktiv",
  [LS.Inactive]: "Inaktiv",
  [LS.Maintenance]: "Wartung",
};
