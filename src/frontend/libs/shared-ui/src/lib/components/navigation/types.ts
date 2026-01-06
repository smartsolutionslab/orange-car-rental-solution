import type { NavItem } from "@orange-car-rental/shared";

// Re-export NavItem from shared for convenience
export type { NavItem } from "@orange-car-rental/shared";

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
