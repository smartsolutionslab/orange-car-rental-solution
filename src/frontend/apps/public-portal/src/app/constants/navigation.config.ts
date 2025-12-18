import { ICON_CAR, ICON_CALENDAR, ICON_LOCATION, ICON_EMAIL } from '@orange-car-rental/util';
import type { NavItem } from '@orange-car-rental/shared';

/**
 * Navigation configuration for the public portal
 */
export const PUBLIC_PORTAL_NAV_ITEMS: readonly NavItem[] = [
  { path: '/', label: 'Fahrzeuge', icon: ICON_CAR, exactMatch: true },
  { path: '/my-bookings', label: 'Meine Buchungen', icon: ICON_CALENDAR, requiresAuth: true },
  { path: '/locations', label: 'Standorte', icon: ICON_LOCATION },
  { path: '/contact', label: 'Kontakt', icon: ICON_EMAIL },
] as const;
