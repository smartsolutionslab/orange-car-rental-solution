import type { NavItem } from '@orange-car-rental/shared';

/**
 * SVG icon paths for navigation items
 */
const ICON_CAR =
  '<path d="M18.92 6.01C18.72 5.42 18.16 5 17.5 5h-11c-.66 0-1.22.42-1.42 1.01L3 12v8c0 .55.45 1 1 1h1c.55 0 1-.45 1-1v-1h12v1c0 .55.45 1 1 1h1c.55 0 1-.45 1-1v-8l-2.08-5.99zM6.5 16c-.83 0-1.5-.67-1.5-1.5S5.67 13 6.5 13s1.5.67 1.5 1.5S7.33 16 6.5 16zm11 0c-.83 0-1.5-.67-1.5-1.5s.67-1.5 1.5-1.5 1.5.67 1.5 1.5-.67 1.5-1.5 1.5zM5 11l1.5-4.5h11L19 11H5z"/>';
const ICON_LOCATION =
  '<path d="M12 2C8.13 2 5 5.13 5 9c0 5.25 7 13 7 13s7-7.75 7-13c0-3.87-3.13-7-7-7zm0 9.5c-1.38 0-2.5-1.12-2.5-2.5s1.12-2.5 2.5-2.5 2.5 1.12 2.5 2.5-1.12 2.5-2.5 2.5z"/>';
const ICON_CALENDAR =
  '<path d="M19 3h-1V1h-2v2H8V1H6v2H5c-1.11 0-1.99.9-1.99 2L3 19c0 1.1.89 2 2 2h14c1.1 0 2-.9 2-2V5c0-1.1-.9-2-2-2zm0 16H5V8h14v11zM9 10H7v2h2v-2zm4 0h-2v2h2v-2zm4 0h-2v2h2v-2zm-8 4H7v2h2v-2zm4 0h-2v2h2v-2zm4 0h-2v2h2v-2z"/>';

/**
 * Shell navigation items configuration
 *
 * Items are filtered based on:
 * - requiresAuth: Only shown when user is authenticated
 * - hideWhenAuth: Only shown when user is NOT authenticated
 * - roles: Only shown when user has one of the specified roles
 */
export const SHELL_NAV_ITEMS: readonly NavItem[] = [
  // Public items (visible to all)
  {
    path: '/',
    label: 'Fahrzeuge',
    icon: ICON_CAR,
    exactMatch: true,
  },
  {
    path: '/locations',
    label: 'Standorte',
    icon: ICON_LOCATION,
  },
  // Customer items (visible when authenticated)
  {
    path: '/my-bookings',
    label: 'Meine Buchungen',
    icon: ICON_CALENDAR,
    requiresAuth: true,
  },
];

/**
 * Default navigation configuration for the shell
 */
export const SHELL_NAV_CONFIG = {
  items: SHELL_NAV_ITEMS,
  brandTitle: 'Orange Car Rental',
  loginLabel: 'Anmelden',
  logoutLabel: 'Abmelden',
} as const;
