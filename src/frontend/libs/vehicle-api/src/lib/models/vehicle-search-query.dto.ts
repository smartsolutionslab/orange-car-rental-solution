/**
 * Search query parameters for vehicle search
 */
import type { ISODateString } from "@orange-car-rental/shared";
import type { LocationCode } from "@orange-car-rental/location-api";
import type { CategoryCode } from "./category-code.type";
import type { SeatingCapacity } from "./seating-capacity.type";
import type { DailyRate } from "./daily-rate.type";
import type { FuelType } from "./fuel-type.enum";
import type { TransmissionType } from "./transmission-type.enum";
import type { VehicleStatus } from "./vehicle-status.enum";

export type VehicleSearchQuery = {
  readonly pickupDate?: ISODateString;
  readonly returnDate?: ISODateString;
  readonly locationCode?: LocationCode;
  readonly categoryCode?: CategoryCode;
  readonly minSeats?: SeatingCapacity;
  readonly fuelType?: FuelType;
  readonly transmissionType?: TransmissionType;
  readonly maxDailyRateGross?: DailyRate;
  readonly status?: VehicleStatus;
  readonly pageNumber?: number;
  readonly pageSize?: number;
};
