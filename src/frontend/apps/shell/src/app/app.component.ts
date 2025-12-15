import { Component, inject, OnInit, signal } from '@angular/core';
import { Router, RouterOutlet } from '@angular/router';
import { NavigationComponent } from '@orange-car-rental/shared-ui';
import { BaseAuthService } from '@orange-car-rental/auth';
import { SHELL_NAV_CONFIG } from './constants/navigation.config';
import type { NavConfig } from '@orange-car-rental/shared-ui';

/**
 * Shell Application Component
 *
 * Provides the unified navigation and layout for all micro-frontends.
 * Handles authentication state and role-based navigation filtering.
 */
@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, NavigationComponent],
  template: `
    <div class="shell-container">
      <ocr-navigation
        [config]="navConfig"
        [isAuthenticated]="isAuthenticated()"
        [username]="username()"
        [userRoles]="userRoles()"
        (loginClick)="onLogin()"
        (logoutClick)="onLogout()"
      />

      <main class="shell-main">
        <router-outlet></router-outlet>
      </main>

      <footer class="shell-footer">
        <p>&copy; 2025 Orange Car Rental</p>
      </footer>
    </div>
  `,
  styles: [`
    .shell-container {
      display: flex;
      flex-direction: column;
      min-height: 100vh;
    }

    .shell-main {
      flex: 1;
      padding: 2rem;
      background-color: #f5f5f5;
    }

    .shell-footer {
      background-color: #333;
      color: white;
      text-align: center;
      padding: 1rem;
    }

    .shell-footer p {
      margin: 0;
    }
  `]
})
export class AppComponent implements OnInit {
  private readonly authService = inject(BaseAuthService);
  private readonly router = inject(Router);

  /** Navigation configuration */
  readonly navConfig: NavConfig = SHELL_NAV_CONFIG;

  /** Authentication state signals */
  readonly isAuthenticated = signal(false);
  readonly username = signal('');
  readonly userRoles = signal<string[]>([]);

  ngOnInit(): void {
    this.updateAuthState();
  }

  /**
   * Update authentication state from auth service
   */
  private updateAuthState(): void {
    this.isAuthenticated.set(this.authService.isAuthenticated());
    this.username.set(this.authService.getUsername());
    this.userRoles.set(this.authService.getUserRoles());
  }

  /**
   * Handle login button click - navigate to login page
   */
  onLogin(): void {
    this.router.navigate(['/login']);
  }

  /**
   * Handle logout button click
   */
  onLogout(): void {
    this.authService.logout();
  }
}
