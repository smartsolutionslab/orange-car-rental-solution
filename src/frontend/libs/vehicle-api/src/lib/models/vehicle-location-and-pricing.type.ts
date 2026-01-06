/**
 * Vehicle location and pricing for add request
 */
import type { LocationCode } from "@orange-car-rental/location-api";
import type { DailyRate } from "./daily-rate.type";

export type VehicleLocationAndPricing = {
  readonly locationCode: LocationCode;
  readonly dailyRateNet: DailyRate;
};
