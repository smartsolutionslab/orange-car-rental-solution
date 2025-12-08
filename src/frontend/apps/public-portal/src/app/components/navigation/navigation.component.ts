import { Component, computed, inject, signal } from '@angular/core';
import type { OnInit } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../services/auth.service';
import { ICON_LOGIN, ICON_LOGOUT } from '@orange-car-rental/util';
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
  imports: [RouterLink, RouterLinkActive, CommonModule],
  templateUrl: './navigation.component.html',
  styleUrl: './navigation.component.css'
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
  protected readonly navItems = computed(() => {
    const authenticated = this.isAuthenticated();
    return this.allNavItems.filter(item => {
      // Hide items that require auth when not authenticated
      if (item.requiresAuth && !authenticated) {
        return false;
      }
      // Hide items that should be hidden when authenticated
      if (item.hideWhenAuth && authenticated) {
        return false;
      }
      return true;
    });
  });

  /** Icons for template access */
  protected readonly icons = {
    login: ICON_LOGIN,
    logout: ICON_LOGOUT,
  };

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
    }
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
