/**
 * Location status enum - use LocationStatus.Active instead of 'Active'
 */
export const LocationStatus = {
  Active: "Active",
  Inactive: "Inactive",
  Maintenance: "Maintenance",
} as const;

export type LocationStatus =
  (typeof LocationStatus)[keyof typeof LocationStatus];
