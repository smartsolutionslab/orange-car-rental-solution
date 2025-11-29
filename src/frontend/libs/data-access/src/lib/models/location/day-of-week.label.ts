/**
 * Day of week labels (German)
 */
import type { DayOfWeek } from './day-of-week.enum';
import { DayOfWeek as DOW } from './day-of-week.enum';

export const DayOfWeekLabel: Record<DayOfWeek, string> = {
  [DOW.Monday]: 'Montag',
  [DOW.Tuesday]: 'Dienstag',
  [DOW.Wednesday]: 'Mittwoch',
  [DOW.Thursday]: 'Donnerstag',
  [DOW.Friday]: 'Freitag',
  [DOW.Saturday]: 'Samstag',
  [DOW.Sunday]: 'Sonntag',
};
