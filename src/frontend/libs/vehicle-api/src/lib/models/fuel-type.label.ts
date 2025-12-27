/**
 * Fuel type labels (German)
 */
import type { FuelType } from './fuel-type.enum';
import { FuelType as FT } from './fuel-type.enum';

export const FuelTypeLabel: Record<FuelType, string> = {
  [FT.Petrol]: 'Benzin',
  [FT.Diesel]: 'Diesel',
  [FT.Electric]: 'Elektro',
  [FT.Hybrid]: 'Hybrid',
};
