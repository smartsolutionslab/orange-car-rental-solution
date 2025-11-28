import { LocationCode, CityName } from './vehicle.model';

// Branded types for location domain
export type LocationName = string;
export type StreetAddress = string;
export type PostalCode = string;
export type FullAddress = string;

// Union type for location status
export type LocationStatus = 'Active' | 'Inactive' | 'Maintenance';

// Re-export LocationCode for convenience
export type { LocationCode, CityName };

/**
 * Location model representing a rental location
 */
export interface Location {
  readonly code: LocationCode;
  readonly name: LocationName;
  readonly street: StreetAddress;
  readonly city: CityName;
  readonly postalCode: PostalCode;
  readonly fullAddress: FullAddress;
  readonly status?: LocationStatus;
}

/**
 * Location statistics
 */
export interface LocationStatistics {
  readonly locationCode: LocationCode;
  readonly locationName: LocationName;
  readonly totalVehicles: number;
  readonly availableVehicles: number;
  readonly rentedVehicles: number;
  readonly maintenanceVehicles: number;
  readonly outOfServiceVehicles: number;
  readonly utilizationRate: number; // Percentage 0-100
}

/**
 * Vehicle distribution by status at a location
 */
export interface VehicleDistribution {
  readonly available: number;
  readonly rented: number;
  readonly maintenance: number;
  readonly outOfService: number;
  readonly reserved: number;
}

/**
 * Location with vehicle distribution
 */
export interface LocationWithDistribution extends Location {
  readonly distribution: VehicleDistribution;
}

/**
 * Constants for location statuses with German labels
 */
export const LOCATION_STATUSES = [
  { code: 'Active', label: 'Aktiv' },
  { code: 'Inactive', label: 'Inaktiv' },
  { code: 'Maintenance', label: 'Wartung' }
] as const satisfies readonly { code: LocationStatus; label: string }[];
