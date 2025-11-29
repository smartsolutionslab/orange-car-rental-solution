import { Injectable, inject } from '@angular/core';
import { Router } from '@angular/router';
import type { CanActivateFn } from '@angular/router';
import { KeycloakService } from 'keycloak-angular';
import { logError } from '@orange-car-rental/util';

@Injectable({
  providedIn: 'root'
})
export class AuthGuardService {
  constructor(
    private keycloakService: KeycloakService,
    private router: Router
  ) {}

  async canActivate(): Promise<boolean> {
    try {
      const authenticated = this.keycloakService.isLoggedIn();

      if (!authenticated) {
        // Redirect to login
        await this.keycloakService.login({
          redirectUri: window.location.origin + window.location.pathname
        });
        return false;
      }

      return true;
    } catch (error) {
      logError('AuthGuard', 'Auth guard error', error);
      // If Keycloak is not available, redirect to login page
      this.router.navigate(['/login']);
      return false;
    }
  }
}

/**
 * Auth guard for protecting routes
 */
export const authGuard: CanActivateFn = async () => {
  const authGuardService = inject(AuthGuardService);
  return authGuardService.canActivate();
};
