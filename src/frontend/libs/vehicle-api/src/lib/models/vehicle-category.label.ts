/**
 * Vehicle category labels (German)
 */
import type { VehicleCategory } from './vehicle-category.enum';
import { VehicleCategory as VC } from './vehicle-category.enum';

export const VehicleCategoryLabel: Record<VehicleCategory, string> = {
  [VC.Economy]: 'Kleinwagen',
  [VC.Compact]: 'Kompaktklasse',
  [VC.MidSize]: 'Mittelklasse',
  [VC.Luxury]: 'Oberklasse',
  [VC.Van]: 'Van/Transporter',
  [VC.Suv]: 'SUV',
};
