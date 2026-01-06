/**
 * Update vehicle daily rate request
 */
import type { DailyRate } from "./daily-rate.type";

export type UpdateVehicleDailyRateRequest = {
  readonly dailyRateNet: DailyRate;
};
