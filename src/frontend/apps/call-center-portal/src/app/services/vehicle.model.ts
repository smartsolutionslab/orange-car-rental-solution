/**
 * Vehicle data model matching the backend VehicleDto
 * All prices include German VAT (19%)
 */
export interface Vehicle {
  id: string;
  name: string;
  categoryCode: string;
  categoryName: string;
  locationCode: string;
  city: string;
  seats: number;
  fuelType: string;
  transmissionType: string;
  dailyRateNet: number;
  dailyRateVat: number;
  dailyRateGross: number;
  currency: string;
  status: string;
  licensePlate: string | null;
  manufacturer: string;
  model: string;
  year: number;
  imageUrl: string | null;
}

/**
 * Search query parameters for vehicle search
 */
export interface VehicleSearchQuery {
  pickupDate?: string;
  returnDate?: string;
  locationCode?: string;
  categoryCode?: string;
  minSeats?: number;
  fuelType?: string;
  transmissionType?: string;
  maxDailyRateGross?: number;
  status?: string;
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
