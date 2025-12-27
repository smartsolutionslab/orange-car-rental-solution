/**
 * Vehicle status enum - use VehicleStatus.Available instead of 'Available'
 */
export const VehicleStatus = {
  Available: 'Available',
  Rented: 'Rented',
  Maintenance: 'Maintenance',
  OutOfService: 'OutOfService',
  Reserved: 'Reserved',
} as const;

export type VehicleStatus = (typeof VehicleStatus)[keyof typeof VehicleStatus];
