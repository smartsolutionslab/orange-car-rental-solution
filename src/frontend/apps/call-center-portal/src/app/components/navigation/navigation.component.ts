import { Component, computed, inject, signal } from '@angular/core';
import type { OnInit } from '@angular/core';
import { NavigationComponent as SharedNavigationComponent } from '@orange-car-rental/ui-components';
import { AuthService } from '../../services/auth.service';
import { CALL_CENTER_NAV_ITEMS } from '../../constants/navigation.config';

/**
 * Navigation component for the call center portal
 * Features:
 * - Dynamic navigation items with auth-based visibility
 * - Role-based filtering (agents see different items than supervisors)
 * - Reactive state using signals
 */
@Component({
  selector: 'app-navigation',
  standalone: true,
  imports: [SharedNavigationComponent],
  template: `
    <ocr-navigation
      [title]="'Orange Car Rental - Call Center'"
      [navItems]="navItems()"
      [isAuthenticated]="isAuthenticated()"
      [username]="username()"
      (loginClick)="login()"
      (logoutClick)="logout()"
    />
  `,
})
export class NavigationComponent implements OnInit {
  private readonly authService = inject(AuthService);

  /** All navigation items configuration */
  private readonly allNavItems = CALL_CENTER_NAV_ITEMS;

  /** Authentication state */
  protected readonly isAuthenticated = signal(false);

  /** Current username */
  protected readonly username = signal('');

  /** Current user roles */
  protected readonly userRoles = signal<string[]>([]);

  /** Filtered navigation items based on auth state and roles */
  protected readonly navItems = computed(() => {
    const authenticated = this.isAuthenticated();
    const roles = this.userRoles();

    return this.allNavItems.filter((item) => {
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

  ngOnInit(): void {
    this.updateAuthState();
  }

  /**
   * Update authentication state from auth service
   */
  private updateAuthState(): void {
    const authenticated = this.authService.isAuthenticated();
    this.isAuthenticated.set(authenticated);
    if (authenticated) {
      this.username.set(this.authService.getUsername());
      this.userRoles.set(this.authService.getUserRoles());
    }
  }

  /**
   * Trigger login flow
   */
  protected login(): void {
    this.authService.login();
  }

  /**
   * Trigger logout flow
   */
  protected logout(): void {
    this.authService.logout();
  }
}
