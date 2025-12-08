import {
  ICON_CAR,
  ICON_CALENDAR,
  ICON_PERSON,
  ICON_LOCATION,
  ICON_EMAIL,
} from '@orange-car-rental/util';
import type { NavItem } from '@orange-car-rental/shared';

/** Call center roles for access control */
export const CALL_CENTER_ROLES = {
  AGENT: 'call-center-agent',
  SUPERVISOR: 'call-center-supervisor',
  ADMIN: 'admin',
} as const;

/**
 * Navigation configuration for the call center portal
 */
export const CALL_CENTER_NAV_ITEMS: readonly NavItem[] = [
  { path: '', label: 'Fahrzeuge', icon: ICON_CAR, exactMatch: true, requiresAuth: true },
  { path: 'reservations', label: 'Buchungen', icon: ICON_CALENDAR, requiresAuth: true },
  { path: 'customers', label: 'Kunden', icon: ICON_PERSON, requiresAuth: true },
  { path: 'locations', label: 'Standorte', icon: ICON_LOCATION, requiresAuth: true, roles: [CALL_CENTER_ROLES.SUPERVISOR, CALL_CENTER_ROLES.ADMIN] },
  { path: 'contact', label: 'Kontakt', icon: ICON_EMAIL },
] as const;
