/**
 * Navigation item filtering utilities
 */
import type { NavItem } from './nav-item.type';

/**
 * Filter navigation items based on authentication state and user roles
 *
 * @param items - Array of navigation items to filter
 * @param isAuthenticated - Whether the user is authenticated
 * @param userRoles - Array of roles the user has (optional)
 * @returns Filtered array of navigation items
 *
 * @example
 * // Basic usage without roles
 * const visibleItems = filterNavItems(navItems, isAuthenticated());
 *
 * @example
 * // With role-based filtering
 * const visibleItems = filterNavItems(navItems, isAuthenticated(), userRoles());
 */
export function filterNavItems(
  items: readonly NavItem[],
  isAuthenticated: boolean,
  userRoles: readonly string[] = [],
): NavItem[] {
  return items.filter((item) => {
    // Hide items that require auth when not authenticated
    if (item.requiresAuth && !isAuthenticated) return false;

    // Hide items that should be hidden when authenticated
    if (item.hideWhenAuth && isAuthenticated) return false;

    // Check role-based access if roles are specified
    if (item.roles && item.roles.length > 0) {
      const hasRequiredRole = item.roles.some((role) => userRoles.includes(role));
      if (!hasRequiredRole) return false;
    }

    return true;
  });
}
