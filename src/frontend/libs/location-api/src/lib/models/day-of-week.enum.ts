/**
 * Day of week enum - use DayOfWeek.Monday instead of 'Monday'
 */
export const DayOfWeek = {
  Monday: 'Monday',
  Tuesday: 'Tuesday',
  Wednesday: 'Wednesday',
  Thursday: 'Thursday',
  Friday: 'Friday',
  Saturday: 'Saturday',
  Sunday: 'Sunday',
} as const;

export type DayOfWeek = (typeof DayOfWeek)[keyof typeof DayOfWeek];
