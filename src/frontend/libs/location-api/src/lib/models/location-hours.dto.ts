/**
 * Location operating hours
 */
import type { DayOfWeek } from "./day-of-week.enum";
import type { TimeString } from "./time-string.type";

export type LocationHours = {
  readonly dayOfWeek: DayOfWeek;
  readonly openTime: TimeString;
  readonly closeTime: TimeString;
  readonly isClosed: boolean;
};
