import { Component, computed, inject, signal } from '@angular/core';
import type { OnInit } from '@angular/core';
import { NavigationComponent as SharedNavigationComponent } from '@orange-car-rental/ui-components';
import { filterNavItems } from '@orange-car-rental/shared';
import { AuthService } from '../../services/auth.service';
import { PUBLIC_PORTAL_NAV_ITEMS } from '../../constants/navigation.config';

/**
 * Navigation component for the public portal
 * Features:
 * - Dynamic navigation items with auth-based visibility
 * - Reactive state using signals
 * - Role-based filtering support
 */
@Component({
  selector: 'app-navigation',
  standalone: true,
  imports: [SharedNavigationComponent],
  template: `
    <ocr-navigation
      [title]="'Orange Car Rental'"
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
  private readonly allNavItems = PUBLIC_PORTAL_NAV_ITEMS;

  /** Authentication state */
  protected readonly isAuthenticated = signal(false);

  /** Current username */
  protected readonly username = signal('');

  /** Filtered navigation items based on auth state */
  protected readonly navItems = computed(() =>
    filterNavItems(this.allNavItems, this.isAuthenticated()),
  );

  ngOnInit(): void {
    this.updateAuthState();
  }

  /**
   * Update authentication state from auth service
   */
  private updateAuthState(): void {
    const authenticated = this.authService.isAuthenticated();
    this.isAuthenticated.set(authenticated);
    if (authenticated) this.username.set(this.authService.getUsername());
  }

  /**
   * Trigger login flow (Keycloak redirect)
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
