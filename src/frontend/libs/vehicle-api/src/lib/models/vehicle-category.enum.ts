/**
 * Vehicle category enum - use VehicleCategory.Compact instead of 'KOMPAKT'
 */
export const VehicleCategory = {
  Economy: "KLEIN",
  Compact: "KOMPAKT",
  MidSize: "MITTEL",
  Luxury: "OBER",
  Van: "VAN",
  Suv: "SUV",
} as const;

export type VehicleCategory =
  (typeof VehicleCategory)[keyof typeof VehicleCategory];
