/**
 * Navigation item configuration for dynamic navigation menus
 */
export type NavItem = {
  /** Route path */
  readonly path: string;
  /** Display label */
  readonly label: string;
  /** SVG icon path content */
  readonly icon: string;
  /** Whether the item requires authentication (default: false) */
  readonly requiresAuth?: boolean;
  /** Whether the item should be hidden when authenticated (default: false) */
  readonly hideWhenAuth?: boolean;
  /** Required roles to see this item (if empty, visible to all authenticated users) */
  readonly roles?: readonly string[];
  /** Whether to use exact path matching for active state (default: false) */
  readonly exactMatch?: boolean;
};
