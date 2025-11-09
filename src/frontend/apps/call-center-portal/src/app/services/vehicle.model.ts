// Custom types for type safety
export type VehicleId = string;
export type VehicleName = string;
export type CategoryCode = string;
export type CategoryName = string;
export type LocationCode = string;
export type CityName = string;
export type SeatingCapacity = number;
export type FuelType = 'Petrol' | 'Diesel' | 'Electric' | 'Hybrid';
export type TransmissionType = 'Manual' | 'Automatic';
export type DailyRate = number;
export type Currency = 'EUR';
export type VehicleStatus = 'Available' | 'Rented' | 'Maintenance' | 'OutOfService' | 'Reserved';
export type LicensePlate = string;
export type Manufacturer = string;
export type VehicleModel = string;
export type ManufacturingYear = number;
export type ImageUrl = string;

/**
 * Vehicle data model matching the backend VehicleDto
 * All prices include German VAT (19%)
 */
export interface Vehicle {
  id: VehicleId;
  name: VehicleName;
  categoryCode: CategoryCode;
  categoryName: CategoryName;
  locationCode: LocationCode;
  city: CityName;
  seats: SeatingCapacity;
  fuelType: FuelType;
  transmissionType: TransmissionType;
  dailyRateNet: DailyRate;
  dailyRateVat: DailyRate;
  dailyRateGross: DailyRate;
  currency: Currency;
  status: VehicleStatus;
  licensePlate: LicensePlate | null;
  manufacturer: Manufacturer;
  model: VehicleModel;
  year: ManufacturingYear;
  imageUrl: ImageUrl | null;
}

/**
 * Search query parameters for vehicle search
 */
export interface VehicleSearchQuery {
  pickupDate?: string; // ISO date string
  returnDate?: string; // ISO date string
  locationCode?: LocationCode;
  categoryCode?: CategoryCode;
  minSeats?: SeatingCapacity;
  fuelType?: FuelType;
  transmissionType?: TransmissionType;
  maxDailyRateGross?: DailyRate;
  status?: VehicleStatus;
  pageNumber?: number;
  pageSize?: number;
}

/**
 * Search result with pagination metadata
 */
export interface VehicleSearchResult {
  vehicles: Vehicle[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
  hasPreviousPage?: boolean;
  hasNextPage?: boolean;
}

/**
 * Add vehicle to fleet request
 */
export interface AddVehicleRequest {
  basicInfo: {
    name: VehicleName;
    manufacturer?: Manufacturer;
    model?: VehicleModel;
    year?: ManufacturingYear;
    imageUrl?: ImageUrl;
  };
  specifications: {
    category: CategoryCode;
    seats: SeatingCapacity;
    fuelType: FuelType;
    transmissionType: TransmissionType;
  };
  locationAndPricing: {
    locationCode: LocationCode;
    dailyRateNet: DailyRate;
  };
  registration?: {
    licensePlate?: LicensePlate;
  };
}

/**
 * Add vehicle result
 */
export interface AddVehicleResult {
  vehicleId: VehicleId;
  name: VehicleName;
  category: CategoryCode;
  location: LocationCode;
  dailyRateGross: DailyRate;
  status: VehicleStatus;
}

/**
 * Update vehicle status request
 */
export interface UpdateVehicleStatusRequest {
  status: VehicleStatus;
}

/**
 * Update vehicle location request
 */
export interface UpdateVehicleLocationRequest {
  locationCode: LocationCode;
}

/**
 * Update vehicle daily rate request
 */
export interface UpdateVehicleDailyRateRequest {
  dailyRateNet: DailyRate;
}

/**
 * Constants for vehicle categories
 */
export const VEHICLE_CATEGORIES: ReadonlyArray<{code: CategoryCode, name: CategoryName}> = [
  { code: 'KLEIN', name: 'Kleinwagen' },
  { code: 'KOMPAKT', name: 'Kompaktklasse' },
  { code: 'MITTEL', name: 'Mittelklasse' },
  { code: 'OBER', name: 'Oberklasse' },
  { code: 'VAN', name: 'Van/Transporter' },
  { code: 'SUV', name: 'SUV' }
] as const;

/**
 * Constants for fuel types
 */
export const FUEL_TYPES: ReadonlyArray<FuelType> = ['Petrol', 'Diesel', 'Electric', 'Hybrid'] as const;

/**
 * Constants for transmission types
 */
export const TRANSMISSION_TYPES: ReadonlyArray<TransmissionType> = ['Manual', 'Automatic'] as const;

/**
 * Constants for vehicle statuses
 */
export const VEHICLE_STATUSES: ReadonlyArray<{code: VehicleStatus, label: string}> = [
  { code: 'Available', label: 'Verfügbar' },
  { code: 'Rented', label: 'Vermietet' },
  { code: 'Maintenance', label: 'Wartung' },
  { code: 'OutOfService', label: 'Außer Betrieb' },
  { code: 'Reserved', label: 'Reserviert' }
] as const;
