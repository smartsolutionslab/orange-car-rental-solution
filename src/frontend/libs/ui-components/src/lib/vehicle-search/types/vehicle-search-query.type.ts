/**
 * Vehicle search query interface
 */
import type { ISODateString } from '@orange-car-rental/shared';
import type { LocationCode } from '@orange-car-rental/location-api';
import type {
  CategoryCode,
  SeatingCapacity,
  DailyRate,
  FuelType,
  TransmissionType,
} from '@orange-car-rental/vehicle-api';

export type VehicleSearchQuery = {
  readonly pickupDate?: ISODateString;
  readonly returnDate?: ISODateString;
  readonly locationCode?: LocationCode;
  readonly categoryCode?: CategoryCode;
  readonly minSeats?: SeatingCapacity;
  readonly fuelType?: FuelType;
  readonly transmissionType?: TransmissionType;
  readonly maxDailyRateGross?: DailyRate;
  readonly pageNumber?: number;
  readonly pageSize?: number;
};
