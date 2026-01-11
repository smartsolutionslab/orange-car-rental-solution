/**
 * Fuel type enum - use FuelType.Petrol instead of 'Petrol'
 * Matches backend FuelType enum
 */
export const FuelType = {
  Petrol: "Petrol",
  Diesel: "Diesel",
  Electric: "Electric",
  Hybrid: "Hybrid",
  PlugInHybrid: "PlugInHybrid",
  Hydrogen: "Hydrogen",
} as const;

export type FuelType = (typeof FuelType)[keyof typeof FuelType];
