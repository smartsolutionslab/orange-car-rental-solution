/**
 * Fuel type enum - use FuelType.Petrol instead of 'Petrol'
 */
export const FuelType = {
  Petrol: "Petrol",
  Diesel: "Diesel",
  Electric: "Electric",
  Hybrid: "Hybrid",
} as const;

export type FuelType = (typeof FuelType)[keyof typeof FuelType];
