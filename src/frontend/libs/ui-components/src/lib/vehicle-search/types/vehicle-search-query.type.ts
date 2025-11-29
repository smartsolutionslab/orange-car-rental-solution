/**
 * Vehicle search query interface
 */
export type VehicleSearchQuery = {
  readonly pickupDate?: string;
  readonly returnDate?: string;
  readonly locationCode?: string;
  readonly categoryCode?: string;
  readonly minSeats?: number;
  readonly fuelType?: string;
  readonly transmissionType?: string;
  readonly maxDailyRateGross?: number;
  readonly pageNumber?: number;
  readonly pageSize?: number;
};
