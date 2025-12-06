import type { NavItem } from '@orange-car-rental/data-access';

// Re-export NavItem from data-access for convenience
export type { NavItem } from '@orange-car-rental/data-access';

/**
 * Navigation configuration
 */
export interface NavConfig {
  /** Navigation items */
  items: readonly NavItem[];
  /** Brand/logo title */
  brandTitle?: string;
  /** Brand logo SVG path */
  brandIcon?: string;
  /** Login button label */
  loginLabel?: string;
  /** Logout button label */
  logoutLabel?: string;
}
