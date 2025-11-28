// Branded types for type safety
export type VehicleId = string;
export type VehicleName = string;
export type CategoryCode = string;
export type CategoryName = string;
export type LocationCode = string;
export type CityName = string;
export type SeatingCapacity = number;
export type DailyRate = number;
export type LicensePlate = string;
export type Manufacturer = string;
export type VehicleModel = string;
export type ManufacturingYear = number;
export type ImageUrl = string;

// Union types for constrained values
export type FuelType = 'Petrol' | 'Diesel' | 'Electric' | 'Hybrid';
export type TransmissionType = 'Manual' | 'Automatic';
export type Currency = 'EUR';
export type VehicleStatus = 'Available' | 'Rented' | 'Maintenance' | 'OutOfService' | 'Reserved';

/**
 * Vehicle data model matching the backend VehicleDto
 * All prices include German VAT (19%)
 */
export interface Vehicle {
  readonly id: VehicleId;
  readonly name: VehicleName;
  readonly categoryCode: CategoryCode;
  readonly categoryName: CategoryName;
  readonly locationCode: LocationCode;
  readonly city: CityName;
  readonly seats: SeatingCapacity;
  readonly fuelType: FuelType;
  readonly transmissionType: TransmissionType;
  readonly dailyRateNet: DailyRate;
  readonly dailyRateVat: DailyRate;
  readonly dailyRateGross: DailyRate;
  readonly currency: Currency;
  readonly status: VehicleStatus;
  readonly licensePlate: LicensePlate | null;
  readonly manufacturer: Manufacturer;
  readonly model: VehicleModel;
  readonly year: ManufacturingYear;
  readonly imageUrl: ImageUrl | null;
}

/**
 * Search query parameters for vehicle search
 */
export interface VehicleSearchQuery {
  readonly pickupDate?: string;
  readonly returnDate?: string;
  readonly locationCode?: LocationCode;
  readonly categoryCode?: CategoryCode;
  readonly minSeats?: SeatingCapacity;
  readonly fuelType?: FuelType;
  readonly transmissionType?: TransmissionType;
  readonly maxDailyRateGross?: DailyRate;
  readonly pageNumber?: number;
  readonly pageSize?: number;
}

/**
 * Search result with pagination metadata
 */
export interface VehicleSearchResult {
  readonly vehicles: Vehicle[];
  readonly totalCount: number;
  readonly pageNumber: number;
  readonly pageSize: number;
  readonly totalPages: number;
  readonly hasPreviousPage?: boolean;
  readonly hasNextPage?: boolean;
}

/**
 * Constants for fuel types
 */
export const FUEL_TYPES = ['Petrol', 'Diesel', 'Electric', 'Hybrid'] as const;

/**
 * Constants for transmission types
 */
export const TRANSMISSION_TYPES = ['Manual', 'Automatic'] as const;

/**
 * Constants for vehicle statuses with German labels
 */
export const VEHICLE_STATUSES = [
  { code: 'Available', label: 'Verfügbar' },
  { code: 'Rented', label: 'Vermietet' },
  { code: 'Maintenance', label: 'Wartung' },
  { code: 'OutOfService', label: 'Außer Betrieb' },
  { code: 'Reserved', label: 'Reserviert' }
] as const satisfies readonly { code: VehicleStatus; label: string }[];

/**
 * Constants for vehicle categories with German labels
 */
export const VEHICLE_CATEGORIES = [
  { code: 'KLEIN', name: 'Kleinwagen' },
  { code: 'KOMPAKT', name: 'Kompaktklasse' },
  { code: 'MITTEL', name: 'Mittelklasse' },
  { code: 'OBER', name: 'Oberklasse' },
  { code: 'VAN', name: 'Van/Transporter' },
  { code: 'SUV', name: 'SUV' }
] as const satisfies readonly { code: CategoryCode; name: CategoryName }[];
