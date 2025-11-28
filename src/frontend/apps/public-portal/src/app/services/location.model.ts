import { LocationCode, CityName } from './vehicle.model';
import { PostalCode } from './reservation.model';

// Branded types for location domain
export type LocationName = string;
export type StreetAddress = string;
export type FullAddress = string;

// Union type for location status
export type LocationStatus = 'Active' | 'Inactive' | 'Maintenance';

/**
 * Location model representing a rental location
 */
export interface Location {
  readonly code: LocationCode;
  readonly name: LocationName;
  readonly street: StreetAddress;
  readonly city: CityName;
  readonly postalCode: PostalCode;
  readonly fullAddress: FullAddress;
  readonly status?: LocationStatus;
}

/**
 * Location with availability information
 */
export interface LocationWithAvailability extends Location {
  readonly availableVehicleCount: number;
  readonly isOpen: boolean;
}

/**
 * Location operating hours
 */
export interface LocationHours {
  readonly dayOfWeek: DayOfWeek;
  readonly openTime: TimeString;
  readonly closeTime: TimeString;
  readonly isClosed: boolean;
}

// Additional helper types
export type DayOfWeek = 'Monday' | 'Tuesday' | 'Wednesday' | 'Thursday' | 'Friday' | 'Saturday' | 'Sunday';
export type TimeString = string; // Format: "HH:MM"

/**
 * Constants for location statuses with German labels
 */
export const LOCATION_STATUSES = [
  { code: 'Active', label: 'Aktiv' },
  { code: 'Inactive', label: 'Inaktiv' },
  { code: 'Maintenance', label: 'Wartung' }
] as const satisfies readonly { code: LocationStatus; label: string }[];

/**
 * Constants for days of week with German labels
 */
export const DAYS_OF_WEEK = [
  { code: 'Monday', label: 'Montag' },
  { code: 'Tuesday', label: 'Dienstag' },
  { code: 'Wednesday', label: 'Mittwoch' },
  { code: 'Thursday', label: 'Donnerstag' },
  { code: 'Friday', label: 'Freitag' },
  { code: 'Saturday', label: 'Samstag' },
  { code: 'Sunday', label: 'Sonntag' }
] as const satisfies readonly { code: DayOfWeek; label: string }[];
