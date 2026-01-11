/**
 * Location status enum - use LocationStatus.Active instead of 'Active'
 * Matches backend LocationStatus enum
 */
export const LocationStatus = {
  Active: "Active",
  Closed: "Closed",
  UnderMaintenance: "UnderMaintenance",
  Inactive: "Inactive",
} as const;

export type LocationStatus =
  (typeof LocationStatus)[keyof typeof LocationStatus];
