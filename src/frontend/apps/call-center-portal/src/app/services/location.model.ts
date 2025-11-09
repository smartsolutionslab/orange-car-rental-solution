// Custom types for type safety
export type LocationCode = string;
export type LocationName = string;
export type StreetAddress = string;
export type CityName = string;
export type PostalCode = string;
export type FullAddress = string;

/**
 * Location model representing a rental location
 */
export interface Location {
  code: LocationCode;
  name: LocationName;
  street: StreetAddress;
  city: CityName;
  postalCode: PostalCode;
  fullAddress: FullAddress;
}

/**
 * Location statistics
 */
export interface LocationStatistics {
  locationCode: LocationCode;
  locationName: LocationName;
  totalVehicles: number;
  availableVehicles: number;
  rentedVehicles: number;
  maintenanceVehicles: number;
  outOfServiceVehicles: number;
  utilizationRate: number; // Percentage
}

/**
 * Vehicle distribution by status at a location
 */
export interface VehicleDistribution {
  available: number;
  rented: number;
  maintenance: number;
  outOfService: number;
  reserved: number;
}
