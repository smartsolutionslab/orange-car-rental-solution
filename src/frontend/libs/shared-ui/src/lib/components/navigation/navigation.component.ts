import { Component, computed, input, output } from "@angular/core";
import { RouterLink, RouterLinkActive } from "@angular/router";
import { CommonModule } from "@angular/common";
import type { NavConfig } from "./types";

/**
 * Default car icon for branding
 */
const DEFAULT_BRAND_ICON =
  '<path d="M18.92 6.01C18.72 5.42 18.16 5 17.5 5h-11c-.66 0-1.22.42-1.42 1.01L3 12v8c0 .55.45 1 1 1h1c.55 0 1-.45 1-1v-1h12v1c0 .55.45 1 1 1h1c.55 0 1-.45 1-1v-8l-2.08-5.99zM6.5 16c-.83 0-1.5-.67-1.5-1.5S5.67 13 6.5 13s1.5.67 1.5 1.5S7.33 16 6.5 16zm11 0c-.83 0-1.5-.67-1.5-1.5s.67-1.5 1.5-1.5 1.5.67 1.5 1.5-.67 1.5-1.5 1.5zM5 11l1.5-4.5h11L19 11H5z"/>';

/**
 * Login icon
 */
const LOGIN_ICON =
  '<path d="M11 7L9.6 8.4l2.6 2.6H2v2h10.2l-2.6 2.6L11 17l5-5-5-5zm9 12h-8v2h8c1.1 0 2-.9 2-2V5c0-1.1-.9-2-2-2h-8v2h8v14z"/>';

/**
 * Logout icon
 */
const LOGOUT_ICON =
  '<path d="M17 7l-1.4 1.4 2.6 2.6H8v2h10.2l-2.6 2.6L17 17l5-5-5-5zM4 5h8V3H4c-1.1 0-2 .9-2 2v14c0 1.1.9 2 2 2h8v-2H4V5z"/>';

/**
 * Shared navigation component with configurable items and auth support.
 *
 * Usage:
 * ```html
 * <ocr-navigation
 *   [config]="navConfig"
 *   [isAuthenticated]="isLoggedIn"
 *   [username]="currentUser"
 *   [userRoles]="roles"
 *   (loginClick)="onLogin()"
 *   (logoutClick)="onLogout()"
 * />
 * ```
 */
@Component({
  selector: "ocr-navigation",
  standalone: true,
  imports: [RouterLink, RouterLinkActive, CommonModule],
  templateUrl: "./navigation.component.html",
  styleUrl: "./navigation.component.css",
})
export class NavigationComponent {
  /** Navigation configuration */
  config = input.required<NavConfig>();

  /** Whether user is authenticated */
  isAuthenticated = input<boolean>(false);

  /** Current username to display */
  username = input<string>("");

  /** User roles for role-based filtering */
  userRoles = input<string[]>([]);

  /** Emitted when login button is clicked */
  loginClick = output<void>();

  /** Emitted when logout button is clicked */
  logoutClick = output<void>();

  /** Brand title from config or default */
  protected readonly brandTitle = computed(
    () => this.config().brandTitle ?? "Orange Car Rental",
  );

  /** Brand icon from config or default */
  protected readonly brandIcon = computed(
    () => this.config().brandIcon ?? DEFAULT_BRAND_ICON,
  );

  /** Login label from config or default */
  protected readonly loginLabel = computed(
    () => this.config().loginLabel ?? "Anmelden",
  );

  /** Logout label from config or default */
  protected readonly logoutLabel = computed(
    () => this.config().logoutLabel ?? "Abmelden",
  );

  /** Icons for template */
  protected readonly icons = {
    login: LOGIN_ICON,
    logout: LOGOUT_ICON,
  };

  /** Filtered navigation items based on auth state and roles */
  protected readonly navItems = computed(() => {
    const authenticated = this.isAuthenticated();
    const roles = this.userRoles();
    const items = this.config().items;

    return items.filter((item) => {
      // Hide items that require auth when not authenticated
      if (item.requiresAuth && !authenticated) {
        return false;
      }
      // Hide items that should be hidden when authenticated
      if (item.hideWhenAuth && authenticated) {
        return false;
      }
      // Check role-based access
      if (item.roles && item.roles.length > 0) {
        const hasRequiredRole = item.roles.some((role) => roles.includes(role));
        if (!hasRequiredRole) {
          return false;
        }
      }
      return true;
    });
  });

  /**
   * Handle login button click
   */
  protected onLogin(): void {
    this.loginClick.emit();
  }

  /**
   * Handle logout button click
   */
  protected onLogout(): void {
    this.logoutClick.emit();
  }
}
